using System.Net;
using UMS.Models;
using UMS.Models.Entities;
using UMS.Models.Manager;

namespace UMS.Repositories.AttendanceRepo;

public interface IManagerAttendanceRepository
{
    IDictionary<string,List<ManagerAttendanceModel>> GetAttendance();
    IEnumerable<ManagerAttendanceModel> GetManagerAttendanceById(int managerId);
    IEnumerable<ManagerAttendanceModel> GetMyAttendance(int userId);
    HttpStatusCode CreateManagerAttendance(ManagerAttendanceModel attendance);
}