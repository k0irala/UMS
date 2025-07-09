
using System.Net;
using UMS.Models.Employee;
using UMS.Models.Entities;
namespace UMS.Repositories.EmployeeManagement;
public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllEmployees(DataTableRequest request);
    Task<AddEmployee> GetEmployeeById(int id);
    Task<int> UpdateEmployee(int id,int managerId,string status,UpdateEmployee employee);
    Task<int> DeleteEmployee(int id);
    Task<int> AddEmployee(AddEmployee employee, string code, string status, int managerId);
}