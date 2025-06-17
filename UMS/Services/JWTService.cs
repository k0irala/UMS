using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UMS.Data;
using UMS.Models;

namespace UMS.Services
{
    public class JWTService(ApplicationDbContext dbContext,IConfiguration configuration)
    {
        public LoginResponseModel AuthenticateLogin(LoginRequestModel request) 
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return new LoginResponseModel()
                {
                    UserName = "Not Found",
                    AccessToken = "Not Found",
                    Email = "Not Found"
                };
            } 
            var existingEmployee = dbContext.Employees
                .FirstOrDefault(u=>u.UserName == request.UserName && u.Password == request.Password);

            var existingManager = dbContext.Managers
                .FirstOrDefault(u => u.UserName == request.UserName && u.Password == request.Password);

            
            if(request.UserName == ConstantValues.ADMIN_DEFAULT_USERNAME && request.Password == ConstantValues.ADMIN_DEFAULT_PASSWORD)
            {
               var token = GenerateAccessToken(ConstantValues.ADMIN_DEFAULT_ID, ConstantValues.ADMIN_DEFAULT_FULLNAME, ConstantValues.ADMIN_DEFAULT_EMAIL, ConstantValues.ADMIN_DEFAULT_USERNAME, UMS.Enums.Roles.Admin.ToString());
                return token;
            }

            if (existingEmployee == null && existingManager == null) 
            {
                return new LoginResponseModel();
            }
            if (existingEmployee != null)
            {
                var token = GenerateAccessToken(existingEmployee.Id, existingEmployee.FullName,existingEmployee.Email, existingEmployee.UserName, UMS.Enums.Roles.Employee.ToString());
                return token;
            }
            else if (existingManager != null)
            {
                var token = GenerateAccessToken(existingManager.Id, existingManager.FullName,ConstantValues.MANAGER_DEFAULT_EMAIL, existingManager.UserName, UMS.Enums.Roles.Manager.ToString());
                return token;
            }
            else
            {
                return new LoginResponseModel()
                {
                    UserName = "Not Found",
                };
            }

        }
        public LoginResponseModel GenerateAccessToken(int Id,string FullName,string Email,string UserName,string Role)
        {
            var issuer = configuration["JWTConfig:Issuer"];
            var audience = configuration["JWTConfig:Audience"];
            var key = configuration["JWTConfig:Key"];
            if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("JWT configuration values are not set properly.");
            }
            var tokenValidityMins = configuration.GetValue<int>("JWTConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(tokenValidityMins);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                new Claim(JwtRegisteredClaimNames.Name, UserName),
                new Claim(JwtRegisteredClaimNames.Email,Email),
                new Claim(ClaimTypes.Role,Role)
                ]),
                Issuer = issuer,
                Audience = audience,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponseModel
            {
                RefreshToken = GenerateRefreshToken(Id,Role),
                UserName = UserName,
                AccessToken = accessToken,
                Expiration = tokenExpiryTimeStamp
            };
        }

        public string GenerateRefreshToken(int userId,string Role)
        {
            var tokenId = Guid.NewGuid().ToString();
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            int expiryDays = 5;
            var expiryDate = DateTime.Now.AddDays(expiryDays);
            if (Role == Enums.Roles.Employee.ToString())
            {
                var existingToken = dbContext.RefreshTokenEmployees.FirstOrDefault(t => t.EmployeeId == userId);
                if (existingToken != null)
                {
                    existingToken.RefreshUserToken = refreshToken;
                    existingToken.ExpiresAt = expiryDate;
                    dbContext.SaveChanges();
                    return refreshToken;
                }
            }
            else if (Role == Enums.Roles.Manager.ToString())
            {
                var existingToken = dbContext.RefreshTokenManagers.FirstOrDefault(t => t.ManagerId == userId);
                if (existingToken != null)
                {
                    existingToken.RefreshUserToken = refreshToken;
                    existingToken.ExpiresAt = expiryDate;
                    dbContext.SaveChanges();
                    return refreshToken;
                }
            }
            else if(Role == Enums.Roles.Admin.ToString())
            {
                return refreshToken;
            }
            else
            {

                throw new ArgumentException("Invalid role specified for refresh token generation.");
            }
            return string.Empty;
        }
    }
}
