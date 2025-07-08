using System.Net;
using Dapper;
using UMS.Models;
using UMS.Models.Manager;

namespace UMS.Repositories.AttendanceRepo;

public class ManagerAttendanceRepository(IDapperRepository repository) : IManagerAttendanceRepository
{
    public IDictionary<string, List<ManagerAttendanceModel>> GetAttendance()
    {
        DynamicParameters parameters = new();
        var result = repository.Query<ManagerAttendanceModel>(StoredProcedures.GET_ALL_MANAGER_ATTENDANCE,parameters);
        var managerAttendanceModels = result as ManagerAttendanceModel[] ?? result.ToArray();
        return managerAttendanceModels.ToDictionary(k => k.ManagerName,res => new List<ManagerAttendanceModel>(managerAttendanceModels));
    }

    public IEnumerable<ManagerAttendanceModel> GetManagerAttendanceById(int managerId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@empId",managerId);
        
        var result = repository.Query<ManagerAttendanceModel>(StoredProcedures.GET_MANAGER_ATTENDANCE_BY_ID,parameters);
        return result;
    }

    public IEnumerable<ManagerAttendanceModel> GetMyAttendance(int userId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@userId", userId);
        var result = repository.Query<ManagerAttendanceModel>(StoredProcedures.GET_MANAGER_ATTENDANCE, parameters);
        return result;
    }

    public HttpStatusCode CreateManagerAttendance(ManagerAttendanceModel attendance)
    {
        var attendanceDate = attendance.Date.Date;
        
        DynamicParameters parameters = new();
        parameters.Add("@Date", attendanceDate);
        parameters.Add("@CheckinTime", attendance.CheckInTime);
        parameters.Add("@CheckoutTime", attendance.CheckOutTime);
        parameters.Add("@IsPresent", attendance.IsPresent);
        parameters.Add("@Remarks", attendance.Remarks);
        parameters.Add("@managerId",attendance.ManagerId);
        parameters.Add("@CreatedAt", attendance.CreatedAt);
        parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

        repository.Execute(StoredProcedures.ADD_MANAGER_ATTENDANCE, parameters);
        var data = parameters.Get<int>("@Result");
        
        return data == 0 ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
    }
}