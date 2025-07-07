namespace UMS.Repositories.BlackListToken;

public interface IBlackListTokenRepository
{
    int SaveBlackListToken(Models.BlackListToken blackListToken);
    bool IsBlackListToken(string token);
}