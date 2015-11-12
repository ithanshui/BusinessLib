namespace BusinessLib.BasicAuthentication
{
    public interface ISession
    {
        System.String Site { get; set; }
        System.String Account { get; set; }
        System.String Password { get; set; }
        System.String SecurityCode { get; set; }

        System.String Key { get; set; }
        System.String IP { get; set; }

        System.DateTime Time { get; set; }
        RoleCompetence RoleCompetence { get; set; }

        ISession Clone();
    }

    public interface ISysInfo
    {
        System.String OS { get; set; }
        System.Collections.Generic.List<System.String> Mac { get; set; }
        System.String Cpu { get; set; }
        System.String Memory { get; set; }
        System.String Board { get; set; }
        System.String Disk { get; set; }
        System.String Browser { get; set; }
        System.String Sign { get; set; }
    }
}
