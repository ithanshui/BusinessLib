/*==================================
             ########
            ##########

             ########
            ##########
          ##############
         #######  #######
        ######      ######
        #####        #####
        ####          ####
        ####   ####   ####
        #####  ####  #####
         ################
          ##############
==================================*/

namespace Business.Log
{
    public class log4netAdapter : ILog
    {
        readonly log4net.ILog log;

        public log4netAdapter(log4net.ILog log)
        {
            this.log = log;
        }

        public void Debug(string message)
        {
            if (log.IsDebugEnabled) { log.Debug(message); }
        }

        public void Debug(string format, params object[] args)
        {
            if (log.IsDebugEnabled) { log.Debug(string.Format(format, args)); }
        }

        public void Info(string message)
        {
            if (log.IsInfoEnabled) { log.Info(message); }
        }

        public void Info(string format, params object[] args)
        {
            if (log.IsInfoEnabled) { log.Info(string.Format(format, args)); }
        }

        public void Warn(string message)
        {
            if (log.IsWarnEnabled) { log.Warn(message); }
        }

        public void Warn(string format, params object[] args)
        {
            if (log.IsWarnEnabled) { log.Warn(string.Format(format, args)); }
        }

        public void Error(string message)
        {
            if (log.IsErrorEnabled) { log.Error(message); }
        }

        public void Error(string format, params object[] args)
        {
            if (log.IsErrorEnabled) { log.Error(string.Format(format, args)); }
        }

        public void Fatal(string message)
        {
            if (log.IsFatalEnabled) { log.Fatal(message); }
        }

        public void Fatal(string format, params object[] args)
        {
            if (log.IsFatalEnabled) { log.Fatal(string.Format(format, args)); }
        }
    }
}
