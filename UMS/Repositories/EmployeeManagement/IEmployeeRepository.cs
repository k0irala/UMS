
using System.Net;
using UMS.Models.Employee;
using UMS.Models.Entities;
namespace UMS.Repositories.EmployeeManagement;
public interface IEmployeeRepository
{
    List<Employee> GetAllEmployees(DataTableRequest request);
    Employee GetEmployeeById(int id);
    (HttpStatusCode,bool) UpdateEmployee(AddEmployee employee);
    (HttpStatusCode,bool) DeleteEmployee(int id);
    (HttpStatusCode,bool) AddEmployee(AddEmployee employee);
}