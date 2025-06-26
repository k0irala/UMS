using System.Net;
using UMS.Models;
using UMS.Models.Employee;

namespace UMS.Repositories
{
    public interface IAccountRepository
    {
        (bool IsValid,LoginResponseModel? token) Login(LoginRequestModel requestModel);
        LoginResponseModel RefreshToken(string token, string refreshToken);
        bool Logout(string token);
        HttpStatusCode UserRegister(AddEmployee requestModel);
        HttpStatusCode ManagerRegister(ManagerRegisterModel requestModel);
        public string GenerateNewOtp();
        public Task SendOtpMail(LoginRequestModel model,bool isForgotPassword);
        public string GenerateEmailBody(string userName,string otp);

    }
}
