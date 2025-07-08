using System.Net;
using UMS.Models.Manager;
using UMS.Repositories.AttendanceRepo;

namespace UMS.Services.Attendance;

public class ManagerAttendanceService(IManagerAttendanceRepository repository)
{
    public List<ManagerAttendanceModel> GetManagerAttendanceByID(int id)
    {
        var result = repository.GetManagerAttendanceById(id);
        return result.ToList();
    }

    public IDictionary<string, List<ManagerAttendanceModel>> GetAllManagerAttendance()
    {
        var result = repository.GetAttendance();
        return result;
    }

    public List<ManagerAttendanceModel> GetMyAttendance(int id)
    {
        var result = repository.GetMyAttendance(id);
        return result.ToList();
    }

    public HttpStatusCode AddManagerAttendance(ManagerAttendanceModel model)
    {
        var result = repository.CreateManagerAttendance(model);
        return result;
    }
}