using UMS.Models.Entities;

namespace UMS.Services;

public interface IEmployeeService
{
    Task<Employee> GetEmployeeByIdAsync(int id);
    Task<Employee> EmployeeData(string userName);
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<bool> UpdateEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(int id);
    Task<bool> IsEmailUniqueAsync(string email);
    string GetEmployeeEmail(string email);
    Employee GetEmployeeByEmail(string email);
}