namespace UMS.Models
{
    public class ManagerRegisterModel
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        //All managers will have same email Id to verify the login
        public int DesignationId { get; set; }
    }
}
