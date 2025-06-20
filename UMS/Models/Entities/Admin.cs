namespace UMS.Models.Entities
{
    public class Admin
    {
        public int Id { get; set; } = ConstantValues.ADMIN_DEFAULT_ID;
        public string Email { get; set; } = ConstantValues.ADMIN_DEFAULT_EMAIL;
        public string Password { get; set; } = ConstantValues.ADMIN_DEFAULT_PASSWORD;
        public string UserName { get; set; } = ConstantValues.ADMIN_DEFAULT_USERNAME;
        public string FullName { get; set; } = ConstantValues.ADMIN_DEFAULT_FULLNAME;
        public int RoleId { get; set; } = ConstantValues.ADMIN_DEFAULT_ROLE_ID;

        public virtual Roles Roles { get; set; } 
    }
}