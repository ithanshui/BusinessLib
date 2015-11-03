namespace BusinessLib.Log
{
    public enum LogType
    {
        /// <summary>
        /// Exception = 0
        /// </summary>
        Exception = 0,
        /// <summary>
        /// Record = 1
        /// </summary>
        Record = 1
    }
    public interface ILog
    {
        System.Threading.Tasks.Task WriteAsync(LogType type, string session, string account, object value, object result = null, double time = 0, [System.Runtime.CompilerServices.CallerMemberName] string member = null, string ip = null, string description = null);

        //void MailSend(string content, LogType type, string account, object value, object result = null, double time = 0, [System.Runtime.CompilerServices.CallerMemberName] string member = null, string ip = null, string description = null);
    }
}
