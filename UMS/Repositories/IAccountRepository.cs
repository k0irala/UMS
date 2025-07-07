using System.Net;
using UMS.Models;
using UMS.Models.Employee;

namespace UMS.Repositories
{
    public interface IAccountRepository
    {
        Task<(bool IsValid,LoginResponseModel? token)> Login(LoginRequestModel requestModel);
        LoginResponseModel RefreshToken(string token, string refreshToken);
        HttpStatusCode UserRegister(AddEmployee requestModel);
        HttpStatusCode ManagerRegister(ManagerRegisterModel requestModel);
        public string GenerateNewOtp();
        public Task<OTPResultModel> SendOtpMail(LoginRequestModel model,bool isForgotPassword);
        public string GenerateEmailBody(string userName,string otp);

    }
}
