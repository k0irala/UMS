using System.Net;
using UMS.Models.Manager;
using UMS.Repositories.AttendanceRepo;

namespace UMS.Services.Attendance;

public class ManagerAttendanceService(IManagerAttendanceRepository repository)
{
    public async Task<List<ManagerAttendanceModel>> GetManagerAttendanceByID(int id)
    {
        var result = await repository.GetManagerAttendanceById(id);
        return result.ToList();
    }

    public async Task<IDictionary<string, List<ManagerAttendanceModel>>> GetAllManagerAttendance()
    {
        var result = await repository.GetAttendance();
        return result;
    }

    public async Task<List<ManagerAttendanceModel>> GetMyAttendance(int id)
    {
        var result = await repository.GetMyAttendance(id);
        return result.ToList();
    }

    public async Task<HttpStatusCode> AddManagerAttendance(AddManagerAttendanceModel model)
    {
        var result = await repository.CreateManagerAttendance(model);
        return result;
    }
}