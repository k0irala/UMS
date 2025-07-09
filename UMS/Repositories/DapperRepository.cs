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
        public async Task<T> QuerySingleOrDefaultAsync<T>(string query, DynamicParameters parameters, CommandType type = CommandType.StoredProcedure)
        {
            var data = await DbConfig.EstablishConnection().QuerySingleOrDefaultAsync<T>(query, parameters, commandType: type);
            return data;
        }
        public async Task ExecuteAsync(string query, DynamicParameters parameters, CommandType type = CommandType.StoredProcedure)
        {
            await DbConfig.EstablishConnection().ExecuteAsync(query, parameters, commandType: type);
        }
    }
}
