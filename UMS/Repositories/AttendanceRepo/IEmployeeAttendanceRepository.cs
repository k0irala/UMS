using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Models.Entities;
namespace UMS.Repositories.AttendanceRepo;

public interface IEmployeeAttendanceRepository
{
    Task<IDictionary<string,List<EmployeeAttendanceModel>>> GetAttendance();
    Task<IEnumerable<Attendance>> GetEmployeeAttendance(int employeeId);

    Task<IEnumerable<Attendance>> GetAttendanceByEmp(string email);
    
    Task<HttpStatusCode> CreateEmployeeAttendance(AttendanceModel attendance,int employeeId,string role);

}