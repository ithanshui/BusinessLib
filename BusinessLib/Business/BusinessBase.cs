using BusinessLib.Entity;
using BusinessLib.Mark;
using System;
using System.Linq;

namespace BusinessLib.Business
{
    public abstract class BusinessBase<IData, ILog, ICache> : System.IDisposable
        where IData : class, BusinessLib.Data.IData
        where ILog : class, BusinessLib.Log.ILog
        where ICache : class, BusinessLib.Cache.ICache
    {
        readonly IData db;
        readonly ILog log;
        readonly ICache cache;

        public IData DB { get { return db; } }

        public ILog Log { get { return log; } }

        public ICache Cache { get { return cache; } }

        System.Boolean isRoleCompetence;
        public System.Boolean IsRoleCompetence
        {
            get { return isRoleCompetence; }
            set
            {
                if (null == db || null == cache) { return; }

                if (isRoleCompetence == value) { return; }

                var mark = MarkBase.GetObject<string>(MarkEnum.Sys_Action);
                if (value)
                {
                    if (0 == TimerActions.Count(m => mark == m.Item1))
                    {
                        TimerActions.Add(new System.Tuple<string, Action>(mark, () =>
                        {
                            using (var con = db.GetConnection())
                            {
                                //-r
                                cache.Set(MarkBase.GetObject<string>(MarkEnum.R_SysAccount), con.Entity.Get<SysAccount>().ToList());
                                cache.Set(MarkBase.GetObject<string>(MarkEnum.R_SysAccount_Role), con.Entity.Get<SysAccount_Role>().ToList());
                                cache.Set(MarkBase.GetObject<string>(MarkEnum.R_SysRole), con.Entity.Get<SysRole>().ToList());
                                cache.Set(MarkBase.GetObject<string>(MarkEnum.R_SysRole_Competence), con.Entity.Get<SysRole_Competence>().ToList());
                                cache.Set(MarkBase.GetObject<string>(MarkEnum.R_SysCompetence), con.Entity.Get<SysCompetence>().ToList());
                                //config -c
                                cache.Set(MarkBase.GetObject<string>(MarkEnum.C_SysConfig), con.Entity.Get<SysConfig>().ToList());
                            }
                        }));
                    }
                }
                else
                {
                    var sys_Action = TimerActions.FirstOrDefault(m => mark == m.Item1);
                    if (!(default(Tuple<string, Action>) == sys_Action))
                    {
                        TimerActions.Remove(sys_Action);
                    }
                }
                isRoleCompetence = value;
            }
        }

        internal static readonly System.Collections.ObjectModel.ObservableCollection<System.Tuple<System.String, System.Action>> TimerActions = new System.Collections.ObjectModel.ObservableCollection<System.Tuple<System.String, System.Action>>();

        static System.Threading.Timer Timer;
        /// <summary>
        /// Minutes 5
        /// </summary>
        static readonly TimeSpan TimerSpan = System.TimeSpan.FromMinutes(5);

        static BusinessBase()
        {
            TimerActions.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (System.Tuple<System.String, System.Action> item in e.NewItems)
                        {
                            item.Item2.Invoke();
                        }

                        if (null == Timer)
                        {
                            Timer = new System.Threading.Timer(new System.Threading.TimerCallback((obj) =>
                            {
                                for (int i = TimerActions.Count - 1; i >= 0; i--)
                                {
                                    if (i >= TimerActions.Count) { continue; }
                                    try
                                    {
                                        TimerActions[i].Item2.BeginInvoke((obj1) => { }, null);
                                    }
                                    catch { }
                                }
                            }), null, TimerSpan, TimerSpan);
                        }
                        break;
                }
            };
        }

        public BusinessBase(params System.Action[] timerActions)
        {
            if (0 == timerActions.Length) { return; }
            //DynamicProxy
            if (this.GetType().Namespace.Equals("Castle.Proxies")) { return; }

            foreach (var item in timerActions)
            {
                var name = string.Format("{0}.{1}", item.Method.DeclaringType.FullName, item.Method.Name);
                if (0 == TimerActions.Count(m => name == m.Item1))
                {
                    TimerActions.Add(new System.Tuple<string, Action>(name, item));
                }
            }
        }

        public BusinessBase(IData db = null, ILog log = null, ICache cache = null, params System.Action[] timerActions)
            : this(timerActions)
        {
            this.db = db;
            this.log = log;
            this.cache = cache;
        }

        public void Dispose()
        {
            //foreach (var item in TimerActions.Where(m => this.Guid == m.Item1).ToArray())
            //{
            //    TimerActions.Remove(item);
            //}

            if (0 == TimerActions.Count)
            {
                Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                Timer.Dispose();
                Timer = null;
            }

            #region interface

            if (null != this.db)
            {
                var disp = this.db.GetType().GetInterface("System.IDisposable");

                if (null != disp)
                {
                    ((IDisposable)this.db).Dispose();
                }
            }

            if (null != this.log)
            {
                var disp = this.log.GetType().GetInterface("System.IDisposable");

                if (null != disp)
                {
                    ((IDisposable)this.log).Dispose();
                }
            }

            if (null != this.cache)
            {
                var disp = this.cache.GetType().GetInterface("System.IDisposable");

                if (null != disp)
                {
                    ((IDisposable)this.cache).Dispose();
                }
            }

            #endregion
        }
    }
}