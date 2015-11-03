namespace BusinessLib.BasicAuthentication
{
    using BusinessLib.Extensions;
    using BusinessLib.Result;
    using BusinessLib.Mark;

    //public interface IIntercept
    //{
    //    void Intercept(System.Reflection.MethodInfo method, System.Reflection.ParameterInfo[] agsInfo, object[] ags);
    //}

    //public abstract class dd : IIntercept
    //{
    //    public void Intercept(System.Reflection.MethodInfo method, System.Reflection.ParameterInfo[] agsInfo, object[] ags)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}

    public sealed class Interceptor : InterceptorBase
    {
        public Interceptor(BusinessLib.Log.ILog log, BusinessLib.Cache.ICache cache, bool isLogRecord = true)
            : base(log, cache, isLogRecord: isLogRecord) { }

        /*
        public void Intercept(Ninject.Extensions.Interception.IInvocation invocation)
        {
            var arguments = invocation.Request.Arguments;

            if (null == arguments || null == arguments[0])
            {
                invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_ArgumentsIllegal).ToString();
                return;
            }

            IResult<object> error;

            var session = Interceptor.CheckSession(System.Convert.ToString(arguments[0]), cache, out error);

            if (null == session)
            {
                invocation.ReturnValue = error.ToString(); return;
            }

            var method = invocation.Request.Method.GetMethodFullName();
            // check competence
            if (!Interceptor.CheckCompetence(session, cache, method, out error))
            {
                invocation.ReturnValue = error.ToString(); return;
            }

            var agsType = invocation.Request.Method.GetParameters();
            // set session
            var i1 = System.Array.FindIndex(agsType, p => p.ParameterType.Equals(typeof(BusinessLib.BasicAuthentication.ISession)));
            if (-1 < i1)
            {
                arguments[i1] = session;
            }

            var i2 = -1;
            if (1 < arguments.Length)
            {
                var ags = null == arguments[1] ? System.String.Empty : System.Convert.ToString(arguments[1]);
                if (!System.String.IsNullOrEmpty(ags))
                {
                    // set ags
                    i2 = System.Array.FindIndex(agsType, p => 0 < p.ParameterType.GetCustomAttributes(typeof(Attributes.ArgumentsAttribute), true).Length);
                    if (-1 < i2)
                    {
                        arguments[i2] = Newtonsoft.Json.JsonConvert.DeserializeObject(ags, agsType[i2].ParameterType);

                        #region check ags
                        //check ags
                        var result = BusinessLib.Attributes.AttributesHelp.CheckAgsJson(agsType[i2].ParameterType, ags, arguments[i2]);
                        if (null != result)
                        {
                            invocation.ReturnValue = result.ToString();
                            log.WriteAsync(BusinessLib.Log.LogType.Exception, session.Key, session.Account, arguments, invocation.ReturnValue, System.DateTime.Now.Subtract(System.DateTime.Now).TotalSeconds, method, session.IP);
                            return;
                        }
                        //check ags end
                        #endregion
                    }
                }
            }

            var logType = BusinessLib.Log.LogType.Record;
            var startTime = System.DateTime.Now;

            try
            {
                invocation.Proceed();
            }
            catch (System.Exception ex)
            {
                logType = BusinessLib.Log.LogType.Exception;
                invocation.ReturnValue = ResultExtensions.Result(0, System.Convert.ToString(ex)).ToString();
            }
            finally
            {
                //remove roleCompetence
                //var _session = session.Clone();
                //_session.RoleCompetence = default(RoleCompetence);
                //arguments[i] = _session;

                if (-1 < i1)
                {
                    arguments[i1] = null;
                }
                if (-1 < i2)
                {
                    arguments[i2] = null;
                }

                if (this.IsLogRecord || logType == Log.LogType.Exception)
                {
                    log.WriteAsync(logType, session.Key, session.Account, arguments, invocation.ReturnValue, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, session.IP);
                }
                else
                {
                    log.WriteAsync(logType, session.Key, session.Account, null, null, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, session.IP);
                }
            }
        }
        */

        public override void Intercept(Ninject.Extensions.Interception.IInvocation invocation)
        {
            var startTime = System.DateTime.Now;
            var method = invocation.Request.Method.GetMethodFullName();
            var agsInfo = invocation.Request.Method.GetParameters();
            var arguments = invocation.Request.Arguments;
            var logType = BusinessLib.Log.LogType.Record;
            ISession session = null;
            Attributes.ArgumentsAttribute attrs = null;
            var i = -1;
            var i1 = -1;

            try
            {
                for (int i2 = 0; i2 < agsInfo.Length; i2++)
                {
                    var argumentsAttrs = agsInfo[i2].ParameterType.GetCustomAttributes(typeof(Attributes.ArgumentsAttribute), true);
                    if (0 < argumentsAttrs.Length) { i = i2; attrs = argumentsAttrs[0] as Attributes.ArgumentsAttribute; break; }
                }

                if (0 == arguments.Length)
                {
                    arguments = null;
                    invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_ArgumentsIllegal).ToString(); logType = BusinessLib.Log.LogType.Exception; return;
                }

                IResult<object> error;

                session = Interceptor.CheckSession(System.Convert.ToString(arguments[0]), cache, out error);

                if (null == session)
                {
                    invocation.ReturnValue = error.ToString(); logType = BusinessLib.Log.LogType.Exception; return;
                }

                // check competence
                if (!Interceptor.CheckCompetence(session, cache, method, out error))
                {
                    invocation.ReturnValue = error.ToString(); logType = BusinessLib.Log.LogType.Exception; return;
                }

                // set session
                i1 = System.Array.FindIndex(agsInfo, p => p.ParameterType.Equals(typeof(BusinessLib.BasicAuthentication.ISession)));
                if (-1 < i1)
                {
                    arguments[i1] = session;
                }

                if (1 < arguments.Length)
                {
                    var ags = null == arguments[1] ? System.String.Empty : System.Convert.ToString(arguments[1]);
                    if (!System.String.IsNullOrEmpty(ags))
                    {
                        // set ags
                        if (-1 < i)
                        {
                            arguments[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(ags, agsInfo[i].ParameterType);

                            #region check ags
                            //check ags
                            var result = CheckAgsJson(agsInfo[i].ParameterType, attrs, ags, arguments[i]);
                            if (null != result)
                            {
                                invocation.ReturnValue = result.ToString();
                                logType = BusinessLib.Log.LogType.Exception;
                                return;
                            }
                            //check ags end
                            #endregion
                        }
                    }
                }

                //===============================//
                startTime = System.DateTime.Now;
                invocation.Proceed();
            }
            catch (System.Exception ex)
            {
                invocation.ReturnValue = ResultExtensions.Result(0, System.Convert.ToString(ex)).ToString();
                logType = BusinessLib.Log.LogType.Exception;
            }
            finally
            {
                if (null != log)
                {
                    if (-1 < i1)
                    {
                        arguments[i1] = null;
                    }
                    if (-1 < i)
                    {
                        arguments[i] = null;
                    }

                    //remove roleCompetence
                    //var _session = session.Clone();
                    //_session.RoleCompetence = default(RoleCompetence);
                    //arguments[i] = _session;

                    if (this.IsLogRecord || logType == Log.LogType.Exception)
                    {
                        if (null == session)
                        {
                            log.WriteAsync(logType, null, null, arguments, invocation.ReturnValue, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, null);
                        }
                        else
                        {
                            log.WriteAsync(logType, session.Key, session.Account, arguments, invocation.ReturnValue, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, session.IP);
                        }
                    }
                    else
                    {
                        if (null == session)
                        {
                            log.WriteAsync(logType, null, null, null, null, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, null);
                        }
                        else
                        {
                            log.WriteAsync(logType, session.Key, session.Account, null, null, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, session.IP);
                        }
                    }
                }
            }
        }
    }
}
