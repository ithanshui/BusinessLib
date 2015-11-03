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

        /*
        public void Intercept(Ninject.Extensions.Interception.IInvocation invocation)
        {
            var arguments = invocation.Request.Arguments;

            if (null == arguments || null == arguments[0])
            {
                invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_ArgumentsIllegal).ToString();
                return;
            }

            var method = invocation.Request.Method.GetMethodFullName();

            try
            {
                var token = System.Convert.ToString(arguments[0]).GetTokenNot();
                if (null == token)
                {
                    invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_SessionIllegal).ToString();
                    return;
                }

                //if (!cache.Contains(token.Site))
                //{
                //    invocation.ReturnValue = new ResultMark(MarkEnum.Exp_SiteIllegal).ToString();
                //    return;
                //}

                var i = -1;
                if (1 < arguments.Length)
                {
                    var agsType = invocation.Request.Method.GetParameters();
                    var ags = null == arguments[1] ? System.String.Empty : System.Convert.ToString(arguments[1]);
                    if (!System.String.IsNullOrEmpty(ags))
                    {
                        // set ags
                        i = System.Array.FindIndex(agsType, p => 0 < p.ParameterType.GetCustomAttributes(typeof(Attributes.ArgumentsAttribute), false).Length);
                        if (-1 < i)
                        {
                            arguments[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(ags, agsType[i].ParameterType);

                            #region check ags
                            //check ags
                            var result = BusinessLib.Attributes.AttributesHelp.CheckAgsJson(agsType[i].ParameterType, ags, arguments[i]);
                            if (null != result)
                            {
                                invocation.ReturnValue = result.ToString();
                                log.WriteAsync(BusinessLib.Log.LogType.Exception, System.String.Empty, token.Site, arguments, invocation.ReturnValue, System.DateTime.Now.Subtract(System.DateTime.Now).TotalSeconds, method, token.IP);
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
                    if (-1 < i)
                    {
                        arguments[i] = null;
                    }

                    if (this.IsLogRecord || logType == Log.LogType.Exception)
                    {
                        log.WriteAsync(logType, System.String.Empty, token.Site, arguments, invocation.ReturnValue, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, token.IP);
                    }
                    else
                    {
                        log.WriteAsync(logType, System.String.Empty, token.Site, null, null, System.DateTime.Now.Subtract(startTime).TotalSeconds, method, token.IP);
                    }
                }
            }
            catch (System.Exception ex)
            {
                invocation.ReturnValue = ResultExtensions.Result(0, System.Convert.ToString(ex)).ToString();
            }
        }
        */

        public override void Intercept(Ninject.Extensions.Interception.IInvocation invocation)
        {
            var startTime = System.DateTime.Now;
            var method = invocation.Request.Method.GetMethodFullName();
            var agsType = invocation.Request.Method.GetParameters();
            var arguments = invocation.Request.Arguments;
            var logType = BusinessLib.Log.LogType.Record;
            ITokenNot token = null;
            Attributes.ArgumentsAttribute attrs = null;
            var i = -1;

            try
            {
                for (int i2 = 0; i2 < agsType.Length; i2++)
                {
                    var argumentsAttrs = agsType[i2].ParameterType.GetCustomAttributes(typeof(Attributes.ArgumentsAttribute), true);
                    if (0 < argumentsAttrs.Length) { i = i2; attrs = argumentsAttrs[0] as Attributes.ArgumentsAttribute; break; }
                }

                if (0 == arguments.Length)
                {
                    arguments = null;
                    invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_ArgumentsIllegal).ToString();
                    logType = BusinessLib.Log.LogType.Exception;
                    return;
                }

                token = System.Convert.ToString(arguments[0]).GetTokenNot();
                if (null == token)
                {
                    invocation.ReturnValue = ResultExtensions.Result(MarkEnum.Exp_SessionIllegal).ToString();
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
                            arguments[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(ags, agsType[i].ParameterType);

                            #region check ags
                            //check ags
                            var result = CheckAgsJson(agsType[i].ParameterType, attrs, ags, arguments[i]);
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

                startTime = System.DateTime.Now;
                invocation.Proceed();

            }
            catch (System.Exception ex)
            {
                logType = BusinessLib.Log.LogType.Exception;
                invocation.ReturnValue = ResultExtensions.Result(0, System.Convert.ToString(ex)).ToString();
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
