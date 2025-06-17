using Dapper;
using System.Data;
using System.Data.Common;

namespace UMS.Repositories
{
    public class DapperRepository : IDapperRepository
    {
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            return await DbConfig.EstablishConnection().QueryAsync<T>(query, parameters, commandType: commandType);
        }
        public IEnumerable<T> Query<T>(string query, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            return DbConfig.EstablishConnection().Query<T>(query, parameters, commandType: commandType);
        }
        public T QuerySingleOrDefault<T>(string query, DynamicParameters parameters, CommandType type = CommandType.StoredProcedure)
        {
            var data = DbConfig.EstablishConnection().QuerySingleOrDefault<T>(query, parameters, commandType: type);
            return data == null ? throw new Exception("No data found for the given query.") : data;
        }
        public void Execute(string query, DynamicParameters parameters, CommandType type = CommandType.StoredProcedure)
        {
            DbConfig.EstablishConnection().Execute(query, parameters, commandType: type);
        }
    }
}
