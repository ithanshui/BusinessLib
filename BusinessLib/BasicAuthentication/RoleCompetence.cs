namespace BusinessLib.BasicAuthentication
{
    #region RoleCompetence

    public class RoleCompetence
    {
        //==SELECT==//
        public System.Collections.Generic.List<RoleCompetence> RoleCompetences { get; set; }
        //==SELECT==//
        public Accounts Account { get; set; }
        public System.Collections.Generic.List<Roles> Roles { get; set; }
        //==========================================================//
        //All Competence !Check!
        public System.Collections.Generic.List<Competences> CompetenceAll { get; set; }
        //All Account
        public System.Collections.Generic.List<Accounts> AccountAll { get; set; }
        //All Role
        public System.Collections.Generic.List<Roles> RoleAll { get; set; }
        //==========================================================//
        public System.Collections.Generic.List<RoleCompetence> RoleCompetenceAll { get; set; }
    }
    public class Roles
    {
        public System.String SysAccount_Role_gid { get; set; }
        public System.String CreateKey { get; set; }
        public System.String AccountKey { get; set; }
        public System.String Parent { get; set; }
        public System.String Key { get; set; }
        public System.String Role { get; set; }
        public System.String Descrip { get; set; }
        public System.Collections.Generic.List<Competences> Competences { get; set; }
        //public System.Boolean IsChild { get; set; }
    }
    public class Competences
    {
        public System.String Role_Competence_gid { get; set; }
        public System.String Parent { get; set; }
        public System.String Key { get; set; }
        public System.String Role { get; set; }
        public System.String Competence { get; set; }
        public System.String Descrip { get; set; }
        //public System.Boolean IsChild { get; set; }
    }
    public class Accounts
    {
        public System.String Parent { get; set; }
        public System.String Key { get; set; }
        public System.String Account { get; set; }
        public System.Boolean IsChild { get; set; }
    }

    #endregion
}
