using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UMS.Data;
using UMS.Models;
using UMS.Models.Entities;
using UMS.Enums;
using UMS.Repositories;
using Dapper;
using Azure.Core;
using System.Diagnostics;
using UMS.Encryption;

namespace UMS.Services
{
    public class JWTService(ApplicationDbContext dbContext, IConfiguration configuration, IDapperRepository repository, IEmployeeService empService,AesEncryption aesEncryption)
    {
        private readonly byte[] _key = Encoding.UTF8.GetBytes(configuration["AESKEYS:AESEncryptionKey"] ?? string.Empty);
        private readonly byte[] _iv = Encoding.UTF8.GetBytes(configuration["AESKEYS:AESEncryptionIV"] ?? string.Empty);
        // 1. Validate credentials only (NO token generation)
        public (bool IsValid, LoginResponseModel? Token) ValidateLoginCredentials(LoginRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                return (false, null);

            if (request.UserName == ConstantValues.ADMIN_DEFAULT_USERNAME &&
                request.Password == ConstantValues.ADMIN_DEFAULT_PASSWORD)
            {
                var token = GenerateAccessToken(
                    ConstantValues.ADMIN_DEFAULT_ID,
                    ConstantValues.ADMIN_DEFAULT_FULLNAME,
                    ConstantValues.ADMIN_DEFAULT_EMAIL,
                    ConstantValues.ADMIN_DEFAULT_USERNAME,
                    UMS.Enums.Roles.Admin.ToString());

                return (true, token);
            }

            string decryptedPass = aesEncryption.DecryptString(request.Password,_key,_iv);
            var employee = dbContext.Employees
                .FirstOrDefault(u => u.UserName == request.UserName && u.Password == decryptedPass);

            if (employee != null)
            {
                return (true, null);
            }

            var manager = dbContext.Managers
                .FirstOrDefault(u => u.UserName == request.UserName && u.Password == decryptedPass);

            if (manager != null)
            {
                return (true, null);
            }

            return (false, null);
        }

        // 2. Verify OTP and generate token if valid
        public LoginResponseModel VerifyOtp(string otp, bool isForgotPassword = false)
        {
            string email;
            var empEmail = empService.GetEmployeeEmail(otp);
            const string managerEmail = ConstantValues.MANAGER_DEFAULT_EMAIL;

            if (!string.IsNullOrWhiteSpace(empEmail))
            {
                email = empEmail;
            }
            else if (!string.IsNullOrWhiteSpace(managerEmail))
            {
                email = managerEmail;
            }
            else
            {
                throw new ArgumentException("Invalid OTP or email.");
            }


            if (string.IsNullOrWhiteSpace(otp))
                throw new ArgumentException("OTP cannot be null or empty.");

            var existingOtp = dbContext.LoginVerificationOTPs.FirstOrDefault(o => o.OTP == otp && o.Email == email)
                              ?? throw new ArgumentException("Invalid OTP.");

            var deleteExistingOtp = dbContext.LoginVerificationOTPs
                .FirstOrDefault(o => o.Email == email && o.OTP == otp);
            if (deleteExistingOtp != null)
            {
                dbContext.LoginVerificationOTPs.Remove(deleteExistingOtp);
                dbContext.SaveChanges();
            }
            if (existingOtp.ExpiresAt < DateTime.Now)
                throw new ArgumentException("OTP has expired.");

            var employee = dbContext.Employees.FirstOrDefault(e => e.Email == existingOtp.Email);
            Manager? manager = null;
            if (employee == null && existingOtp.Email == ConstantValues.MANAGER_DEFAULT_EMAIL)
            {
                manager = dbContext.Managers.FirstOrDefault();
            }

            var isEmployee = employee != null;
            if (!isEmployee && manager == null)
                throw new ArgumentException("Manager not found.");
            var id = isEmployee ? employee.Id : manager!.Id;
            var fullName = isEmployee ? employee.FullName : manager!.FullName;
            var userName = isEmployee ? employee.UserName : manager!.UserName;
            var role = isEmployee ? Enums.Roles.Employee.ToString() : UMS.Enums.Roles.Manager.ToString();

            return GenerateAccessToken(id, fullName, existingOtp.Email, userName, role);
        }


