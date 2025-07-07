namespace UMS
{
    public class StoredProcedures
    {
        #region Designation

        public const string ADD_DESIGNATION = "usp_AddDesignation";
        public const string GET_DESIGNATION_BY_ID = "usp_GetDesignationById";
        public const string GET_ALL_DESIGNATIONS = "usp_GetAllDesignations";
        public const string DELETE_DESIGNATION = "usp_DeleteDesignation";
        public const string UPDATE_DESIGNATION = "usp_UpdateDesignation";
        #endregion

        #region Account
        public const string MANAGER_REGISTER = "usp_RegisterManager";
        public const string USER_REGISTER = "dbo.usp_RegisterUser";
        public const string PASSWORD_RESET = "usp_ResetPassword";
        #endregion

        #region OTP
        public const string OTP_INSERT = "usp_InsertOTP";
        #endregion

        #region Manager
        public const string REFRESH_TOKEN_MANAGER_INSERT = "usp_RefreshTokenManagerInsert";
        public const string ADD_MANAGER = "usp_AddManager";
        public const string UPDATE_MANAGER = "usp_UpdateManager";
        public const string DELETE_MANAGER = "usp_DeleteManager";
        public const string GET_MANAGER_BY_ID = "usp_GetManagerById";
        public const string GET_ALL_MANAGERS = "usp_GetAllManagers";
        #endregion
        
        #region Employee
        public const string REFRESH_TOKEN_EMPLOYEE_INSERT = "usp_RefreshTokenEmployeeInsert";
        public const string GET_ALL_EMPLOYEE = "usp_GettAllEmployees";
        public const string ADD_EMPLOYEE = "usp_AddEmployee";
        public const string GET_EMP_BY_ID = "usp_GetEmployeeById";
        public const string DELETE_EMPLOYEE = "usp_DeleteEmployee";
        public const string UPDATE_EMPLOYEE =  "usp_UpdateEmployee";
        #endregion
        
        #region Attendance
        public const string GET_ALL_EMP_ATTENDANCE = "usp_GetAllEmployeeAttendance";
        public const string GET_EMP_ATTENDANCE_BY_ID = "usp_GetAllEmployeeAttendanceByID";
        public const string GET_EMP_ATTENDANCE = "usp_GetEmployeeAttendance";
        public const string ADD_EMP_ATTENDANCE = "usp_AddEmployeeAttendance";
        #endregion

        #region  Tokens

        public const string SAVE_BLACK_LIST_TOKEN = "usp_SaveBlackListToken";
        public const string GET_BLACK_LIST_TOKEN = "usp_GetBlackListToken";
        #endregion
    }
}
