namespace Business.Authentication
{
    using Business;
    using Result;

    public sealed class Interceptor<Result, Session> : InterceptorBase
        where Result : class, IResult, new()
        where Session : class, ISession
    {
        public Interceptor() : base() { }

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
            Session session = null;
            var i = meta.Arguments.Item1;
            var i1 = meta.SessionPosition;

            try
            {
                if (0 == arguments.Length)
                {
                    arguments = null;

                    invocation.ReturnValue = ResultFactory.Create<Result>(Mark.MarkItem.Exp_ArgumentsIllegal); logType = BusinessLogType.Exception; return;
                }

                var token = this.Business.GetToken(arguments[0]);
                if (null == token)
                {
                    invocation.ReturnValue = ResultFactory.Create<Result>(Mark.MarkItem.Exp_SessionIllegal); logType = BusinessLogType.Exception; return;
                }

                session = this.Business.GetSession<Session>(token);
                if (null == session)
                {
                    invocation.ReturnValue = ResultFactory.Create<Result>(Mark.MarkItem.Exp_SessionOut); logType = BusinessLogType.Exception; return;
                }

                // check competence
                if (!this.Business.CheckCompetence(session, method))
                {
                    invocation.ReturnValue = ResultFactory.Create<Result>(Mark.MarkItem.Exp_CompetenceIllegal); logType = BusinessLogType.Exception; return;
                }

                // set session
                if (-1 < i1)
                {
                    arguments[i1] = session;
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

                //===============================//
                startTime.Restart();
                invocation.Proceed();
            }
            catch (System.Exception ex)
            {
                invocation.ReturnValue = ResultFactory.Create<Result>(0, System.Convert.ToString(ex));
                logType = BusinessLogType.Exception;
            }
            finally
            {
                startTime.Stop();

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

                if (null != this.Business.WriteLogAsync)
                {
                    if (logType == BusinessLogType.Exception)
                    {
                        if (null == session)
                        {
                            this.Business.WriteLogAsync.BeginInvoke(new BusinessLogData(logType, null, null, arguments, invocation.ReturnValue, startTime.Elapsed.TotalSeconds, method, null), null, null);
                        }
                        else
                        {
                            this.Business.WriteLogAsync.BeginInvoke(new BusinessLogData(logType, session.Key, session.Account, arguments, invocation.ReturnValue, startTime.Elapsed.TotalSeconds, method, session.IP), null, null);
                        }
                    }
                    else if (!this.BusinessLogAttr.NotRecord && !logAttr.NotRecord)
                    {
                        if (null == session)
                        {
                            this.Business.WriteLogAsync.BeginInvoke(new BusinessLogData(logType, null, null, (!this.BusinessLogAttr.NotValue && !logAttr.NotValue) ? arguments : null, (!this.BusinessLogAttr.NotResult && !logAttr.NotResult) ? invocation.ReturnValue : null, startTime.Elapsed.TotalSeconds, method, null), null, null);
                        }
                        else
                        {
                            this.Business.WriteLogAsync.BeginInvoke(new BusinessLogData(logType, session.Key, session.Account, (!this.BusinessLogAttr.NotValue && !logAttr.NotValue) ? arguments : null, (!this.BusinessLogAttr.NotResult && !logAttr.NotResult) ? invocation.ReturnValue : null, startTime.Elapsed.TotalSeconds, method, session.IP), null, null);
                        }
                    }
                }
            }
        }
    }
}