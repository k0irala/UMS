namespace UMS.Models.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DesignationId { get; set; } 
        public int ManagerId { get; set; } 
        public ICollection<RefreshTokenEmployee> RefreshTokens { get; set; } = [];
    }
}
