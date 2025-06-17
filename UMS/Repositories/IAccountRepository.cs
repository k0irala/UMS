using System.Net;
using UMS.Models;

namespace UMS.Repositories
{
    public interface IAccountRepository
    {
        LoginResponseModel Login(LoginRequestModel requestModel);
        LoginResponseModel RefreshToken(string token, string refreshToken);
        bool Logout(string token);
        HttpStatusCode UserRegister(UserRegisterModel requestModel);
        HttpStatusCode ManagerRegister(ManagerRegisterModel requestModel);
        public string GenerateNewOtp();
        public Task SendOtpMail(string email, string otp,string userName);
        public string GenerateEmailBody(string userName,string otp);

    }
}
