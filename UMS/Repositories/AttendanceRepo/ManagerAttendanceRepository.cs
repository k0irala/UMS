using System.Net;
using Dapper;
using UMS.Models;
using UMS.Models.Manager;

namespace UMS.Repositories.AttendanceRepo;

public class ManagerAttendanceRepository(IDapperRepository repository) : IManagerAttendanceRepository
{
    public async Task<IDictionary<string, List<ManagerAttendanceModel>>> GetAttendance()
    {
        DynamicParameters parameters = new();
        parameters.Add("@managerId",0);
        parameters.Add("@type","getAllAttendance");
        var result = await repository.QueryAsync<ManagerAttendanceModel>(StoredProcedures.GET_ALL_MANAGER_ATTENDANCE,parameters);
        var managerAttendanceModels = result as ManagerAttendanceModel[] ?? result.ToArray();
        return managerAttendanceModels.ToDictionary(k => k.ManagerName,res => new List<ManagerAttendanceModel>(managerAttendanceModels));
    }

    public async Task<IEnumerable<ManagerAttendanceModel>> GetManagerAttendanceById(int managerId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@managerId",managerId);
        parameters.Add("@type","attendanceById");
        var result = await repository.QueryAsync<ManagerAttendanceModel>(StoredProcedures.GET_ALL_MANAGER_ATTENDANCE,parameters);
        return result;
    }

    public async Task<IEnumerable<ManagerAttendanceModel>> GetMyAttendance(int userId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@userId", userId);
        var result = await repository.QueryAsync<ManagerAttendanceModel>(StoredProcedures.GET_MANAGER_ATTENDANCE, parameters);
        return result;
    }

    public async Task<HttpStatusCode> CreateManagerAttendance(AddManagerAttendanceModel attendance)
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

        await repository.ExecuteAsync(StoredProcedures.ADD_MANAGER_ATTENDANCE, parameters);
        var data = parameters.Get<int>("@Result");
        
        return data == 0 ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
    }
}