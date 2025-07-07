using System.Data;
using Dapper;

namespace UMS.Repositories.BlackListToken;

public class BlackListTokenRepository(IDapperRepository repository) : IBlackListTokenRepository
{
    public int SaveBlackListToken(Models.BlackListToken blackListToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@token", blackListToken.Token);
        parameters.Add("@expiresAt", blackListToken.ExpiresAt);
        parameters.Add("@Result",dbType:DbType.Int32,direction:ParameterDirection.Output);
        
        repository.Execute(StoredProcedures.SAVE_BLACK_LIST_TOKEN, parameters);   
        var result = parameters.Get<int>("@Result");
        
        return result;
    }

    public bool IsBlackListToken(string token)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@token", token);
        var result = repository.QuerySingleOrDefault<Models.BlackListToken>(StoredProcedures.GET_BLACK_LIST_TOKEN, parameters);
        return result != null;
    }
}