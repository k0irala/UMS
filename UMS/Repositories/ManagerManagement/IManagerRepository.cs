using UMS.Models.Employee;
using UMS.Models.Entities;
using UMS.Models.Manager;

namespace UMS.Repositories.ManagerManagement;

public interface IManagerRepository
{
    List<GetManagerQueryResponse> GetAllManagers(DataTableRequest request);
    GetManagerByIdQueryResponse GetManagerById(int id);
    int UpdateManager(int id,AddManager manager);
    int DeleteManager(int id);
    int AddManager(AddManager manager);
}