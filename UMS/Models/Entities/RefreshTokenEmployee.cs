
namespace UMS.Models.Entities
{
    public class RefreshTokenEmployee
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string RefreshUserToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddDays(5); // Default expiry of 5 days
        public Employee Employee { get; set; } = new Employee();
    }
}
