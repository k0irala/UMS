using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.Results;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Repositories;
using UMS.Services;

namespace UMS.Controllers
{
    [ApiVersion(1)]
    [ApiVersion(2)]
    [Route("api/v{apiversion:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController(IAccountRepository accRepository, JWTService jWtService,IValidator<AddEmployee> empValidator,ManagerService managerService,BlackListTokenService blackList) : ControllerBase
    {
        [HttpPost("EmployeeRegister")]
        [AllowAnonymous]
        public IActionResult Register(AddEmployee emp)
        {
            var validate = empValidator.Validate(emp);
            if (!validate.IsValid) return BadRequest(validate);
            var result = accRepository.UserRegister(emp);

            return result switch
            {
                HttpStatusCode.OK => Ok("Employee Registered Successfully"),
                HttpStatusCode.Conflict => Conflict("Email Already Registered"),
                HttpStatusCode.BadRequest => BadRequest("Invalid Employee Data"),
                HttpStatusCode.NotFound=> NotFound("Manager Not Found in the given designation"),
                _ => Conflict("Error in Registering Employee to the database")
            };
        }

        [HttpPost("ManagerRegister")]
        [AllowAnonymous]
        public IActionResult Register(ManagerRegisterModel managerModel)
        {
            var result = accRepository.ManagerRegister(managerModel);

            if (result == HttpStatusCode.OK) { return Ok("Manager Registered Successfully"); }

            return Conflict(result == HttpStatusCode.Conflict ? "Duplicate Manager Exists" : "Error in Registering Manager to the database");
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            HttpContext.Session.SetString("UserName", model.UserName);
            const bool isForgotPassword = false;
            var (isValidUser, token) = await accRepository.Login(model);
            if (!isValidUser)
            {
                return Unauthorized("Invalid username or password.");
            }

            if (token != null)
            {
                var role = jWtService.UserRole(token);
                HttpContext.Session.SetString("Role", role);
                return Ok(token);
            }
            
            var result = await accRepository.SendOtpMail(model, isForgotPassword);
            return Ok(result);
            
            
          
        }
        [HttpPost("VerifyOtp")]
        [AllowAnonymous]
        public IActionResult VerifyOtp(string OTP)
        {
            var response = jWtService.VerifyOtp(OTP, false);
            if (response == new LoginResponseModel())
                return Conflict("OTP has expired");
            HttpContext.Session.SetString("Email", response.Email);
            HttpContext.Session.SetInt32("UserId",response.UserId);
            return Ok(response);
        }
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            HttpContext.Session.SetString("Email", email);
            const bool isForgotPassword = true;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email are required.");
            }

            await accRepository.SendOtpMail(new LoginRequestModel { UserName = email }, isForgotPassword);

            return Ok("Otp has been sent to your email. Please verify to continue.");
        }
        [HttpPost("NewPassword")]
        [Authorize]
        public IActionResult NewPassword(NewPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.NewPassword) || model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("New password and confirm password do not match.");
            }

            HttpContext.Session.TryGetValue("Email", out var emailBytes);
            var email = emailBytes != null ? System.Text.Encoding.UTF8.GetString(emailBytes) : null;
            var response = jWtService.PasswordReset(model.NewPassword, email);
            if (response != null)
            {
                return Ok("Password reset successfully.");
            }
            else
            {
                return BadRequest("Invalid OTP or password reset failed.");
            }
        }

        [HttpPost("ManagerEmail")]
        [Authorize(Roles="Admin")]
        public IActionResult ChangeEmail(int managerId,string email)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(403, "You do not have permission to change manager emails.");
            var manager = managerService.ChangeManagerEmail(managerId,email);

            if (manager == HttpStatusCode.OK)
                return Ok("The email of the manager has been changed!!");
            return BadRequest(manager);
        }

        [HttpPost("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Conflict("Could not Log Out!!");
            var token = authHeader["Bearer ".Length..].Trim();
            var result = blackList.SaveBlackListToken(new BlackListToken(){Token=token,ExpiresAt = DateTime.Now.AddHours(3)});
            return Ok(result + "Logged Out!");
        }
    }
}
