using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Models.Manager;
using UMS.Services;
using UMS.Services.Attendance;

namespace UMS.Controllers;

[ApiController]
[ApiVersion(1)]
[ApiVersion(2)]
[Route("api/v{apiversion:apiVersion}/[controller]")]
public class ManagerAttendanceController(ManagerAttendanceService managerAttendance) :ControllerBase
{
    [HttpGet("manager")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion(2)]
    public IActionResult GetAllAttendance()
    {
        var result = managerAttendance.GetAllManagerAttendance();
        return Ok(result); 
    }

    [HttpGet("GetManagerAttendnace/{managerId}")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion(2)]
    public IActionResult GetManagerAttendanceById(int managerId)
    {
        var result = managerAttendance.GetManagerAttendanceByID(managerId);
        return Ok(result);
    }

    [HttpGet("getmyattendance")]
    [Authorize]
    [MapToApiVersion(2)]
    public IActionResult GetManagerAttendance()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized("User is not logged in.");
        }
        var result = managerAttendance.GetMyAttendance(userId.Value);
        return Ok(result);
    }

    [HttpPost("addmanagerattendance")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion(2)]
    public IActionResult CreateManagerAttendance(ManagerAttendanceModel value)
    {
        var result = managerAttendance.AddManagerAttendance(value);
        return result switch
        {
            HttpStatusCode.BadRequest => BadRequest("Invalid Attendance Data"),
            HttpStatusCode.Conflict => Conflict("Email Already Registered"),
            HttpStatusCode.OK => Ok("Attendance Registered Successfully"),
            _ => Conflict("The Attendance cannot be registered!")
        };
    }
}