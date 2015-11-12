namespace BusinessLib.BasicAuthentication
{
    using BusinessLib.Extensions;
    using BusinessLib.Result;
    using BusinessLib.Mark;

    public sealed class Interceptor : InterceptorBase
    {
        public Interceptor(BusinessLib.Log.ILog log, BusinessLib.Cache.ICache cache, bool isLogRecord = true)
            : base(log, cache, isLogRecord: isLogRecord) { }

        public override void Intercept(Ninject.Extensions.Interception.IInvocation invocation)
        {
            var startTime = System.DateTime.Now;
            var meta = this.MetaData[invocation.Request.Method.Name];
            var method = meta.FullName;
            var arguments = invocation.Request.Arguments;
            var logType = BusinessLib.Log.LogType.Record;
            ISession session = null;
            var i = meta.Arguments.Item1;
            var i1 = meta.SessionPosition;

            try
            {
                if (0 == arguments.Length)
                {
                    arguments = null;
                    invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_ArgumentsIllegal); logType = BusinessLib.Log.LogType.Exception; return;
                }

                IResult error;

                session = Interceptor.CheckSession(System.Convert.ToString(arguments[0]), cache, out error);

                if (null == session)
                {
                    invocation.ReturnValue = error; logType = BusinessLib.Log.LogType.Exception; return;
                }

                // check competence
                if (!Interceptor.CheckCompetence(session, cache, method, out error))
                {
                    invocation.ReturnValue = error; logType = BusinessLib.Log.LogType.Exception; return;
                }

                // set session
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
                            arguments[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(ags, meta.Arguments.Item2);

                            #region check ags
                            //check ags
                            var result = CheckAgsJson(meta, arguments[i]);
                            if (null != result)
                            {
                                invocation.ReturnValue = result;
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
                invocation.ReturnValue = ResultExtensions.Result(0, System.Convert.ToString(ex));
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