using System.Net;
using UMS.Models.Designation;

namespace UMS.Repositories
{
    public interface IDesignationRepository
    {
        HttpStatusCode AddDesignation(AddDesignationModel designationModel);
        HttpStatusCode UpdateDesignation(int id, UpdateDesignationModel designationModel);
        HttpStatusCode DeleteDesignation(int id);
        AddDesignationModel GetDesignationById(int id);
        IEnumerable<AddDesignationModel> GetAllDesignations();

    }
}
