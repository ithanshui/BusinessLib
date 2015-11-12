using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLib.Mail
{
    public interface IMail
    {
        void Send(string subject, string content, string from, string displayName, string host, string credentialsUserName, string credentialsPassword, int port = 25, bool enableSsl = false, Encoding contentEncoding = null, string mediaType = "text/html", params string[] to);
    }
}
