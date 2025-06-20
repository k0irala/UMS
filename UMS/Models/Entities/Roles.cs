namespace UMS.Models.Entities
{
    public class Roles
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public ICollection<Admin> Admin { get; set; } = [];
    }
}
