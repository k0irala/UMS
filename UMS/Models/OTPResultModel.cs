namespace UMS.Models;

public class OTPResultModel
{
        public bool Success { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
}