using Microsoft.EntityFrameworkCore;
using UMS.Data;
using UMS.Models.Entities;
using UMS.Repositories.EmployeeManagement;

namespace UMS.Services
{

    public class EmployeeService(IEmployeeRepository employeeRepository,ApplicationDbContext _dbContext) : IEmployeeService
    {

        public Employee? GetEmployeeByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.");
            }
            return _dbContext.Employees.FirstOrDefault(e => e.Email == email);
        }
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("Invalid employee ID.");
            }
            return await _dbContext.Employees.FindAsync(id);
        }

        public async Task<Employee> EmployeeData(string userName)
        {
            return await _dbContext.Employees.FirstOrDefaultAsync(e => e.UserName == userName);
        }

        public List<Employee> GetAllEmployees(DataTableRequest request)
        {
            var employees = employeeRepository.GetAllEmployees(request);
            return employees.Count == 0 ? [] : employees;
        }


        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            _dbContext.Employees.Update(employee);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await GetEmployeeByIdAsync(id);
            if (employee != null)
            {
                _dbContext.Employees.Remove(employee);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbContext.Employees.AnyAsync(e => e.Email == email);
        }
        public string GetEmployeeEmail(string otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
            {
                return "OTP is required";
            }
            var employee = _dbContext.LoginVerificationOTPs.FirstOrDefault(e => e.OTP == otp);
            if (employee != null)
            {
                return employee.Email ?? "Employee email is null.";
            }
            else
            {
                return "Employee with the specified OTP does not exist.";
            }
        }
    }   
}