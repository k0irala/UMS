﻿using System.Net;
using Dapper;
using Swashbuckle.AspNetCore.SwaggerGen;
using UMS.Models;
using UMS.Models.Employee;
using UMS.Models.Entities;
using UMS.Services;

namespace UMS.Repositories.AttendanceRepo;

public class EmployeeAttendanceRepository(IDapperRepository repository,EmployeeService employeeService,ManagerService managerService) : IEmployeeAttendanceRepository
{
    public async Task<IDictionary<string,List<EmployeeAttendanceModel>>> GetAttendance()
    {
        DynamicParameters parameters = new();
        var result = await repository.QueryAsync<EmployeeAttendanceModel>(StoredProcedures.GET_ALL_EMP_ATTENDANCE,parameters);
        var employeeAttendanceModels = result as EmployeeAttendanceModel[] ?? result.ToArray();
        return employeeAttendanceModels.ToDictionary(k => k.EmployeeName,res => new List<EmployeeAttendanceModel>(employeeAttendanceModels));
    }

    public async Task<IEnumerable<Attendance>> GetEmployeeAttendance(int employeeId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@empId", employeeId);
        
        var result = await repository.QueryAsync<Attendance>(StoredProcedures.GET_EMP_ATTENDANCE_BY_ID,parameters);
        return result;
    }

    public async Task<IEnumerable<Attendance>> GetAttendanceByEmp(string email)
    {
        var emp = employeeService.GetEmployeeByEmail(email);
        DynamicParameters parameters = new();
        parameters.Add("@empId", emp.Id);
        var result = await repository.QueryAsync<Attendance>(StoredProcedures.GET_EMP_ATTENDANCE, parameters);
        return result;
    }

    public async Task<HttpStatusCode> CreateEmployeeAttendance(AttendanceModel attendance,int employeeId,string role)
    {
        if (role == "Employee") return HttpStatusCode.OK;
        var result = await employeeService.GetById(employeeId);
        var empData =result;
        if (empData == null) return HttpStatusCode.BadRequest;
        var manager = managerService.GetManagerByDesignation(empData.DesignationId);
        if (manager == null)
            return HttpStatusCode.BadRequest;
        var employees = managerService.GetEmployeeByManager(manager.Id);
        if (employees == null)
            return HttpStatusCode.BadRequest;
        var attendanceDate = attendance.Date.Date;
        
        DynamicParameters parameters = new();
        parameters.Add("@Date", attendanceDate);
        parameters.Add("@CheckinTime", attendance.CheckInTime);
        parameters.Add("@CheckoutTime", attendance.CheckOutTime);
        parameters.Add("@IsPresent", attendance.IsPresent);
        parameters.Add("@Remarks", attendance.Remarks);
        parameters.Add("@CreatedAt", attendance.CreatedAt);
        parameters.Add("@empId", employeeId);
        parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

        repository.ExecuteAsync(StoredProcedures.ADD_EMP_ATTENDANCE, parameters);
        var data = parameters.Get<int>("@Result");
        
        return data == 0 ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
    }
    
    
}