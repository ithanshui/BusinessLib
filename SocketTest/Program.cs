using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTest
{
    class Program
    {
        static SuperSocket.WebSocket.WebSocketServer m_WebSocketServer;

        static void Main(string[] args)
        {
            var subProtocol = new SuperSocket.WebSocket.SubProtocol.BasicSubProtocol<SuperSocket.WebSocket.WebSocketSession>("Basic");
            m_WebSocketServer = new SuperSocket.WebSocket.WebSocketServer(subProtocol);
            m_WebSocketServer.NewSessionConnected += m_WebSocketServer_NewSessionConnected;
            m_WebSocketServer.SessionClosed += m_WebSocketServer_SessionClosed;
            m_WebSocketServer.NewDataReceived += m_WebSocketServer_NewDataReceived;

            var serverConfig = new SuperSocket.SocketBase.Config.ServerConfig
            {
                Name = "WebSocket Server",
                Mode = SuperSocket.SocketBase.SocketMode.Tcp,
                LogAllSocketException = true,
                //Certificate = new SuperSocket.SocketBase.Config.CertificateConfig { FilePath = "supersocket.pfx", Password = "supersocket" },
                ClearIdleSession = true,
                ClearIdleSessionInterval = 60,
                IdleSessionTimeOut = 300,
                Listeners = new System.Collections.Generic.List<SuperSocket.SocketBase.Config.ListenerConfig>() {
                    //new SuperSocket.SocketBase.Config.ListenerConfig() { Port = 5100, Ip = "Any", Security = "Tls" }, 
                    new SuperSocket.SocketBase.Config.ListenerConfig() { Port = 5101, Ip = "Any" }
                },
                MaxConnectionNumber = 1000,
                SendingQueueSize = 1000,
                ReceiveBufferSize = 65535,
                MaxRequestLength = 65535,
                KeepAliveInterval = 60,
                KeepAliveTime = 600,
                SendBufferSize = 0,
            };
            m_WebSocketServer.Setup(serverConfig, logFactory: new SuperSocket.SocketBase.Logging.ConsoleLogFactory());
            m_WebSocketServer.Start();
            System.Console.Read();
        }

        static void m_WebSocketServer_SessionClosed(SuperSocket.WebSocket.WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            //TestLib.Common.Interceptor.Instance.CommandClosed(new Business.Auth.Token { Key = session.SessionID, IP = session.RemoteEndPoint.Address.ToString() }.ToString(), null);
        }

        static void m_WebSocketServer_NewSessionConnected(SuperSocket.WebSocket.WebSocketSession session)
        {
            //session.TrySend(session.SessionID + " Connected");
        }

        static void m_WebSocketServer_NewDataReceived(SuperSocket.WebSocket.WebSocketSession session, byte[] value)
        {
            var result = Business.Extensions.Command.CommandCall(value, TestLib.Common.Interceptor, TestLib.Common.InterceptorNot, session.RemoteEndPoint.Address.ToString(), session.SessionID);

            if (null != result)
            {
                session.TrySend(result, 0, result.Length);
            }
        }

        #region Client Call

        const char HeadSplit = '|';

        //===================Command-Client-Send==================//

        public static byte[] GetCommandData(string cmd, string token, bool key, string data)
        {
            return GetCommandData(cmd, token, key, System.Text.Encoding.UTF8.GetBytes(data));
        }
        public static byte[] GetCommandData(string cmd, string token = null, bool key = false, byte[] data = null)
        {
            if (System.String.IsNullOrEmpty(cmd)) { throw new System.Exception("cmd Not null"); }

            var head = System.Text.Encoding.UTF8.GetBytes(string.Format("{1}{0}{2}", HeadSplit, cmd, token));

            var _data = new byte[head.Length + 2 + (null == data ? 0 : data.Length)];

            _data[0] = (byte)head.Length;
            System.Buffer.BlockCopy(head, 0, _data, 1, head.Length);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(key), 0, _data, 1 + head.Length, 1);
            if (null != data) { System.Buffer.BlockCopy(data, 0, _data, head.Length + 2, data.Length); }

            return _data;
        }

        //===================Command-Client-Received==================//

        public struct CommandResult
        {
            /// <summary>
            /// Gets the state of this result.
            /// </summary>
            public short State { get; set; }

            /// <summary>
            /// Gets the token of this result, used for callback
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// Gets the message of this result.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Gets the data of this result.
            /// </summary>
            public byte[] Data { get; set; }
        }

        public static CommandResult CommandCall(byte[] value)
        {
            var stateData = new byte[2];
            System.Buffer.BlockCopy(value, 0, stateData, 0, stateData.Length);
            var state = System.BitConverter.ToInt16(stateData, 0);
            var headLength = (int)value[2];
            var token = System.Text.Encoding.UTF8.GetString(value, 3, headLength);

            if (0 < state)
            {
                var data = new byte[value.Length - (headLength + 3)];
                System.Buffer.BlockCopy(value, headLength + 3, data, 0, data.Length);
                return new CommandResult { State = state, Token = token, Data = data };
            }

            var tokens = token.Split(HeadSplit);
            return new CommandResult { State = state, Token = tokens[0], Message = tokens[1] };
        }

        #endregion
    }
}
