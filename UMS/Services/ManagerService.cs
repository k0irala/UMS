using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using UMS.Data;
using UMS.Models.Entities;

namespace UMS.Services
{
    public class ManagerService(ApplicationDbContext dbContext) : IManagerService
    {
        public async Task<Manager> GetManagetById(int id)
        {
            // Implementation to get a manager by ID
            var existingManager = await dbContext.Managers.FindAsync(id);
            return existingManager ?? new Manager();
        }
        public async Task<IEnumerable<Manager>> GetAllManagers()
        {
            // Implementation to get all managers
            var allManagers = await dbContext.Managers.ToListAsync();
            return allManagers;
        }
        public Task<HttpStatusCode> UpdateManager(Manager manager)
        {
            // Implementation to update a manager
            throw new NotImplementedException();
        }
        public Task<HttpStatusCode> DeleteManager(int id)
        {
            // Implementation to delete a manager
            throw new NotImplementedException();
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
    }
}