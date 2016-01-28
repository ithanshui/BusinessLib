namespace Business.Auth
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
}
