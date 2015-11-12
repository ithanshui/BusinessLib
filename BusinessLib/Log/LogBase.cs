using System.Linq;
using BusinessLib.Entity;

namespace BusinessLib.Log
{
    /*
    public class LogBase<IData, ICache> : LogBase
        where IData : class, BusinessLib.Data.IData, new()
        where ICache : class, BusinessLib.Cache.ICache, new()
    {
        static IData db = new IData();
        static ICache cache = new ICache();

        public System.Boolean IsMailSend { get; set; }

        public LogBase(bool isMailSend)
            : base(db, cache)
        {

        }

        async public override System.Threading.Tasks.Task WriteAsync(LogType type, string session, string account, object value, object result = null, double time = 0, string member = null, string ip = null, string describe = null)
        {
            await base.WriteAsync(type, session, account, value, result, time, member, ip, describe);

            //send excep mail
            if (IsMailSend)// && LogType.Exception == type
            {
                await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    var content = string.Format("dtt:{0} type:{1} account:{2} account:{3} member:{4} value:{5} result:{6} time:{7} ip:{8} description:{9}",
                           System.DateTime.Now.ToString(),
                           (int)type,
                           session,
                           account,
                           member,
                           Newtonsoft.Json.JsonConvert.SerializeObject(value),
                           Newtonsoft.Json.JsonConvert.SerializeObject(result),
                           time,
                           ip,
                           describe);

                    try
                    {
                        this.MailSend(content, type, session, account, value, result, time, member, ip, describe);
                    }
                    catch (System.Exception ex)
                    {
                        if (null != db)
                        {
                            this.DB.Save(new SysLog { type = (int)LogType.Exception, session = session, account = "Sys", member = "MailSend", value = content, result = Newtonsoft.Json.JsonConvert.SerializeObject(ex), time = time, ip = ip, describe = describe });
                        }
                    }
                });
            }
        }

        public virtual void MailSend(string content, LogType type, string session, string account, object value, object result = null, double time = 0, string member = null, string ip = null, string describe = null)
        {
            //Mail.Send(member, content, "test@test.com", string.Format("Exception {0}", member), "smtp.live.com", "xlievo@live.com", "password", port: 587, enableSsl: true, to: "test@test.com");
        }
    }
    */
    public class LogBase : BusinessLib.Log.ILog
    {
        //System.IObservable<string> dd;
        //static System.Collections.Concurrent.BlockingCollection<SysLog> Queue;

        readonly BusinessLib.Data.IData db;

        public BusinessLib.Data.IData DB { get { return db; } }

        readonly BusinessLib.Cache.ICache cache;

        public BusinessLib.Cache.ICache Cache { get { return cache; } }

        public LogBase(BusinessLib.Data.IData db = null, BusinessLib.Cache.ICache cache = null)
        {
            this.db = db;
            this.cache = cache;
        }

        async public virtual System.Threading.Tasks.Task WriteAsync(LogType type, string session, string account, object value, object result = null, double time = 0, [System.Runtime.CompilerServices.CallerMemberName] string member = null, string ip = null, string describe = null)
        {
            //Enqueue
            //Dequeue
            //var forOpt = new System.Threading.Tasks.ParallelOptions() { MaxDegreeOfParallelism = 2 };

            if (null != this.db)
            {
                await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        this.db.Save(new SysLog { type = (int)type, session = session, account = account, member = member, value = Newtonsoft.Json.JsonConvert.SerializeObject(value), result = Newtonsoft.Json.JsonConvert.SerializeObject(result), time = time, ip = ip, describe = describe });
                    }
                    catch (System.Exception ex)
                    {
                        var msg = string.Format("{0}======================{1}======================{0}{2}{0}", System.Environment.NewLine, System.DateTime.Now, System.Convert.ToString(ex));
                        System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SysLog.text"), msg, System.Text.Encoding.UTF8);
                        throw ex;
                    }
                });
            }

            if (null != this.cache)
            {
                await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        this.cache.Set(string.Format("SysLog_{0}", System.Guid.NewGuid().ToString("N")), new SysLog { type = (int)type, session = session, account = account, member = member, value = Newtonsoft.Json.JsonConvert.SerializeObject(value), result = Newtonsoft.Json.JsonConvert.SerializeObject(result), time = time, ip = ip, describe = describe });
                    }
                    catch (System.Exception ex)
                    {
                        var msg = string.Format("{0}======================{1}======================{0}{2}{0}", System.Environment.NewLine, System.DateTime.Now, System.Convert.ToString(ex));
                        System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SysLog.text"), msg, System.Text.Encoding.UTF8);
                        throw ex;
                    }
                });
            }
        }
    }
}