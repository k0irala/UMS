using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Services;

namespace UMS.Controllers;
[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
public class EmployeeController(IEmployeeService empService) :ControllerBase
{
    [HttpPost]
    public IActionResult GetAlL(DataTableRequest request)
    {
        var emp = empService.GetAllEmployees(request);
        return Ok(emp);
    }
    
}