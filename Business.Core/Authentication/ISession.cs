namespace Business.Authentication
{
    public interface ISession : ISerialize
    {
        System.String Site { get; set; }
        System.String Account { get; set; }
        System.String Password { get; set; }

        System.String Key { get; set; }
        System.String IP { get; set; }

        System.DateTime Time { get; set; }

        System.Collections.Generic.List<string> Competences { get; set; }

        ISession Clone();
    }

    public interface ISession<DataType> : ISession
    {
        new ISession<DataType> Clone();

        DataType Data { get; set; }
    }

    //public interface ISysInfo
    //{
    //    System.String OS { get; set; }
    //    System.Collections.Generic.List<System.String> Mac { get; set; }
    //    System.String Cpu { get; set; }
    //    System.String Memory { get; set; }
    //    System.String Board { get; set; }
    //    System.String Disk { get; set; }
    //    System.String Browser { get; set; }
    //    System.String Sign { get; set; }
    //}
}
