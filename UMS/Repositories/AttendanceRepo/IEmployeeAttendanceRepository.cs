using System.Net;
using UMS.Models;
using UMS.Models.Entities;
namespace UMS.Repositories.AttendanceRepo;

public interface IEmployeeAttendanceRepository
{
    IEnumerable<Attendance> GetAttendance();
    IEnumerable<Attendance> GetEmployeeAttendance(int employeeId);

    IEnumerable<Attendance> GetAttendanceByEmp(string email);
    
    HttpStatusCode CreateEmployeeAttendance(AttendanceModel attendance,int employeeId,string role);

}