        public string UserRole(LoginResponseModel response)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(response.AccessToken);
            var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role || c.Type == "role")?.Value;
            return roleClaim ?? "";
        }
        
        // 3. Token generation logic (JWT + Refresh)
        private LoginResponseModel GenerateAccessToken(int id, string fullName, string email, string userName, string role)
        {
            var issuer = configuration["JWTConfig:Issuer"];
            var audience = configuration["JWTConfig:Audience"];
            var key = configuration["JWTConfig:Key"];
            var tokenValidityMins = configuration.GetValue<int>("JWTConfig:TokenValidityMins");

            if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
                throw new ArgumentException("JWT configuration is missing.");

            var expiration = DateTime.Now.AddMinutes(tokenValidityMins);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = issuer,
                Audience = audience,
                Expires = expiration,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new LoginResponseModel
            {
                UserName = userName,
                Email = email,
                AccessToken = accessToken,
                Expiration = expiration,
                RefreshTokenExpiration = DateTime.Now.AddDays(5),
                RefreshToken = GenerateRefreshToken(id, accessToken, role)
            };
        }

        // 4. Generate and store Refresh Token
        private string GenerateRefreshToken(int userId, string accessToken, string role)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var expiry = DateTime.Now.AddDays(5);

            if (role == UMS.Enums.Roles.Employee.ToString())
            {
                var token = dbContext.RefreshTokenEmployees.FirstOrDefault(t => t.EmployeeId == userId);
                if (token != null)
                {
                    token.RefreshUserToken = refreshToken;
                    token.ExpiresAt = expiry;
                }
                else
                {
                    DynamicParameters parameters = new();
                    parameters.Add("@EmployeeId", userId);
                    parameters.Add("@AccessToken", accessToken);
                    parameters.Add("@RefreshUserToken", refreshToken);
                    parameters.Add("@ExpiresAt", expiry);
                    parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                    repository.Execute(StoredProcedures.REFRESH_TOKEN_EMPLOYEE_INSERT, parameters);

                    int result = parameters.Get<int>("@Result");
                    if (result != 1)
                    {
                        throw new Exception("Error generating refresh token for employee.");
                    }
                }
            }
            else if (role == UMS.Enums.Roles.Manager.ToString())
            {
                var token = dbContext.RefreshTokenManagers.FirstOrDefault(t => t.ManagerId == userId);
                if (token != null)
                {
                    token.RefreshUserToken = refreshToken;
                    token.ExpiresAt = expiry;
                }
                else
                {
                    DynamicParameters parameters = new();
                    parameters.Add("@ManagerId", userId);
                    parameters.Add("@AccessToken", accessToken);
                    parameters.Add("@RefreshUserToken", refreshToken);
                    parameters.Add("@ExpiresAt", expiry);
                    parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                    repository.Execute(StoredProcedures.REFRESH_TOKEN_MANAGER_INSERT, parameters);

                    int result = parameters.Get<int>("@Result");
                    if (result != 1)
                    {
                        throw new Exception("Error generating refresh token for manager.");
                    }
                }
            }
            else if (role != UMS.Enums.Roles.Admin.ToString())
            {
                throw new ArgumentException("Invalid role specified for refresh token generation.");
            }

            return refreshToken;
        }
        public string PasswordReset(string password,string email)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.");
            }

            DynamicParameters parameters = new();
            parameters.Add("@Password", password);
            parameters.Add("@Email", email);
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            repository.Execute(StoredProcedures.PASSWORD_RESET, parameters);
            int result = parameters.Get<int>("@Result");
            if (result != 1)
            {
                throw new Exception("Error resetting password.");
            }
            return "Password reset successfully.";
        }
    }
}
