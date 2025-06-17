using UMS.Enums;

namespace UMS
{
    public class ConstantValues
    {
        #region ManagerEmail
        public const string MANAGER_DEFAULT_EMAIL = "manager.validator@gmail.com";
        #endregion

        #region Admin Credentials
        public const int ADMIN_DEFAULT_ID = 99;
        public const string ADMIN_DEFAULT_EMAIL = "admin@admin.com";
        public const string ADMIN_DEFAULT_PASSWORD = "admin1234";
        public const string ADMIN_DEFAULT_USERNAME = "admin";
        public const string ADMIN_DEFAULT_FULLNAME = "Admin User";
        public const string ADMIN_DEFAULT_ROLE = "Admin";
        public const int ADMIN_DEFAULT_ROLE_ID = (int)Roles.Admin;
        #endregion
    }
}
