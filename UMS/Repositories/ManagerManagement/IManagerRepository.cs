using UMS.Models.Employee;
using UMS.Models.Entities;
using UMS.Models.Manager;

namespace UMS.Repositories.ManagerManagement;

public interface IManagerRepository
{
    Task<List<GetManagerQueryResponse>> GetAllManagers(DataTableRequest request);
    Task<GetManagerByIdQueryResponse> GetManagerById(int id);
    Task<int> UpdateManager(int id,AddManager manager);
    Task<int> DeleteManager(int id);
    Task<int> AddManager(AddManager manager);
}