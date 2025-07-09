namespace UMS.Repositories.BlackListToken;

public interface IBlackListTokenRepository
{
    Task<int> SaveBlackListToken(Models.BlackListToken blackListToken);
    Task<bool> IsBlackListToken(string token);
}