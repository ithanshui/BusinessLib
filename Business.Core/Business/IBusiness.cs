namespace Business
{
    public interface IBusiness
    {
        string Login(string value, out string error);

        Authentication.IToken GetToken(object token);

        Session GetSession<Session>(Authentication.IToken token)
            where Session : class, Authentication.ISession;

        bool CheckCompetence(Authentication.ISession session, string competence);

        System.Action<BusinessLogData> WriteLogAsync { get; set; }
    }
}
