namespace Business
{
    public interface IBusiness
    {
        string Login(string value, out string error, string commandId = null);

        Auth.IToken GetToken(object token);

        Session GetSession<Session>(Auth.IToken token)
            where Session : class, Auth.ISession;

        bool CheckCompetence(Auth.ISession session, string competence);

        System.Action<BusinessLogData> WriteLogAsync { get; set; }
    }
}
