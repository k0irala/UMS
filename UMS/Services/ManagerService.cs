using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UMS.Data;
using UMS.Models.Entities;
using UMS.Models.Manager;
using UMS.Repositories.ManagerManagement;

namespace UMS.Services
{
    public class ManagerService(ApplicationDbContext dbContext,IManagerRepository managerRepository,IDistributedCache cache)
        
    {
        private readonly TimeSpan cacheExpiration = TimeSpan.FromHours(1);
        public async Task<GetManagerByIdQueryResponse> GetManagerById(int id)
        {
            var cacheKey = $"Manager_{id}";
            var cacheItem = cache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheItem))
            {
                return JsonConvert.DeserializeObject<GetManagerByIdQueryResponse>(cacheItem);
            }
            
            var result = await managerRepository.GetManagerById(id);
            cache.SetString(cacheKey,JsonConvert.SerializeObject(result),new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow = cacheExpiration,SlidingExpiration = TimeSpan.FromMinutes(2)});
            return result.Id == 0 ? new GetManagerByIdQueryResponse() : result;
        }
        public async Task<List<GetManagerQueryResponse>> GetAllManagers(DataTableRequest request)
        {
            var cacheKey = $"Manager_{request.Skip}_{request.Take}_{request.OrderColumn}_{request.OrderDirection}";
            var cachedItem = cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cachedItem))
            {
                return JsonConvert.DeserializeObject<List<GetManagerQueryResponse>>(cachedItem);
            }
            var result = await managerRepository.GetAllManagers(request);
            cache.SetString(cacheKey, JsonConvert.SerializeObject(result),new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow = cacheExpiration,SlidingExpiration = TimeSpan.FromMinutes(2)});
            return result;
        }
        public async Task<(HttpStatusCode,bool)> UpdateManager(int id,AddManager manager)
        {
            // Implementation to update a manager
            cache.Remove($"UMS_Manager_{id}");
            var result = await managerRepository.UpdateManager(id, manager);
            if (result == 1) cache.Remove($"UMS_Manager_{id}");
            return result switch
            {
                1 => (HttpStatusCode.OK, true),
                -1 => (HttpStatusCode.BadRequest, false),
                _ => (HttpStatusCode.InternalServerError, false),
            };
        }
        public async Task<(HttpStatusCode,bool)> DeleteManager(int id)
        { 
            cache.Remove($"UMS_Manager_{id}");
           var result = await managerRepository.DeleteManager(id);
           return result switch
           {
               1 => (HttpStatusCode.OK, true),
               -1 => (HttpStatusCode.NotFound, false),
               0 => (HttpStatusCode.Conflict, false),
               _ => (HttpStatusCode.InternalServerError, false),
           };
        }
        public async Task<Manager> GetManagerByDesignation(int designationId)
        {
            // Implementation to get a manager by designation ID
            return await dbContext.Managers
                .FirstOrDefaultAsync(m => m.DesignationId == designationId) ?? new Manager();
        }

        public async Task<Employee> GetEmployeeByManager(int managerId)
        {
            return await dbContext.Employees.FirstOrDefaultAsync(e => e.ManagerId == managerId);
        }

        public async Task<HttpStatusCode> ChangeManagerEmail(int managerId,string email)
        {
            var existingManager = await dbContext.Managers.SingleOrDefaultAsync(x=>x.Id == managerId);
            if(existingManager == null)
                return HttpStatusCode.NotFound;
            existingManager.Email = email;
            dbContext.Update(existingManager);
            dbContext.SaveChanges();
            return HttpStatusCode.OK;
        }

        public async Task<(HttpStatusCode, bool)> CreateManager(AddManager manager)
        {
            manager.Email = manager.Email == "string" ? ConstantValues.MANAGER_DEFAULT_EMAIL : manager.Email;
            var result = await managerRepository.AddManager(manager);
            return result switch
            {
                1 => (HttpStatusCode.OK, true),
                -1 => (HttpStatusCode.BadRequest, false),
                _ => (HttpStatusCode.InternalServerError, false),
            };

        }
    }
}