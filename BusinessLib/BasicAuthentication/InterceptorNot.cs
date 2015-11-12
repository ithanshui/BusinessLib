namespace BusinessLib.BasicAuthentication
{
    using BusinessLib.Extensions;
    using BusinessLib.Business;
    using BusinessLib.Result;
    using BusinessLib.Mark;

    public class InterceptorNot : InterceptorBase
    {
        public InterceptorNot(BusinessLib.Log.ILog log, BusinessLib.Cache.ICache cache, bool isLogRecord = true)
            : base(log, cache, isLogRecord: isLogRecord) { }

        public override void Intercept(Ninject.Extensions.Interception.IInvocation invocation)
        {
            var startTime = System.DateTime.Now;
            var meta = this.MetaData[invocation.Request.Method.Name];
            var method = meta.FullName;
            var arguments = invocation.Request.Arguments;
            var logType = BusinessLib.Log.LogType.Record;
            ITokenNot token = null;
            var i = meta.Arguments.Item1;

            try
            {
                if (0 == arguments.Length)
                {
                    arguments = null;
                    invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_ArgumentsIllegal);
                    logType = BusinessLib.Log.LogType.Exception;
                    return;
                }

                token = System.Convert.ToString(arguments[0]).GetTokenNot();
                if (null == token)
                {
                    invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_SessionIllegal);
                    logType = BusinessLib.Log.LogType.Exception;
                    return;
                }

                //if (!cache.Contains(token.Site))
                //{
                //    invocation.ReturnValue = new ResultMark(MarkEnum.Exp_SiteIllegal).ToString();
                //    return;
                //}

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

                startTime = System.DateTime.Now;
                invocation.Proceed();

            }
            catch (System.Exception ex)
            {
                logType = BusinessLib.Log.LogType.Exception;
                invocation.ReturnValue = ResultExtensions.Result(0, System.Convert.ToString(ex));
            }
            finally
            {
                if (null != log)
                {
                    if (-1 < i)
                    {
                        arguments[i] = null;
                    }

                    if (this.IsLogRecord || logType == Log.LogType.Exception)
                    {
                        if (null == token)
                        {
                            log.WriteAsync(logType, System.String.Empty, null, arguments, invocation.ReturnValue, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, null);
                        }
                        else
                        {
                            log.WriteAsync(logType, System.String.Empty, token.Site, arguments, invocation.ReturnValue, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, token.IP);
                        }
                    }
                    else
                    {
                        if (null == token)
                        {
                            log.WriteAsync(logType, System.String.Empty, null, null, null, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, null);
                        }
                        else
                        {
                            log.WriteAsync(logType, System.String.Empty, token.Site, null, null, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, token.IP);
                        }
                    }
                }
            }
        }
    }
}
