using System.Diagnostics;
using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Models.Entities;
using UMS.Models.Manager;
using UMS.Repositories.AttendanceRepo;
using UMS.ResponseExamples;
using UMS.Services.Attendance;

namespace UMS.Controllers;

[ApiController]
[ApiVersion(1)]
[ApiVersion(2)]
[Route("api/v{apiversion:apiVersion}/[controller]")]

public class AttendanceController(IEmployeeAttendanceRepository employeeAttendanceRepository,ManagerAttendanceService managerAttendance) : ControllerBase
{
    [ProducesResponseType(400)]
    [SwaggerResponseExample(400, typeof(EmpAttendanceBadRequestExample))]
    [ProducesResponseType(typeof(EmpAttendanceResponseExample), 200)]
    [ProducesResponseType(400)]
    [SwaggerResponseExample(200, typeof(EmpAttendanceResponseExample))]
    // [SwaggerResponse(HttpStatusCode.BadRequest)]
    [HttpGet("employeeAttendance")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll()
    {
        var result =await employeeAttendanceRepository.GetAttendance();
        return Ok(result); 
    }

    [HttpGet("GetEmployeeAttendnace/{employeeId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllEmpAttendance(int employeeId)
    {
        var result = await employeeAttendanceRepository.GetEmployeeAttendance(employeeId);
        return Ok(result);
    }

    [HttpGet("GetAttendance")]
    [Authorize]
    public async Task<IActionResult> GetEmpAttendance()
    {
        HttpContext.Session.TryGetValue("Email", out var emailBytes);
        var email = emailBytes != null ? System.Text.Encoding.UTF8.GetString(emailBytes) : null;
        if (email == null) return BadRequest("No Attendance Found");
        var result = await employeeAttendanceRepository.GetAttendanceByEmp(email);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAttendance(AttendanceModel value,int employeeId)
    {
        var role = HttpContext.Session.GetString("Role");
        var result = await employeeAttendanceRepository.CreateEmployeeAttendance(value,employeeId, role);
        return result switch
        {
            HttpStatusCode.BadRequest => BadRequest("Invalid Attendance Data"),
            HttpStatusCode.Conflict => Conflict("Email Already Registered"),
            HttpStatusCode.OK => Ok("Attendance Registered Successfully"),
            _ => Conflict("The Attendance cannot be registered!")
        };
    }

    
    
}