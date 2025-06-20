namespace UMS.Models.Employee
{
    public class AddEmployee
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int DesignationId { get; set; }
    }
}
