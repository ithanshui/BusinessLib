using BusinessLib.Extensions;
using BusinessLib.Business;
using BusinessLib.Mark;
using BusinessLib.Result;
using BusinessLib.Attributes;
namespace BusinessLib.BasicAuthentication
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

        public static BusinessLib.BasicAuthentication.ISession CheckSession(string token, BusinessLib.Cache.ICache cache, out IResult<object> error)
        {
            error = null;

            var _token = token.GetToken();
            if (null == _token)
            {
                error = ResultExtensions.Result(MarkEnum.Exp_SessionIllegal); return null;
            }

            var session = null == _token ? null : cache.Get(string.Format("Session_{0}", _token.Key)) as BusinessLib.BasicAuthentication.ISession;
            if (null == session)
            {
                error = ResultExtensions.Result(MarkEnum.Exp_SessionOut);
                return null;
            }

            if (!System.Object.Equals(session.IP, _token.IP)) { session.IP = _token.IP; cache.SetSession(session); }// update ip

            return session;
        }

        public static bool CheckCompetence(BusinessLib.BasicAuthentication.ISession session, BusinessLib.Cache.ICache cache, string competence, out IResult<object> error)
        {
            error = null;

            var mark = MarkBase.GetObject<string>(MarkEnum.R_SysCompetence);
            if (!cache.Contains(mark))
            {
                error = ResultExtensions.Result(MarkEnum.Exp_CompetenceListNotExist);
                return false;
            }

            var competences = cache.Get<System.Collections.Generic.List<Entity.SysCompetence>>(mark);
            switch (session.Site)
            {
                case "0"://SYS
                    if (null == session.RoleCompetence.Roles //null
                    ||
                    null == session.RoleCompetence.CompetenceAll
                    ||
                    (competences.Exists(c => competence.Equals(c.competence)) //exist
                && !session.RoleCompetence.CompetenceAll.Exists(c => competence.Equals(c.Competence))))//check
                    {
                        error = ResultExtensions.Result(MarkEnum.Exp_CompetenceIllegal);
                        return false;
                    }
                    return true;
                default: return true;
            }
        }

        sealed class SCheckAgs
        {
            public ArgumentAttribute[] Atts;
            public System.Type Type;
            public object Value;
        }

        internal static BusinessLib.Result.IResult<object> CheckAgsJson(System.Type parameterType, Attributes.ArgumentsAttribute attrs, string ags, object arguments)
        {
            var fields = parameterType.GetFields();
            var propertys = parameterType.GetProperties();

            var checks = new System.Collections.Generic.Dictionary<string, SCheckAgs>(fields.Length + propertys.Length);
            //add all name
            foreach (var item in fields)
            {
                checks.Add(item.Name, new SCheckAgs { Atts = item.GetCustomAttributes(typeof(ArgumentAttribute), true) as ArgumentAttribute[], Type = item.FieldType });
            }
            foreach (var item in propertys)
            {
                checks.Add(item.Name, new SCheckAgs { Atts = item.GetCustomAttributes(typeof(ArgumentAttribute), true) as ArgumentAttribute[], Type = item.PropertyType });
            }

            var values = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(ags).Children<Newtonsoft.Json.Linq.JProperty>();
            //add value
            foreach (var item in values)
            {
                var name = item.Name;
                if (!checks.ContainsKey(name)) { continue; }
                checks[name].Value = item.Value.ToObject(checks[name].Type);
                //set value
                if (attrs.TrimChar && typeof(System.String) == checks[name].Type && null != checks[name].Value)
                {
                    var field = System.Array.Find(fields, c => name == c.Name);
                    if (null != field)
                    {
                        //set value
                        field.SetValue(arguments, System.Convert.ToString(checks[name].Value).Trim());
                    }
                    else
                    {
                        var property = System.Array.Find(propertys, c => name == c.Name);
                        if (null != property)
                        {
                            //set value
                            property.SetValue(arguments, System.Convert.ToString(checks[name].Value).Trim());
                        }
                        else { throw new System.Exception(string.Format("GetCheckAgs excep! argument {0} Can't find", name)); }
                    }
                }
            }

            //check
            foreach (var argument in checks)
            {
                if (null == argument.Value.Atts) { continue; }

                foreach (var item in argument.Value.Atts)
                {
                    var result = item.Checked(argument.Value.Type, argument.Value.Value, argument.Key);
                    if (null != result) { return result; }
                }
            }

            return null;
        }
    }
}
