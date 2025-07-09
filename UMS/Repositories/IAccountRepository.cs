using System.Net;
using UMS.Models;
using UMS.Models.Employee;

namespace UMS.Repositories
{
    public interface IAccountRepository
    {
        Task<(bool IsValid,LoginResponseModel? token)> Login(LoginRequestModel requestModel);
        Task<HttpStatusCode> UserRegister(AddEmployee requestModel);
        Task<HttpStatusCode> ManagerRegister(ManagerRegisterModel requestModel);
        public string GenerateNewOtp();
        public Task<OTPResultModel> SendOtpMail(LoginRequestModel model,bool isForgotPassword);
        public string GenerateEmailBody(string userName,string otp);

    }
}
