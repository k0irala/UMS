using System.Data;
using Dapper;

namespace UMS.DynamicParametersExtension;

public static class DynamicParametersExtensions
{
    public static void AddDefaultPaginationParameters(this DynamicParameters parameters, DataTableRequest request)
    {
        parameters.Add("@orderByCol", request.OrderColumn);
        parameters.Add("@sortOrder", request.OrderDirection == "" ? "ASC" : request.OrderDirection);
        parameters.Add("@currentPage", request.Skip / (request.Take == 0 ? 1 : request.Take) + 1);
        parameters.Add("@itemPerPage", request.Take);
        parameters.Add("@totalItems", null, DbType.Int32, ParameterDirection.Output, sizeof(int));
    }
}