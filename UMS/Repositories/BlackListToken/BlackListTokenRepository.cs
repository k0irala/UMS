using System.Data;
using Dapper;

namespace UMS.Repositories.BlackListToken;

public class BlackListTokenRepository(IDapperRepository repository) : IBlackListTokenRepository
{
    public async Task<int> SaveBlackListToken(Models.BlackListToken blackListToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@token", blackListToken.Token);
        parameters.Add("@expiresAt", blackListToken.ExpiresAt);
        parameters.Add("@Result",dbType:DbType.Int32,direction:ParameterDirection.Output);
        
        await repository.ExecuteAsync(StoredProcedures.SAVE_BLACK_LIST_TOKEN, parameters);
        return parameters.Get<int>("@Result");

    }

    public async Task<bool> IsBlackListToken(string token)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@token", token);
        var result = await repository.QuerySingleOrDefaultAsync<Models.BlackListToken>(StoredProcedures.GET_BLACK_LIST_TOKEN, parameters);
        return result != null;
    }
}