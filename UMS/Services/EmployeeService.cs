using System.Net;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UMS.Data;
using UMS.Encryption;
using UMS.Models.Employee;
using UMS.Models.Entities;
using UMS.Repositories.EmployeeManagement;

namespace UMS.Services
{

    public class EmployeeService(IEmployeeRepository employeeRepository,AesEncryption encryption,ManagerService managerService,ApplicationDbContext _dbContext,IValidator<AddEmployee> empValidator,IValidator<UpdateEmployee> updateEmpValidator )
    {

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.");
            }
            return await  _dbContext.Employees.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<(HttpStatusCode, bool)> CreateEmployee(AddEmployee employee)
        {
            var validationResult = empValidator.Validate(employee);
            if (!validationResult.IsValid) return (HttpStatusCode.BadRequest, validationResult.IsValid);
            var manager = managerService.GetManagerByDesignation(employee.DesignationId);
            var result= await employeeRepository.AddEmployee(employee,GenerateRandomEmpCode(),"Active",manager.Id);
            return result switch
            {
                1 => (HttpStatusCode.Created,true),
                -1 => (HttpStatusCode.BadRequest,false),
                _ => (HttpStatusCode.InternalServerError,false),
            };
        }
        public async Task<AddEmployee> GetById(int id)
        {
            var emp = await  employeeRepository.GetEmployeeById(id);
            return emp;
        }

        public async Task<Employee> EmployeeData(string userName)
        {
            return await _dbContext.Employees.FirstOrDefaultAsync(e => e.UserName == userName);
        }

        public async Task<List<Employee>> GetAllEmployees(DataTableRequest request)
        {
            var employees = await employeeRepository.GetAllEmployees(request);
            return employees.Count == 0 ? [] : employees;
        }


        public async Task<(HttpStatusCode,bool)> UpdateEmployee(int id,UpdateEmployee employee)
        {
            employee.Password = encryption.EncryptString(employee.Password); 
            var validationResult = updateEmpValidator.Validate(employee);
            if (!validationResult.IsValid) return (HttpStatusCode.BadRequest, validationResult.IsValid);
            var manager = managerService.GetManagerByDesignation(employee.DesignationId);
            var result = await employeeRepository.UpdateEmployee(id,manager.Id,status:"Active",employee);
            return result switch
            {
                1 => (HttpStatusCode.OK, true),
                -1 => (HttpStatusCode.BadRequest, false),
                0 => (HttpStatusCode.Conflict,false),
                _ => (HttpStatusCode.InternalServerError, false),
            };
        }

        public async Task<(HttpStatusCode,bool)> DeleteEmployee(int id)
        {
            var result =  await employeeRepository.DeleteEmployee(id);
            return result switch
            {
                1 => (HttpStatusCode.OK, true),
                -1 => (HttpStatusCode.BadRequest, false),
                _ => (HttpStatusCode.InternalServerError, false),
            };
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbContext.Employees.AnyAsync(e => e.Email == email);
        }
        public async Task<string> GetEmployeeEmail(string otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
            {
                return "OTP is required";
            }
            var employee = await _dbContext.LoginVerificationOTPs.FirstOrDefaultAsync(e => e.OTP == otp);
            if (employee != null)
            {
                return employee.Email ?? "Employee email is null.";
            }
            else
            {
                return "Employee with the specified OTP does not exist.";
            }
        }
        public string GenerateRandomEmpCode()
        {
            return "EMP" + new Random().Next(1000, 9999);
        }
    }   
}