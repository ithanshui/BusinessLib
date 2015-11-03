namespace BusinessLib.Mark
{
    using BusinessLib.Extensions;

    #region MarkEnum

    public enum MarkEnum
    {
        Sys_KEY = 99,
        Sys_IV = 98,

        /// <summary>
        /// Undefined Exception
        /// </summary>
        Exp_UndefinedException = 0,
        /// <summary>
        /// Arguments Illegal
        /// </summary>
        Exp_ArgumentsIllegal = -1,
        /// <summary>
        /// Site Illegal
        /// </summary>
        Exp_SiteIllegal = -2,
        /// <summary>
        /// Session Illegal
        /// </summary>
        Exp_SessionIllegal = -3,
        /// <summary>
        /// Session Out
        /// </summary>
        Exp_SessionOut = -4,
        /// <summary>
        /// Competence Illegal
        /// </summary>
        Exp_CompetenceIllegal = -5,
        /// <summary>
        /// Competence List Not Exist
        /// </summary>
        Exp_CompetenceListNotExist = -6,
        /// <summary>
        /// User Error
        /// </summary>
        Exp_UserError = -7,
        /// <summary>
        /// Password Error
        /// </summary>
        Exp_PasswordError = -8,
        /// <summary>
        /// User Frozen
        /// </summary>
        Exp_UserFrozen = -9,
        /// <summary>
        /// RoleCompetence Not Exist
        /// </summary>
        Exp_RoleCompetenceNotExist = -10,
        /// <summary>
        /// Account Not Exist
        /// </summary>
        Exp_AccountNotExist = -11,
        /// <summary>
        /// Role Not Exist
        /// </summary>
        Exp_RoleNotExist = -12,
        /// <summary>
        /// Competence Not Exist
        /// </summary>
        Exp_CompetenceNotExist = -13,
        /// <summary>
        /// Login Time Space
        /// </summary>
        Exp_LoginTimeSpace = -14,

        /// <summary>
        /// Result_State
        /// </summary>
        Result_State = 97,
        /// <summary>
        /// OutTime_Session_Time 30
        /// </summary>
        OutTime_Session_Time = 96,
        /// <summary>
        /// Login_ErrorCount 3
        /// </summary>
        Login_ErrorCount = 95,
        /// <summary>
        /// Login_ErrorRange 2
        /// </summary>
        Login_ErrorRange = 94,
        /// <summary>
        /// Login_ErrorFreeze 3
        /// </summary>
        Login_ErrorFreeze = 93,

        /// <summary>
        /// SYS
        /// </summary>
        Sys_Action = 92,
        /// <summary>
        /// R_SysAccount
        /// </summary>
        R_SysAccount = 91,
        /// <summary>
        /// R_SysAccount_Role
        /// </summary>
        R_SysAccount_Role = 90,
        /// <summary>
        /// R_SysRole
        /// </summary>
        R_SysRole = 89,
        /// <summary>
        /// R_SysRole_Competence
        /// </summary>
        R_SysRole_Competence = 88,
        /// <summary>
        /// R_SysCompetence
        /// </summary>
        R_SysCompetence = 87,
        /// <summary>
        /// C_SysConfig
        /// </summary>
        C_SysConfig = 86,
    }

    #endregion

    public class MarkBase : Extensions.IDictionary<System.Object, System.Object, System.Collections.DictionaryEntry>
    {
        internal readonly static System.Collections.Specialized.HybridDictionary MarkList = new System.Collections.Specialized.HybridDictionary();

        static MarkBase()
        {
            //MarkList.Add("KEY", "BLNew!@#");
            //MarkList.Add("IV", "BLNewIV!");

            //MarkList.Add("RESULT_State", "ResultState");
            //MarkList.Add("OUTTIME_Session_Time", 30);
            //MarkList.Add("LOGIN_ErrorCount", 3);
            //MarkList.Add("LOGIN_ErrorRange", 2);
            //MarkList.Add("LOGIN_ErrorFreeze", 3);

            //MarkList.Add("SYS_Action", "SYS");
            //MarkList.Add("R_SysAccount", "R_SysAccount");
            //MarkList.Add("R_SysAccount_Role", "R_SysAccount_Role");
            //MarkList.Add("R_Role", "R_Role");
            //MarkList.Add("R_Role_Competence", "R_Role_Competence");
            //MarkList.Add("R_Competence", "R_Competence");
            //MarkList.Add("C_SysConfig", "C_SysConfig");

            //MarkList.Add(0, "Undefined Exception");
            //MarkList.Add(-1, "Arguments Illegal");
            //MarkList.Add(-2, "Site Illegal");
            //MarkList.Add(-3, "Session Illegal");
            //MarkList.Add(-4, "Session Out");
            //MarkList.Add(-5, "Competence Illegal");
            //MarkList.Add(-6, "Competence List Not Exist");
            //MarkList.Add(-7, "User Error");
            //MarkList.Add(-8, "Password Error");
            //MarkList.Add(-9, "User Frozen");

            //MarkList.Add(-10, "RoleCompetence Not Exist");
            //MarkList.Add(-11, "Account Not Exist");
            //MarkList.Add(-12, "Role Not Exist");
            //MarkList.Add(-13, "Competence Not Exist");
            MarkList.Add(MarkEnum.Sys_KEY, "BLNew!@#");
            MarkList.Add(MarkEnum.Sys_IV, "BLNewIV!");

            MarkList.Add(MarkEnum.Result_State, "ResultState");
            MarkList.Add(MarkEnum.OutTime_Session_Time, 60 * 24);
            MarkList.Add(MarkEnum.Login_ErrorCount, 3);
            MarkList.Add(MarkEnum.Login_ErrorRange, 2);
            MarkList.Add(MarkEnum.Login_ErrorFreeze, 3);

            MarkList.Add(MarkEnum.Sys_Action, "SYS");
            MarkList.Add(MarkEnum.R_SysAccount, "R_SysAccount");
            MarkList.Add(MarkEnum.R_SysAccount_Role, "R_SysAccount_Role");
            MarkList.Add(MarkEnum.R_SysRole, "R_SysRole");
            MarkList.Add(MarkEnum.R_SysRole_Competence, "R_SysRole_Competence");
            MarkList.Add(MarkEnum.R_SysCompetence, "R_SysCompetence");
            MarkList.Add(MarkEnum.C_SysConfig, "C_SysConfig");

            MarkList.Add(MarkEnum.Exp_UndefinedException, "Undefined Exception");
            MarkList.Add(MarkEnum.Exp_ArgumentsIllegal, "Arguments Illegal");
            MarkList.Add(MarkEnum.Exp_SiteIllegal, "Site Illegal");
            MarkList.Add(MarkEnum.Exp_SessionIllegal, "Session Illegal");
            MarkList.Add(MarkEnum.Exp_SessionOut, "Session Out");
            MarkList.Add(MarkEnum.Exp_CompetenceIllegal, "Competence Illegal");
            MarkList.Add(MarkEnum.Exp_CompetenceListNotExist, "Competence List Not Exist");
            MarkList.Add(MarkEnum.Exp_UserError, "User Error");
            MarkList.Add(MarkEnum.Exp_PasswordError, "Password Error");
            MarkList.Add(MarkEnum.Exp_UserFrozen, "User Frozen");

            MarkList.Add(MarkEnum.Exp_RoleCompetenceNotExist, "RoleCompetence Not Exist");
            MarkList.Add(MarkEnum.Exp_AccountNotExist, "Account Not Exist");
            MarkList.Add(MarkEnum.Exp_RoleNotExist, "Role Not Exist");
            MarkList.Add(MarkEnum.Exp_CompetenceNotExist, "Competence Not Exist");

            MarkList.Add(MarkEnum.Exp_LoginTimeSpace, "Login Time Space");
        }

        public static Type GetObject<Type>(object key)
        {
            return MarkList[key].ChangeType<Type>();
        }

        #region IDictionary

        public RemovedCallback<object, object> RemovedHandler { get; set; }

        public void Set<Type>(object key, Type value)
        {
            MarkList.Add(key, value);
        }

        public object Get(object key)
        {
            return MarkList[key];
        }

        public Type Get<Type>(object key)
        {
            return Get(key).ChangeType<Type>();
        }

        public void Remove(object key)
        {
            MarkList.Remove(key);

            if (null != RemovedHandler) { RemovedHandler(key, null); }
        }

        public bool Contains(object key)
        {
            return MarkList.Contains(key);
        }

        public long Count
        {
            get { return MarkList.Count; }
        }

        public void Clear()
        {
            MarkList.Clear();
        }

        public System.Collections.Generic.IEnumerator<System.Collections.DictionaryEntry> GetEnumerator()
        {
            foreach (System.Collections.DictionaryEntry item in MarkList)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}