using Dapper;
using FluentValidation;
using FluentValidation.Results;
using System.Net;
using UMS.Enums;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Services;

namespace UMS.Repositories
{
    public class AccountRepository(
        IValidator<ManagerRegisterModel> managerValidator,
        IDapperRepository repository,
        JWTService jwtService,
        IEmailService emailService,
        IEmployeeService empService,
        IManagerService managerService) : IAccountRepository
    {
        public (bool IsValid, LoginResponseModel? token) Login(LoginRequestModel request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and Password cannot be null or empty.");

            var (isValid, token) = jwtService.ValidateLoginCredentials(request);
            if (!isValid)
                throw new UnauthorizedAccessException("Invalid username or password.");

            return (isValid, token);
        }

        public bool Logout(string token)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode ManagerRegister(ManagerRegisterModel request)
        {
            var validation = managerValidator.Validate(request);
            if (!validation.IsValid)
                return HttpStatusCode.BadRequest;

            var parameters = new DynamicParameters();
            parameters.Add("@FullName", request.FullName);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@Password", request.Password);
            parameters.Add("@Email", ConstantValues.MANAGER_DEFAULT_EMAIL);
            parameters.Add("@DesignationId", request.DesignationId);
            parameters.Add("@RoleId", Roles.Manager);
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            repository.Execute(StoredProcedures.MANAGER_REGISTER, parameters);
            int result = parameters.Get<int>("@Result");

            return result switch
            {
                1 => HttpStatusCode.OK,
                -1 => HttpStatusCode.Conflict,
                _ => throw new Exception("An error occurred while registering the manager.")
            };
        }

        public LoginResponseModel RefreshToken(string token, string refreshToken)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode UserRegister(AddEmployee request)
        {
            var manager = managerService.GetManagerByDesignation(request.DesignationId)
                ?? throw new ArgumentException("Manager not found for the given designation.");

            var parameters = new DynamicParameters();
            parameters.Add("@FullName", request.FullName);
            parameters.Add("@UserName", request.UserName);
            parameters.Add("@Code", GenerateRandomEmpCode());
            parameters.Add("@Password", request.Password);
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

        public string ForgetPassword() => string.Empty;

        public string GenerateNewOtp()
        {
            return new Random().Next(100000, 999999).ToString(); // 6-digit OTP
        }

        public async Task SendOtpMail(LoginRequestModel model)
        {
            if (model.UserName == ConstantValues.ADMIN_DEFAULT_USERNAME &&
                model.Password == ConstantValues.ADMIN_DEFAULT_PASSWORD)
                return; // Skip OTP for admin

            string? email = null;
            var employee = await empService.EmployeeData(model.UserName, model.Password);
            if (employee != null)
            {
                email = employee.Email;
            }
            else
            {
                email = ConstantValues.MANAGER_DEFAULT_EMAIL;
            }

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Invalid username or password.");

            var otp = GenerateNewOtp();

            var parameters = new DynamicParameters();
            parameters.Add("@Otp", otp);
            parameters.Add("@Email", email);
            parameters.Add("@CreatedAt", DateTime.Now);
            parameters.Add("@ExpiresAt", DateTime.Now.AddMinutes(5));
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            repository.Execute(StoredProcedures.OTP_INSERT, parameters);

            var mailRequest = new MailRequestModel
            {
                Email = email,
                Subject = "The OTP for Logging in is:",
                Body = GenerateEmailBody(model.UserName, otp)
            };

            await emailService.SendEmail(mailRequest);
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
