using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Models.Entities;
namespace UMS.Repositories.AttendanceRepo;

public interface IEmployeeAttendanceRepository
{
    IDictionary<string,EmployeeAttendanceModel> GetAttendance();
    IEnumerable<Attendance> GetEmployeeAttendance(int employeeId);

    IEnumerable<Attendance> GetAttendanceByEmp(string email);
    
    HttpStatusCode CreateEmployeeAttendance(AttendanceModel attendance,int employeeId,string role);

}