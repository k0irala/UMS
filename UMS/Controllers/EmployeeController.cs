using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Models.Employee;
using UMS.Services;

namespace UMS.Controllers;
[ApiVersion(1)]
[ApiVersion(2)]
[Route("api/v{apiversion:apiVersion}/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")]
public class EmployeeController(EmployeeService empService) :ControllerBase
{
    [HttpPost("GetAllEmployees")]
    public IActionResult GetAlL(DataTableRequest request)
    {
        var emp = empService.GetAllEmployees(request);
        return Ok(emp);
    }

    [HttpGet("{id}")]
    public IActionResult GetEmployee(int id)
    {
        var emp = empService.GetById(id);
        return Ok(emp);
    }

    [HttpPut]
    public IActionResult UpdateEmployee(int id, UpdateEmployee employee)
    {
        var (statusCode,isSuccess) = empService.UpdateEmployee(id,employee);
        if (isSuccess)
        {
            return StatusCode((int)statusCode,new ResponseModel()
            {
                StatusCode = statusCode,
                IsSuccess = true,
                SuccessMessage = "Employee Updated Successfully"
            });
        }
        return StatusCode((int)statusCode,new ResponseModel()
        {
            StatusCode = statusCode,
            IsSuccess = isSuccess,
            ErrorMessage = statusCode.ToString()
        });
    }
    [HttpDelete("{id}")]
    public IActionResult DeleteEmployee(int id)
    {
        var (statusCode,isSuccess) = empService.DeleteEmployee(id);
        if (isSuccess)
            return StatusCode((int)statusCode,
                new ResponseModel()
                {
                    StatusCode = statusCode, IsSuccess = isSuccess, SuccessMessage = "Employee Deleted Successfully"
                });
        return StatusCode((int)statusCode, new ResponseModel()
        {
            StatusCode = statusCode,
            IsSuccess = isSuccess,
            ErrorMessage = statusCode.ToString()
        });
    }

    [HttpPost]
    public IActionResult AddEmployee(AddEmployee employee)
    {
        var (statusCode,isSuccess) = empService.CreateEmployee(employee);
        if (isSuccess) return StatusCode((int)statusCode,new ResponseModel(){StatusCode = statusCode,IsSuccess = isSuccess,SuccessMessage = "Employee Created Successfully"});
        return StatusCode((int)statusCode, new ResponseModel()
        {
            StatusCode = statusCode,
            IsSuccess = isSuccess,
            ErrorMessage = statusCode.ToString()
        });
    }
    
}