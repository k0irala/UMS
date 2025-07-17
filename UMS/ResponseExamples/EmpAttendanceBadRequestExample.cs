using Swashbuckle.AspNetCore.Filters;
using UMS;

public class EmpAttendanceBadRequestExample : IExamplesProvider<ApiResponse<object>>
{
    public ApiResponse<object> GetExamples()
    {
        return new ApiResponse<object>
        {
            code = 400,
            message = "Bad Request - Invalid input parameters",
            data = null,
            errors = new List<Errors>
            {
                new Errors
                {
                    errorCode = "VALIDATION_ERROR",
                    errorMessage = "Date parameter is required"
                },
                new Errors
                {
                    errorCode = "INVALID_FORMAT",
                    errorMessage = "Date format should be YYYY-MM-DD"
                }
            },
            meta = null
        };
    }
}