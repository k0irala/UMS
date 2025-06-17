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
        #endregion
    }
}
