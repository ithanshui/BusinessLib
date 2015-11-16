using BusinessLib.Entity;

namespace BusinessLib.Log
{
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
                        System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SysLog.txt"), msg, System.Text.Encoding.UTF8);
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
                        System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SysLog.txt"), msg, System.Text.Encoding.UTF8);
                        throw ex;
                    }
                });
            }
        }
    }
}