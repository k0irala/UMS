namespace UMS.Models;

public class BlackListToken
{
    public int Id { get; set; }
    public string Token { get; set; } =string.Empty;
    public DateTime ExpiresAt { get; set; }
}