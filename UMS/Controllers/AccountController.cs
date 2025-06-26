using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Repositories;
using UMS.Services;

namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccountRepository accRepository, JWTService jWTService) : ControllerBase
    {
        [HttpPost("EmployeeRegister")]
        [AllowAnonymous]
        public IActionResult Register(AddEmployee emp)
        {
            var result = accRepository.UserRegister(emp);

            if (result == HttpStatusCode.OK) { return Ok("Employee Registered Successfully"); }
            else if (result == HttpStatusCode.Conflict) { return Conflict("Duplicate Employee Exists"); }
            else if (result == HttpStatusCode.BadRequest) { return BadRequest("Invalid Employee Data"); }
            else return Conflict("Error in Registering Employee to the database");
        }

        [HttpPost("ManagerRegister")]
        [AllowAnonymous]
        public IActionResult Register(ManagerRegisterModel managerModel)
        {
            var result = accRepository.ManagerRegister(managerModel);

            if (result == HttpStatusCode.OK) { return Ok("Manager Registered Successfully"); }
            else if (result == HttpStatusCode.Conflict) { return Conflict("Duplicate Manager Exists"); }
            else return Conflict("Error in Registering Manager to the database");
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            HttpContext.Session.SetString("UserName", model.UserName);
            var isForgotPassword = false;
            var (isValidUser, token) = accRepository.Login(model);
            if (!isValidUser)
            {
                return Unauthorized("Invalid username or password.");
            }
            if (token != null)
                return Ok(token);

            await accRepository.SendOtpMail(model, isForgotPassword);
            return Ok("OTP sent to your email. Please verify to continue.");
        }
        [HttpPost("VerifyOtp")]
        [AllowAnonymous]
        public IActionResult VerifyOtp(string OTP)
        {
            var response = jWTService.VerifyOtp(OTP, false);
            HttpContext.Session.SetString("Email", response.Email);
            return Ok(response);
        }
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            HttpContext.Session.SetString("Email", email);
            var isForgorPassword = true;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email are required.");
            }

            await accRepository.SendOtpMail(new LoginRequestModel { UserName = email }, isForgorPassword);

            return Ok("Otp has been sent to your email. Please verify to continue.");
        }
        [HttpPost("NewPassword")]
        [Authorize]
        public IActionResult NewPassword(NewPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.NewPassword) && model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("New password and confirm password do not match.");
            }

            HttpContext.Session.TryGetValue("Email", out var emailBytes);
            var email = emailBytes != null ? System.Text.Encoding.UTF8.GetString(emailBytes) : null;
            var response = jWTService.PasswordReset(model.NewPassword, email);
            if (response != null)
            {
                return Ok("Password reset successfully.");
            }
            else
            {
                return BadRequest("Invalid OTP or password reset failed.");
            }
        }
        
    }
}
