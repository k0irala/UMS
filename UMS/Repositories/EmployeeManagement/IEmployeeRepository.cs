
using System.Net;
using UMS.Models.Employee;
using UMS.Models.Entities;
namespace UMS.Repositories.EmployeeManagement;
public interface IEmployeeRepository
{
    List<Employee> GetAllEmployees(DataTableRequest request);
    AddEmployee GetEmployeeById(int id);
    int UpdateEmployee(int id,int managerId,string status,UpdateEmployee employee);
    int DeleteEmployee(int id);
    int AddEmployee(AddEmployee employee, string code, string status, int managerId);
}