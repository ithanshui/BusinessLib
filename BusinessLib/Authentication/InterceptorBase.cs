using BusinessLib.Extensions;
using BusinessLib.Business;
using BusinessLib.Mark;
using BusinessLib.Result;
using BusinessLib.Attributes;

namespace BusinessLib.Authentication
{
    public abstract class InterceptorBase : Ninject.Extensions.Interception.IInterceptor
    {
        internal readonly BusinessLib.Log.ILog log;
        internal readonly BusinessLib.Cache.ICache cache;
        internal readonly BusinessLib.Data.IData db;

        public System.Boolean IsLogRecord { get; set; }

        public InterceptorBase(BusinessLib.Log.ILog log = null, BusinessLib.Cache.ICache cache = null, BusinessLib.Data.IData db = null, bool isLogRecord = true)
        {
            this.log = log;
            this.cache = cache;
            this.db = db;
            this.IsLogRecord = isLogRecord;
        }

        public abstract void Intercept(Ninject.Extensions.Interception.IInvocation invocation);

        sealed class CheckAgsObj
        {
            public System.Collections.Generic.List<CheckedAttribute> checkAtts;
            public System.Type type;
            public object value;
        }

        internal virtual Result CheckAgs<Result>(InterceptorMetaData metaData, object arguments)
            where Result : class, IResult, new()
        {
            var checks = new System.Collections.Generic.Dictionary<string, CheckAgsObj>(metaData.CheckedAtts.Count);
            foreach (var item in metaData.CheckedAtts)
            {
                checks.Add(item.Key, new CheckAgsObj { checkAtts = item.Value.Item2, type = item.Value.Item1 });
            }

            //=========================================//
            foreach (var item in metaData.MemberAccessor)
            {
                var name = item.Key;
                var check = checks.ContainsKey(name);
                var trim = metaData.Arguments.Item3.TrimAllChar && typeof(System.String) == item.Value.Item1;

                if (check || trim)
                {
                    var value = item.Value.Item2(arguments);

                    if (check)
                    {
                        checks[name].value = value;
                    }

                    if (trim)
                    {
                        if (null != value)
                        {
                            item.Value.Item3(arguments, System.Convert.ToString(value).Trim());
                        }
                    }
                }
            }

            //check
            foreach (var argument in checks)
            {
                foreach (var item in argument.Value.checkAtts)
                {
                    var result = item.Checked<Result>(argument.Value.type, argument.Value.value, argument.Key);
                    if (null != result) { return result; }
                }
            }

            return null;
        }

        internal static BusinessLib.Authentication.ISession CheckSession<Result>(string token, BusinessLib.Cache.ICache cache, out Result error)
            where Result : class, IResult, new()
        {
            error = null;

            var _token = token.GetToken();
            if (null == _token)
            {
                error = ResultFactory.Create<Result>(MarkEnum.Exp_SessionIllegal); return null;
            }

            var session = null == _token ? null : cache.Get(string.Format("Session_{0}", _token.Key)) as BusinessLib.Authentication.ISession;
            if (null == session)
            {
                error = ResultFactory.Create<Result>(MarkEnum.Exp_SessionOut);
                return null;
            }

            if (!System.Object.Equals(session.IP, _token.IP)) { session.IP = _token.IP; cache.SetSession(session); }// update ip

            return session;
        }

        internal static bool CheckCompetence<Result>(BusinessLib.Authentication.ISession session, BusinessLib.Cache.ICache cache, string competence, out Result error)
            where Result : class, IResult, new()
        {
            error = null;

            var mark = MarkBase.GetObject<string>(MarkEnum.R_SysCompetence);
            if (!cache.Contains(mark))
            {
                error = ResultFactory.Create<Result>(MarkEnum.Exp_CompetenceListNotExist);
                return false;
            }

            var competences = cache.Get<System.Collections.Generic.List<Entity.SysCompetence>>(mark);
            switch (session.Site)
            {
                case "0"://Sys
                    if (null == session.RoleCompetence.Roles //null
                    ||
                    null == session.RoleCompetence.CompetenceAll
                    ||
                    (competences.Exists(c => competence.Equals(c.competence)) //exist
                && !session.RoleCompetence.CompetenceAll.Exists(c => competence.Equals(c.Competence))))//check
                    {
                        error = ResultFactory.Create<Result>(MarkEnum.Exp_CompetenceIllegal);
                        return false;
                    }
                    return true;
                default: return true;
            }
        }
    }
}
