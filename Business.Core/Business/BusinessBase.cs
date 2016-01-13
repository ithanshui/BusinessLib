namespace Business
{
    public abstract class BusinessBase : System.IDisposable
    {
        static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Action> TimerActions = new System.Collections.Concurrent.ConcurrentDictionary<string, System.Action>();

        static System.Threading.Timer Timer;

        static BusinessBase()
        {
            Timer = new System.Threading.Timer(new System.Threading.TimerCallback((obj) =>
            {
                foreach (var item in TimerActions.Values)
                {
                    try
                    {
                        item.BeginInvoke((obj1) => { }, null);
                    }
                    catch { }
                }
            }), null, TimerSpan, TimerSpan);
        }

        /// <summary>
        /// Minutes 5
        /// </summary>
        static readonly System.TimeSpan TimerSpan = System.TimeSpan.FromMinutes(5);

        public static bool AddAction(string key, System.Action action)
        {
            return TimerActions.TryAdd(key, action);
        }

        public static System.Action SetAction(string key, System.Action action)
        {
            return TimerActions.AddOrUpdate(key, action, (key1, value) => action);
        }

        public static bool RemoveAction(string key, out System.Action action)
        {
            return TimerActions.TryRemove(key, out action);
        }

        public static void ChangeTimer(System.TimeSpan timerSpan)
        {
            Timer.Change(timerSpan, timerSpan);
        }

        public virtual void Dispose()
        {
            //if (0 == TimerActions.Count)
            //{
            //    Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            //    Timer.Dispose();
            //    Timer = null;
            //}
        }
    }

    public abstract class BusinessBase<IData, ILog, ICache> : BusinessBase, IBusiness
        where IData : class, Data.IData
        where ILog : class, Log.ILog
        where ICache : class, Cache.ICache
    {
        readonly IData db;
        readonly ILog log;
        readonly ICache cache;

        public IData DB { get { return db; } }

        public ILog Log { get { return log; } }

        public ICache Cache { get { return cache; } }

        public BusinessBase(params System.Action[] timerActions)
        {
            if (0 == timerActions.Length) { return; }
            //DynamicProxy
            if (this.GetType().Namespace.Equals("Castle.Proxies")) { return; }

            foreach (var item in timerActions)
            {
                BusinessBase.AddAction(Extensions.Help.GetMethodFullName(item.Method), item);
            }
        }

        public BusinessBase(IData db, ILog log, ICache cache, params System.Action[] timerActions)
            : this(timerActions)
        {
            this.db = db;
            this.log = log;
            this.cache = cache;
        }

        [Attributes.NotIntercept]
        public virtual string Login(string value, out string error)
        {
            throw new System.NotImplementedException();
        }

        [Attributes.NotIntercept]
        public virtual Authentication.IToken GetToken(object token)
        {
            if (null == token) { return null; }

            var _token = Extensions.Help.JsonDeserialize<Authentication.Token>(System.Convert.ToString(token));
            if (null == _token) { return null; }

            return _token;
        }

        [Attributes.NotIntercept]
        public virtual Session GetSession<Session>(Authentication.IToken token)
            where Session : class, Authentication.ISession
        {
            if (null == token) { return null; }

            var cacheValue = this.Cache.Get(string.Format("Session_{0}", token.Key));
            if (!cacheValue.HasValue) { return null; }

            var session = Extensions.Help.ProtoBufDeserialize<Session>(cacheValue);

            if (!System.Object.Equals(session.IP, token.IP))
            {
                session.IP = token.IP; this.Cache.Set(session.Key, session.ToBytes());
            }

            return session;
        }

        [Attributes.NotIntercept]
        public virtual bool CheckCompetence(Authentication.ISession session, string competence)
        {
            return !(null != session.Competences && !session.Competences.Contains(competence));
        }

        public System.Action<BusinessLogData> WriteLogAsync { get; set; }

        [Attributes.NotIntercept]
        public override void Dispose()
        {
            base.Dispose();
            //foreach (var item in TimerActions.Where(m => this.Guid == m.Item1).ToArray())
            //{
            //    TimerActions.Remove(item);
            //}

            #region interface

            if (null != this.db && typeof(System.IDisposable).IsAssignableFrom(this.db.GetType()))
            {
                ((System.IDisposable)this.db).Dispose();
            }

            if (null != this.log && typeof(System.IDisposable).IsAssignableFrom(this.log.GetType()))
            {
                ((System.IDisposable)this.log).Dispose();
            }

            if (null != this.cache && typeof(System.IDisposable).IsAssignableFrom(this.cache.GetType()))
            {
                ((System.IDisposable)this.cache).Dispose();
            }

            #endregion
        }
    }
}