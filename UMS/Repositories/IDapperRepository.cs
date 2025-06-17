using Dapper;
using System.Data;

namespace UMS.Repositories
{
    public interface IDapperRepository
    {
        Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure);

        IEnumerable<T> Query<T>(string query, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure);

        T QuerySingleOrDefault<T>(string query, DynamicParameters parameters, CommandType type = CommandType.StoredProcedure);
        void Execute(string query, DynamicParameters parameters, CommandType type = CommandType.StoredProcedure);
    }
}
