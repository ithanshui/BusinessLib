using BusinessLib.Attributes;
using BusinessLib.Entity;
using BusinessLib.Mark;
using System;
using System.Linq;

namespace BusinessLib.Business
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
        static readonly TimeSpan TimerSpan = System.TimeSpan.FromMinutes(5);

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

        public static void ChangeTimer(TimeSpan timerSpan)
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

    public abstract class BusinessBase<IData, ILog, ICache> : BusinessBase, System.IDisposable
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

        void GetRoleCompetence()
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
        }

        System.Boolean isRoleCompetence;
        public System.Boolean IsRoleCompetence
        {
            get { return isRoleCompetence; }
            set
            {
                var _value = value;

                if (null == db || null == cache) { return; }

                if (isRoleCompetence == _value) { return; }

                var mark = MarkBase.GetObject<string>(MarkEnum.Sys_Action);
                if (_value)
                {
                    GetRoleCompetence();

                    BusinessBase.AddAction(mark, () =>
                    {
                        GetRoleCompetence();
                    });
                }
                else
                {
                    System.Action action;
                    BusinessBase.RemoveAction(mark, out action);
                }
                isRoleCompetence = _value;
            }
        }

        public BusinessBase(params System.Action[] timerActions)
        {
            if (0 == timerActions.Length) { return; }
            //DynamicProxy
            if (this.GetType().Namespace.Equals("Castle.Proxies")) { return; }

            foreach (var item in timerActions)
            {
                var name = string.Format("{0}.{1}", item.Method.DeclaringType.FullName, item.Method.Name);
                BusinessBase.AddAction(name, item);
            }
        }

        public BusinessBase(IData db, ILog log, ICache cache, params System.Action[] timerActions)
            : this(timerActions)
        {
            this.db = db;
            this.log = log;
            this.cache = cache;
        }

        public override void Dispose()
        {
            base.Dispose();
            //foreach (var item in TimerActions.Where(m => this.Guid == m.Item1).ToArray())
            //{
            //    TimerActions.Remove(item);
            //}

            #region interface

            if (null != this.db)
            {
                var disp = this.db as System.IDisposable;

                if (null != disp)
                {
                    disp.Dispose();
                }
            }

            if (null != this.log)
            {
                var disp = this.log as System.IDisposable;

                if (null != disp)
                {
                    disp.Dispose();
                }
            }

            if (null != this.cache)
            {
                var disp = this.cache as System.IDisposable;

                if (null != disp)
                {
                    disp.Dispose();
                }
            }

            #endregion
        }
    }
}