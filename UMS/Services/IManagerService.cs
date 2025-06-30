using System.Net;
using UMS.Models.Entities;

namespace UMS.Services
{
    public interface IManagerService
    {
        Task<Manager> GetManagetById(int id);
        Task<IEnumerable<Manager>> GetAllManagers();
        Task<HttpStatusCode> UpdateManager(Manager manager);
        Task<HttpStatusCode> DeleteManager(int id);
        Task<Manager> ManagerData(string userName, string password);

        Manager GetManagerByDesignation(int designationId);
        Employee GetEmployeeByManager(int managerId);
        HttpStatusCode ChangeManagerEmail(int managerId,string email);
    }
}