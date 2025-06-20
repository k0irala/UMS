using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Models.Entities;
using UMS.Repositories;
using UMS.Services;

namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController(IAccountRepository accRepository,JWTService jWTService) : ControllerBase
    {
        [HttpPost("EmployeeRegister")]
        public IActionResult Register(AddEmployee emp)
        {
            var result = accRepository.UserRegister(emp);

            if (result == HttpStatusCode.OK) { return Ok("Employee Registered Successfully"); }
            else if (result == HttpStatusCode.Conflict) { return Conflict("Duplicate Employee Exists"); }
            else if (result == HttpStatusCode.BadRequest) { return BadRequest("Invalid Employee Data"); }
            else return Conflict("Error in Registering Employee to the database");
        }

        [HttpPost("ManagerRegister")]
        public IActionResult Register(ManagerRegisterModel managerModel)
        {
            var result =accRepository.ManagerRegister(managerModel);

            if (result == HttpStatusCode.OK) { return Ok("Manager Registered Successfully"); }
            else if (result == HttpStatusCode.Conflict) { return Conflict("Duplicate Manager Exists"); }
            else return Conflict("Error in Registering Manager to the database");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestModel model) 
        {
            HttpContext.Session.SetString("UserName", model.UserName);
            var (isValidUser,token) = accRepository.Login(model);
            if (!isValidUser)
            {
                return Unauthorized("Invalid username or password.");
            }
            if (token != null)
                return Ok(token);

            await accRepository.SendOtpMail(model);   
            return Ok("OTP sent to your email. Please verify to continue.");
        }   
        [HttpPost("VerifyOtp")]
        public IActionResult VerifyOtp(string OTP)
        {
            var email = HttpContext.Session.GetString("UserEmail")!=null ? HttpContext.Session.GetString("UserEmail") : ConstantValues.MANAGER_DEFAULT_EMAIL;
            var response = jWTService.VerifyOtp(OTP,email);
            return Ok(response);
        }
    }
}
