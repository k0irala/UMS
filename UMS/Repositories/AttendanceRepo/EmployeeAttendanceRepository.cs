using System.Net;
using Dapper;
using UMS.Models;
using UMS.Models.Entities;
using UMS.Services;

namespace UMS.Repositories.AttendanceRepo;

public class EmployeeAttendanceRepository(IDapperRepository repository,IEmployeeService employeeService,IManagerService managerService) : IEmployeeAttendanceRepository
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

    public HttpStatusCode CreateEmployeeAttendance(AttendanceModel attendance,int employeeId,string role)
    {
        if (role == "Employee") return HttpStatusCode.OK;
        var result = employeeService.GetEmployeeByIdAsync(employeeId);
        var empData =result?.Result;
        if (empData == null) return HttpStatusCode.BadRequest;
        var manager = managerService.GetManagerByDesignation(empData.DesignationId);
        if (manager == null)
            return HttpStatusCode.BadRequest;
        var employees = managerService.GetEmployeeByManager(manager.Id);
        if (employees == null)
            return HttpStatusCode.BadRequest;
        
        DynamicParameters parameters = new();
        parameters.Add("@Date", attendance.Date);
        parameters.Add("@CheckinTime", attendance.CheckInTime);
        parameters.Add("@CheckoutTime", attendance.CheckOutTime);
        parameters.Add("@IsPresent", attendance.IsPresent);
        parameters.Add("@Remarks", attendance.Remarks);
        parameters.Add("@CreatedAt", attendance.CreatedAt);
        parameters.Add("@empId", employeeId);
        parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

        repository.Execute(StoredProcedures.ADD_EMP_ATTENDANCE, parameters);
        var data = parameters.Get<int>("@Result");
        
        return data == 0 ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
    }
    
    
}