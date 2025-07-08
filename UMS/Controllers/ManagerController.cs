using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Models.Employee;
using UMS.Models.Manager;
using UMS.Services;

namespace UMS.Controllers;

[ApiController]
[ApiVersion(1)]
[ApiVersion(2)]
[Route("api/v{apiversion:apiVersion}/[controller]")]
[Authorize(Roles = "Admin")]
public class ManagerController(ManagerService managerService) :ControllerBase
{
    [HttpPost("getmanagers")]
    public IActionResult GetAll(DataTableRequest request)
    {
        var result = managerService.GetAllManagers(request);
        return Ok(result);
    }
    [HttpGet("getmanagerbyid")]
    public IActionResult GetManagerById(int id)
    {
        var result = managerService.GetManagerById(id);
        return Ok(result);
    }

    [HttpPut]
    public IActionResult Update(int id, AddManager manager)
    {
        var (statusCode,isSuccess) =  managerService.UpdateManager(id,manager);
        if (isSuccess) return StatusCode((int)statusCode,new ResponseModel() { StatusCode = statusCode, 
            IsSuccess = isSuccess, SuccessMessage = "Manager Updated Successfully" });
        return StatusCode((int)statusCode,new ResponseModel()
        {
            StatusCode = statusCode,
            IsSuccess = isSuccess,
            ErrorMessage = statusCode.ToString()
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var (statusCode,isSuccess) = managerService.DeleteManager(id);
        if (isSuccess) return StatusCode((int)statusCode,new ResponseModel(){StatusCode = statusCode,
            IsSuccess = isSuccess,
            SuccessMessage = "Manager Deleted Successfully"});
        return StatusCode((int)statusCode, new ResponseModel()
        {
            StatusCode = statusCode,
            IsSuccess = isSuccess,
            ErrorMessage = statusCode.ToString()
        });
    }
    [HttpPost]
    public IActionResult Create(AddManager manager)
    {
        var (statusCode, isSuccess) = managerService.CreateManager(manager);
        if (isSuccess)
            return StatusCode((int)statusCode,
                new ResponseModel()
                {
                    StatusCode = statusCode, IsSuccess = isSuccess, SuccessMessage = "Manager Created Successfully"
                });
        return StatusCode((int)statusCode, new ResponseModel()
        {
            StatusCode = statusCode,
            IsSuccess = isSuccess,
            ErrorMessage = statusCode.ToString()
        });
    }

}