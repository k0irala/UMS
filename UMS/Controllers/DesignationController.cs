using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Asp.Versioning;
using UMS.Models.Designation;
using UMS.Repositories;

namespace UMS.Controllers
{
    [ApiVersion(1)]
    [ApiVersion(2)]
    [Route("api/v{apiversion:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class DesignationController(DesignationRepository designation) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(AddDesignationModel model)
        {
                HttpStatusCode result = await designation.AddDesignation(model);
                if (result == HttpStatusCode.OK) return Ok("Designation added successfully.");
                else if (result == HttpStatusCode.NotFound) return Conflict("Designation already exists.");
                else return Conflict("One or more validation error Occured");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id cannot be 0");
            }
            var result = await designation.GetDesignationById(id);

            return result != null ? Ok(result) : BadRequest("Id Not Found");
        }

        [HttpGet]
        [MapToApiVersion(1)]
        public async Task<List<AddDesignationModel>> GetAllV1()
        {
            var result = await designation.GetAllDesignations();
            return result != null ? [.. result] : [];
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            HttpStatusCode result = await designation.DeleteDesignation(id);
            if (result == HttpStatusCode.OK) { return Ok("Deisgnation Deleted Successfully"); }
            else if (result == HttpStatusCode.NotFound) { return Conflict("Deisgnation Not Found"); }
            else return Conflict("Error Deleting Designation");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDesignationModel model)
        {
            HttpStatusCode result = await designation.UpdateDesignation(id, model);
            if (result == HttpStatusCode.OK) { return Ok("Designation Updated Successfully"); }
            else if (result == HttpStatusCode.Conflict) { return Conflict("Designation Name Already Exists"); }
            else if (result == HttpStatusCode.NotFound) { return Conflict("Designation Name Not Found"); }
            else { return Conflict("Error Updating Designation"); }

        }

    }
}
