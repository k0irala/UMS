namespace UMS.Models.Entities
{
    public class RefreshTokenManager
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public int ManagerId { get; set; }
        public string RefreshUserToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddDays(5); // Default expiry of 5 days
        public Manager Manager { get; set; } = new Manager();
    }
}
