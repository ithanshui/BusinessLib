namespace Business.Authentication
{
    using Result;
    using Business;

    public sealed class InterceptorNot<Result> : InterceptorBase
        where Result : class, IResult, new()
    {
        public InterceptorNot() : base() { }

        public override void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            var startTime = new System.Diagnostics.Stopwatch();
            startTime.Start();
            var meta = this.MetaData[invocation.Method.Name];
            var method = meta.FullName;
            var deserialize = meta.Arguments.Item4;
            var arguments = invocation.Arguments;
            var logType = BusinessLogType.Record;
            var logAttr = meta.BusinessLogAttr;
            IToken token = null;
            var i = meta.Arguments.Item1;

            try
            {
                if (0 == arguments.Length)
                {
                    arguments = null;
                    invocation.ReturnValue = ResultFactory.Create<Result>(Mark.MarkItem.Exp_ArgumentsIllegal);
                    logType = BusinessLogType.Exception;
                    return;
                }

                token = this.Business.GetToken(arguments[0]);
                if (null == token)
                {
                    invocation.ReturnValue = ResultFactory.Create<Result>(Mark.MarkItem.Exp_SessionIllegal);
                    logType = BusinessLogType.Exception;
                    return;
                }

                if (1 < arguments.Length && null != arguments[1])
                {
                    // set ags
                    if (-1 < i && null != deserialize)
                    {
                        arguments[i] = deserialize.Deserialize(arguments[1], meta.Arguments.Item2);

                        #region check ags
                        //check ags
                        var result = CheckAgs<Result>(meta, arguments[i]);
                        if (null != result)
                        {
                            invocation.ReturnValue = result;
                            logType = BusinessLogType.Exception;
                            return;
                        }
                        //check ags end
                        #endregion
                    }
                }

                startTime.Restart();
                invocation.Proceed();

            }
            catch (System.Exception ex)
            {
                logType = BusinessLogType.Exception;
                invocation.ReturnValue = ResultFactory.Create<Result>(0, System.Convert.ToString(ex));
            }
            finally
            {
                startTime.Stop();

                if (-1 < i)
                {
                    arguments[i] = null;
                }

                if (null != this.Business.WriteLogAsync)
                {
                    if (logType == BusinessLogType.Exception)
                    {
                        if (null == token)
                        {
                            this.Business.WriteLogAsync.BeginInvoke(new BusinessLogData(logType, null, null, arguments, invocation.ReturnValue, startTime.Elapsed.TotalSeconds, method, null), null, null);
                        }
                        else
                        {
                            this.Business.WriteLogAsync.BeginInvoke(new BusinessLogData(logType, null, token.Key, arguments, invocation.ReturnValue, startTime.Elapsed.TotalSeconds, method, token.IP), null, null);
                        }
                    }
                    else if (!this.BusinessLogAttr.NotRecord && !logAttr.NotRecord)
                    {
                        if (null == token)
                        {
                            this.Business.WriteLogAsync.BeginInvoke(new BusinessLogData(logType, null, null, (!this.BusinessLogAttr.NotValue && !logAttr.NotValue) ? arguments : null, (!this.BusinessLogAttr.NotResult && !logAttr.NotResult) ? invocation.ReturnValue : null, startTime.Elapsed.TotalSeconds, method, null), null, null);
                        }
                        else
                        {
                            this.Business.WriteLogAsync.BeginInvoke(new BusinessLogData(logType, null, token.Key, (!this.BusinessLogAttr.NotValue && !logAttr.NotValue) ? arguments : null, (!this.BusinessLogAttr.NotResult && !logAttr.NotResult) ? invocation.ReturnValue : null, startTime.Elapsed.TotalSeconds, method, token.IP), null, null);
                        }
                    }
                }
                //if (this.IsLogRecord || logType == Log.LogType.Exception)
                //{
                //    if (null == token)
                //    {
                //        log.WriteAsync(logType, null, null, arguments, invocation.ReturnValue, startTime.Elapsed.TotalSeconds, method, null);
                //    }
                //    else
                //    {
                //        log.WriteAsync(logType, null, token.Site, arguments, invocation.ReturnValue, startTime.Elapsed.TotalSeconds, method, token.IP);
                //    }
                //}
                //else
                //{
                //    if (null == token)
                //    {
                //        log.WriteAsync(logType, null, null, null, null, startTime.Elapsed.TotalSeconds, method, null);
                //    }
                //    else
                //    {
                //        log.WriteAsync(logType, null, token.Site, null, null, startTime.Elapsed.TotalSeconds, method, token.IP);
                //    }
                //}
            }
        }
    }
}
