using Dapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Net;
using System.Net.Http;
using UMS.Enums;
using UMS.Migrations;
using UMS.Models;
using UMS.Services;
using static System.Net.WebRequestMethods;

namespace UMS.Repositories
{
    public class AccountRepository(IValidator<ManagerRegisterModel> managerValidator,IDapperRepository repository,JWTService jwtService,IEmailService emailService,IHttpContextAccessor httpContext): IAccountRepository
    {
        private readonly ISession _session = httpContext.HttpContext.Session;
        public LoginResponseModel Login(LoginRequestModel requestModel)
        {
            ArgumentNullException.ThrowIfNull(requestModel);
            if (string.IsNullOrEmpty(requestModel.UserName) || string.IsNullOrEmpty(requestModel.Password))
            {
                return new LoginResponseModel()
                {
                    UserName = "Not Found",
                    AccessToken = "Not Found",
                    Email = "Not Found"
                };
            }
            var token = jwtService.AuthenticateLogin(requestModel);
            return token;
        }

        public bool Logout(string token)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode ManagerRegister(ManagerRegisterModel requestModel)
        { 
            ValidationResult validationResult = managerValidator.Validate(requestModel);
            if (!validationResult.IsValid) { return HttpStatusCode.BadRequest; }

            DynamicParameters parameters = new();
            parameters.Add("@FullName", requestModel.FullName);
            parameters.Add("@UserName", requestModel.UserName);
            parameters.Add("@Password", requestModel.Password);
            parameters.Add("@Email", ConstantValues.MANAGER_DEFAULT_EMAIL);
            parameters.Add("@DesignationId", requestModel.DesignationId);
            parameters.Add("@RoleId", Roles.Manager);
            parameters.Add("@Result", dbType:System.Data.DbType.Int32,direction:System.Data.ParameterDirection.Output);

            repository.Execute(StoredProcedures.MANAGER_REGISTER, parameters);

            int result = parameters.Get<int>("@Result");

            if (result == 1) return HttpStatusCode.OK;
            if (result == -1) return HttpStatusCode.Conflict;
            else throw new Exception("An error occurred while adding the designation.");
        }

        public LoginResponseModel RefreshToken(string token, string refreshToken)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode UserRegister(UserRegisterModel requestModel)
        {
            DynamicParameters parameters = new();
            parameters.Add("@FullName", requestModel.FullName);
            parameters.Add("@UserName", requestModel.UserName);
            parameters.Add("@Code", requestModel.Code);
            parameters.Add("@Password", requestModel.Password);
            parameters.Add("@Email", requestModel.Email);
            parameters.Add("@DesignationId", requestModel.DesignationId);
            //parameters.Add("@ManagerId", requestModel.ManagerId);
            //return true;
            return HttpStatusCode.OK;

        }
        public string ForgetPassword() 
        {
            return "";
        }

        public string GenerateNewOtp()
        {
            Random randomValue = new Random();
            int randomInteger = randomValue.Next(100000, 999999); // Generates a 6-digit OTP
            return randomInteger.ToString();
        }

        public async Task SendOtpMail(string email, string otp,string userName)
        {
            _session.SetString("Email", email);
            _session.SetString("UserName",userName);
            _session.SetString("OTP", otp);
            var mailrequest = new MailRequestModel();
            mailrequest.Email = email;
            mailrequest.Subject = "The OTP for Logging in is:";
            mailrequest.Body = GenerateEmailBody(userName, otp);
            await emailService.SendEmail(mailrequest);
        }

        public string GenerateEmailBody(string userName, string otp)
        {
            string emailBody = string.Empty;
            emailBody = "<div>";
            emailBody += "<h1>Hi " + userName + " Thank you for registering";
            emailBody += "Your OTP is :" + otp;
            return emailBody;
        }
    }
}
