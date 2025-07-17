using Swashbuckle.AspNetCore.Filters;
using UMS.Models.Employee;

namespace UMS.ResponseExamples;

public class EmpAttendanceResponseExample : IExamplesProvider<ApiResponse<IEnumerable<EmployeeAttendanceModel>>>
{
    public ApiResponse<IEnumerable<EmployeeAttendanceModel>> GetExamples()
    {
        var attendanceList = new List<EmployeeAttendanceModel>()
        {
            new EmployeeAttendanceModel
            {
                Id = 1,
                EmployeeId = 101,
                EmployeeName = "John Doe",
                Date = new DateTime(2025, 7, 17),
                CheckInTime = new TimeSpan(9, 0, 0),
                CheckOutTime = new TimeSpan(17, 30, 0),
                IsPresent = true,
                Remarks = "On time",
                CreatedAt = DateTime.Now
            },
            new EmployeeAttendanceModel
            {
                Id = 2,
                EmployeeId = 102,
                EmployeeName = "Jane Smith",
                Date = new DateTime(2025, 7, 17),
                CheckInTime = new TimeSpan(9, 15, 0),
                CheckOutTime = new TimeSpan(17, 45, 0),
                IsPresent = true,
                Remarks = "Late check-in",
                CreatedAt = DateTime.Now
            },
            new EmployeeAttendanceModel
            {
                Id = 3,
                EmployeeId = 103,
                EmployeeName = "Robert Brown",
                Date = new DateTime(2025, 7, 17),
                CheckInTime = new TimeSpan(0, 0, 0), // Not present
                CheckOutTime = null,
                IsPresent = false,
                Remarks = "Absent",
                CreatedAt = DateTime.Now
            }
        };

        return new ApiResponse<IEnumerable<EmployeeAttendanceModel>>
        {
            code = 200,
            message = "Employee attendance fetched successfully",
            data = attendanceList,
            meta = new MetaData
            {
                pagination = new Pagination
                {
                    totalItems = 3,
                    currentPage = 1,
                    pageSize = 10,
                    totalPages = 1,
                    hasNextPage = false
                },
                sort = new Sort
                {
                    field = "Date",
                    order = "desc"
                }
            }
        };
    }
}
    