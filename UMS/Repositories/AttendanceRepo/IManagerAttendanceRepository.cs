using System.Net;
using UMS.Models;
using UMS.Models.Entities;
using UMS.Models.Manager;

namespace UMS.Repositories.AttendanceRepo;

public interface IManagerAttendanceRepository
{
    Task<IDictionary<string,List<ManagerAttendanceModel>>> GetAttendance();
    Task<IEnumerable<ManagerAttendanceModel>> GetManagerAttendanceById(int managerId);
    Task<IEnumerable<ManagerAttendanceModel>> GetMyAttendance(int userId);
    Task<HttpStatusCode> CreateManagerAttendance(AddManagerAttendanceModel attendance);
}