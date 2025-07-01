using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Models;
using UMS.Models.Entities;
using UMS.Repositories.AttendanceRepo;

namespace UMS.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AttendanceController(IEmployeeAttendanceRepository employeeAttendanceRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult GetAll()
    {
        var result = employeeAttendanceRepository.GetAttendance();
        return Ok(result); 
    }

    [HttpGet("GetEmployeeAttendnace/{employeeId}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult GetAllEmpAttendance(int employeeId)
    {
        var result = employeeAttendanceRepository.GetEmployeeAttendance(employeeId);
        return Ok(result);
    }

    [HttpGet("GetAttendance")]
    [Authorize]
    public IActionResult GetEmpAttendance()
    {
        HttpContext.Session.TryGetValue("Email", out var emailBytes);
        var email = emailBytes != null ? System.Text.Encoding.UTF8.GetString(emailBytes) : null;
        if (email == null) return BadRequest("No Attendance Found");
        var result = employeeAttendanceRepository.GetAttendanceByEmp(email);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateAttendance(AttendanceModel value,int employeeId)
    {
        var role = HttpContext.Session.GetString("Role");
        if (role == null) return BadRequest("No Role Found");
        employeeAttendanceRepository.CreateEmployeeAttendance(value,employeeId, role);
        return Ok();
    }
}