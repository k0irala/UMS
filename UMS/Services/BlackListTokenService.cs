using System.Data;
using System.Net;
using Dapper;
using UMS.Models;
using UMS.Repositories;
using UMS.Repositories.BlackListToken;

namespace UMS.Services;

public class BlackListTokenService(IBlackListTokenRepository repository)
{
    public bool IsBlackListToken(string token)
    {  
        var result = repository.IsBlackListToken(token);
        return result;
    }

    public (HttpStatusCode,bool) SaveBlackListToken(BlackListToken blackListToken)
    {
        var result = repository.SaveBlackListToken(blackListToken);
        return result switch
        {
            1 => (HttpStatusCode.OK, true),
            0 => (HttpStatusCode.BadRequest, false),
            _ => (HttpStatusCode.Conflict, false)
        };
    }
}