namespace UMS.Models.Entities
{
    public class LoginVerificationOTP
    {
        public int Id { get; set; }
        public string OTP { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddMinutes(5); // Default expiry of 5 minutes
        public string Email { get; set; } = string.Empty;   
    }
}
