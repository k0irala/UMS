using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UMS.Models.Designation;
using UMS.Repositories;

namespace UMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class DesignationController(DesignationRepository designation) : ControllerBase
    {
        [HttpPost]
        public IActionResult Create(AddDesignationModel model)
        {
            try
            {
                HttpStatusCode result = designation.AddDesignation(model);
                if (result == HttpStatusCode.OK) return Ok("Designation added successfully.");
                else if (result == HttpStatusCode.NotFound) return Conflict("Designation already exists.");
                else return Conflict("One or more validation error Occured");
            }
            catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}"); }
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id cannot be 0");
            }
            var result = designation.GetDesignationById(id);

            return result != null ? Ok(result) : BadRequest("Id Not Found");
        }

        [HttpGet]
        public List<AddDesignationModel> GetAll()
        {
            var result = designation.GetAllDesignations();
            return result != null ? [.. result] : [];
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            HttpStatusCode result = designation.DeleteDesignation(id);
            if (result == HttpStatusCode.OK) { return Ok("Deisgnation Deleted Successfully"); }
            else if (result == HttpStatusCode.NotFound) { return Conflict("Deisgnation Not Found"); }
            else return Conflict("Error Deleting Designation");
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateDesignationModel model)
        {
            HttpStatusCode result = designation.UpdateDesignation(id, model);
            if (result == HttpStatusCode.OK) { return Ok("Designation Updated Successfully"); }
            else if (result == HttpStatusCode.Conflict) { return Conflict("Designation Name Already Exists"); }
            else if (result == HttpStatusCode.NotFound) { return Conflict("Designation Name Not Found"); }
            else { return Conflict("Error Updating Designation"); }

        }

    }
}
