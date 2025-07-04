using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using UMS.Data;
using UMS.Models.Entities;
using UMS.Models.Manager;
using UMS.Repositories.ManagerManagement;

namespace UMS.Services
{
    public class ManagerService(ApplicationDbContext dbContext,IManagerRepository managerRepository)
    {
        public GetManagerByIdQueryResponse GetManagerById(int id)
        {
            var result = managerRepository.GetManagerById(id);
            return result.Id == 0 ? new GetManagerByIdQueryResponse() : result;
        }
        public List<GetManagerQueryResponse> GetAllManagers(DataTableRequest request)
        {
            var result = managerRepository.GetAllManagers(request);
            return result;
        }
        public (HttpStatusCode,bool) UpdateManager(int id,AddManager manager)
        {
            // Implementation to update a manager
            var result = managerRepository.UpdateManager(id, manager);
            return result switch
            {
                1 => (HttpStatusCode.OK, true),
                -1 => (HttpStatusCode.BadRequest, false),
                _ => (HttpStatusCode.InternalServerError, false),
            };
        }
        public (HttpStatusCode,bool) DeleteManager(int id)
        {
           var result = managerRepository.DeleteManager(id);
           return result switch
           {
               1 => (HttpStatusCode.OK, true),
               -1 => (HttpStatusCode.NotFound, false),
               0 => (HttpStatusCode.Conflict, false),
               _ => (HttpStatusCode.InternalServerError, false),
           };
        }
        public async Task<Manager> ManagerData(string userName, string password)
        {
            // Implementation to check if a manager exists
            return await dbContext.Managers
                .FirstOrDefaultAsync(m => m.UserName == userName && m.Password == password);
        }
        public Manager GetManagerByDesignation(int designationId)
        {
            // Implementation to get a manager by designation ID
            return dbContext.Managers
                .FirstOrDefault(m => m.DesignationId == designationId) ?? new Manager();
        }

        public Employee GetEmployeeByManager(int managerId)
        {
            return dbContext.Employees.FirstOrDefault(e => e.ManagerId == managerId);
        }

        public HttpStatusCode ChangeManagerEmail(int managerId,string email)
        {
            var existingManager = dbContext.Managers.SingleOrDefault(x=>x.Id == managerId);
            if(existingManager == null)
                return HttpStatusCode.NotFound;
            existingManager.Email = email;
            dbContext.Update(existingManager);
            dbContext.SaveChanges();
            return HttpStatusCode.OK;
        }

        public (HttpStatusCode, bool) CreateManager(AddManager manager)
        {
            manager.Email = manager.Email == "string" ? ConstantValues.MANAGER_DEFAULT_EMAIL : manager.Email;
            var result = managerRepository.AddManager(manager);
            return result switch
            {
                1 => (HttpStatusCode.OK, true),
                -1 => (HttpStatusCode.BadRequest, false),
                _ => (HttpStatusCode.InternalServerError, false),
            };

        }
    }
}