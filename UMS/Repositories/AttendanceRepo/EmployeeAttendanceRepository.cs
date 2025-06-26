using Dapper;
using UMS.Models.Entities;
using UMS.Services;

namespace UMS.Repositories.AttendanceRepo;

public class EmployeeAttendanceRepository(IDapperRepository repository,IEmployeeService employeeService) : IEmployeeAttendanceRepository
{
    public IEnumerable<Attendance> GetAttendance()
    {
        DynamicParameters parameters = new();
        var result = repository.Query<Attendance>(StoredProcedures.GET_ALL_EMP_ATTENDANCE,parameters);
        return result;
    }

    public IEnumerable<Attendance> GetEmployeeAttendance(int employeeId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@empId", employeeId);
        
        var result = repository.Query<Attendance>(StoredProcedures.GET_EMP_ATTENDANCE_BY_ID,parameters);
        return result;
    }

    public IEnumerable<Attendance> GetAttendanceByEmp(string email)
    {
        var emp = employeeService.GetEmployeeByEmail(email);
        DynamicParameters parameters = new();
        parameters.Add("@empId", emp.Id);
        var result = repository.Query<Attendance>(StoredProcedures.GET_EMP_ATTENDANCE, parameters);
        return result;
    }
    
}