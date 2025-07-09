using System.Net;
using UMS.Models.Designation;

namespace UMS.Repositories
{
    public interface IDesignationRepository
    {
        Task<HttpStatusCode> AddDesignation(AddDesignationModel designationModel);
        Task<HttpStatusCode> UpdateDesignation(int id, UpdateDesignationModel designationModel);
        Task<HttpStatusCode> DeleteDesignation(int id);
        Task<AddDesignationModel> GetDesignationById(int id);
        Task<IEnumerable<AddDesignationModel>> GetAllDesignations();

    }
}
