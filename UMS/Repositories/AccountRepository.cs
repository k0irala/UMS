using Dapper;
using FluentValidation;
using FluentValidation.Results;
using System.Net;
using System.Text;
using UMS.Encryption;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Models.Entities;
using UMS.Services;
using Roles = UMS.Enums.Roles;

namespace UMS.Repositories
{
    public class AccountRepository(
        IValidator<ManagerRegisterModel> managerValidator,
        IDapperRepository repository,
        JWTService jwtService,
        IEmailService emailService,
        EmployeeService empService,
        ManagerService managerService,
        AesEncryption aesEncryption) : IAccountRepository
    {
        public async Task<(bool IsValid, LoginResponseModel? token)> Login(LoginRequestModel request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and Password cannot be null or empty.");

            var (isValid, token) = await  jwtService.ValidateLoginCredentials(request);
            if (!isValid)
                throw new UnauthorizedAccessException("Invalid username or password.");

            return (isValid, token);
        }

        public HttpStatusCode ManagerRegister(ManagerRegisterModel request)
        {
            var validation = managerValidator.Validate(request);
            if (!validation.IsValid)
                return HttpStatusCode.BadRequest;

            var encryptedPass = aesEncryption.EncryptString(request.Password);
            var parameters = new DynamicParameters();
            parameters.Add("@FullName", request.FullName);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@Password",encryptedPass);
            parameters.Add("@Email", ConstantValues.MANAGER_DEFAULT_EMAIL);
            parameters.Add("@DesignationId", request.DesignationId);
            parameters.Add("@RoleId", Roles.Manager);
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            repository.Execute(StoredProcedures.MANAGER_REGISTER, parameters);
            var result = parameters.Get<int>("@Result");

            return result switch
            {
                1 => HttpStatusCode.OK,
                -1 => HttpStatusCode.Conflict,
                _ => throw new Exception("An error occurred while registering the manager.")
            };
        }

        public LoginResponseModel RefreshToken(string token, string refreshToken)
        {
            return new LoginResponseModel();
        }

        public HttpStatusCode UserRegister(AddEmployee request)
        {
            var manager = managerService.GetManagerByDesignation(request.DesignationId);
            if (manager == null || string.IsNullOrEmpty(manager.UserName))
            {
                return HttpStatusCode.NotFound;
            }
            var encryptedPass = aesEncryption.EncryptString(request.Password);
            var parameters = new DynamicParameters();
            parameters.Add("@FullName", request.FullName);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@Code", GenerateRandomEmpCode());
            parameters.Add("@Password", encryptedPass);
            parameters.Add("@Email", request.Email);
            parameters.Add("@DesignationId", request.DesignationId);
            parameters.Add("@ManagerId", manager.Id);
            parameters.Add("@RoleId", Roles.Employee);
            parameters.Add("@Status", "Active");
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            repository.Execute(StoredProcedures.USER_REGISTER, parameters);
            int result = parameters.Get<int>("@Result");

            return result switch
            {
                1 => HttpStatusCode.OK,
                -1 => HttpStatusCode.Conflict,
                -2 => HttpStatusCode.BadRequest,
                _ => throw new Exception("An error occurred while registering the employee.")
            };
        }
        public string GenerateNewOtp()
        {
            return new Random().Next(100000, 999999).ToString(); // 6-digit OTP
        }

        public async Task<OTPResultModel> SendOtpMail(LoginRequestModel model,bool isForgotPassword)
        {
            var otp = GenerateNewOtp();
            if (isForgotPassword)
            {
                var emp = empService.GetEmployeeByEmail(model.UserName);
                if (emp == null)
                    throw new ArgumentException("Invalid email address.");
                model.UserName = emp.UserName;

                var param = new DynamicParameters();
                param.Add("@Otp", otp);
                param.Add("@Email",emp.Email );
                param.Add("@CreatedAt", DateTime.Now);
                param.Add("@ExpiresAt", DateTime.Now.AddMinutes(5));
                param.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                repository.Execute(StoredProcedures.OTP_INSERT, param);
                
                var result = param.Get<int>("@Result");
                if (result == -1)
                {
                    return new OTPResultModel()
                    {
                        Success = false,
                        ErrorMessage = "OTP already exists resend after 5 minutes."
                    };
                }

                var requestMail = new MailRequestModel
                {
                    Email = emp.Email,
                    Subject = "The OTP for Logging in is:",
                    Body = GenerateEmailBody(model.UserName, otp)
                };

                await emailService.SendEmail(requestMail);
                return new OTPResultModel()
                {
                    Success = true,
                    ErrorMessage = ""
                };

            }
            if (model.UserName == ConstantValues.ADMIN_DEFAULT_USERNAME &&
                model.Password == ConstantValues.ADMIN_DEFAULT_PASSWORD)
                return new OTPResultModel(){Success = true,ErrorMessage = ""};

            string? email = null;
            var employee = await empService.EmployeeData(model.UserName);
            email = employee.Email;

            if (string.IsNullOrWhiteSpace(email))
                return new OTPResultModel(){Success = false,ErrorMessage = "Invalid email address."};
            
            var parameters = new DynamicParameters();
            parameters.Add("@Otp", otp);
            parameters.Add("@Email", email);
            parameters.Add("@CreatedAt", DateTime.Now);
            parameters.Add("@ExpiresAt", DateTime.Now.AddMinutes(5));
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            repository.Execute(StoredProcedures.OTP_INSERT, parameters);
            
            var res = parameters.Get<int>("@Result");
            if (res == -1)
            {
                return new OTPResultModel()
                {
                    Success = false,
                    ErrorMessage = "OTP already exists resend after 5 minutes."
                };
            }
            var mailRequest = new MailRequestModel
            {
                Email = email,
                Subject = "The OTP for Logging in is:",
                Body = GenerateEmailBody(model.UserName, otp)
            };

            await emailService.SendEmail(mailRequest);
            return new OTPResultModel() { Success = true, ErrorMessage = "", SuccessMessage = "OTP Sent to your mail"};
        }

        public string GenerateEmailBody(string userName, string otp)
        {
            return $"""
                <div>
                    <h1>Hi {userName}, thank you for registering.</h1>
                    <p>Your OTP is: <strong>{otp}</strong></p>
                </div>
            """;
        }

        public string GenerateRandomEmpCode()
        {
            return "EMP" + new Random().Next(1000, 9999);
        }
    }
}
