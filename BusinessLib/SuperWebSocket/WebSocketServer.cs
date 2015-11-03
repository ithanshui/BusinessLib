namespace Common.Net
{
    using BusinessLib.Extensions;
    using System.Linq;

    #region WebSocketServer
    /// <summary>
    /// WebSocket AppServer
    /// </summary>
    public class WebSocketServer : SuperWebSocket.WebSocketServer<WebSocketSession>
    {
        internal System.Collections.Concurrent.ConcurrentDictionary<string, SubCommandBaseByte> subCommands;// = new System.Collections.Concurrent.ConcurrentDictionary<string, SubCommandBaseByte>();
        public WebSocketServer(System.Collections.Generic.IEnumerable<SuperWebSocket.SubProtocol.ISubProtocol<WebSocketSession>> subProtocols)
            : base(subProtocols) { subCommands = CreateSubClass<SubCommandBaseByte>(System.AppDomain.CurrentDomain.GetAssemblies()); base.NewDataReceived += WebSocketServer_NewDataReceived; }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketServer"/> class.
        /// </summary>
        /// <param name="subProtocol">The sub protocol.</param>
        public WebSocketServer(SuperWebSocket.SubProtocol.ISubProtocol<WebSocketSession> subProtocol)
            : base(subProtocol)
        {
            subCommands = CreateSubClass<SubCommandBaseByte>(System.AppDomain.CurrentDomain.GetAssemblies()); base.NewDataReceived += WebSocketServer_NewDataReceived;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketServer"/> class.
        /// </summary>
        public WebSocketServer()
            : base(new System.Collections.Generic.List<SuperWebSocket.SubProtocol.ISubProtocol<WebSocketSession>>()) { subCommands = CreateSubClass<SubCommandBaseByte>(System.AppDomain.CurrentDomain.GetAssemblies()); base.NewDataReceived += WebSocketServer_NewDataReceived; }

        public delegate void SessionHandler<TAppSession, TParam, TParam1, TParam2>(TAppSession session, TParam key, TParam1 token, TParam2 data)
        where TAppSession : SuperSocket.SocketBase.IAppSession;
        public new event SessionHandler<WebSocketSession, string, string, byte[]> NewDataReceived;

        unsafe void WebSocketServer_NewDataReceived(WebSocketSession session, byte[] data)
        {
            if (0 == data.Length) { return; }
            // key length
            var l = data[0];
            // key
            var _key = new byte[l];
            System.Buffer.BlockCopy(data, 1, _key, 0, _key.Length);
            var key = System.Text.Encoding.UTF8.GetString(_key, 0, _key.Length);
            // token
            var token = System.String.Empty;
            var i = key.IndexOf('-');
            if (0 < i) { token = key.Substring(i + 1); key = key.Substring(0, i); }
            // compression
            var compres = System.Convert.ToBoolean(data[1 + l]);
            // data
            var _data = new byte[data.Length - (2 + l)];
            System.Buffer.BlockCopy(data, 2 + l, _data, 0, _data.Length);
            //End
            if (compres) { using (var stream = Help.CompressionByteToStream(_data, Help.CompressionMode.Decompress)) { _data = stream.ToArray(); } }

            SubCommandBaseByte subRequestInfoByte;
            if (null != subCommands && subCommands.TryGetValue(key, out subRequestInfoByte)) { subRequestInfoByte.ExecuteCommand(session, new SubRequestInfoByte(key, token, _data)); }
            else if (null != this.NewDataReceived) { this.NewDataReceived(session, key, token, _data); }
        }

        static System.Collections.Concurrent.ConcurrentDictionary<string, T> CreateSubClass<T>(System.Reflection.Assembly[] assemblys, params object[] args)
        {
            if (null == assemblys) { return null; }
            var subInstance = new System.Collections.Concurrent.ConcurrentDictionary<string, T>();
            var type = typeof(T);
            foreach (var abs in assemblys.Where(a => null != a))
            {
                try
                {
                    var types = abs.GetTypes().Where(a => null != a && a.IsSubclassOf(type));
                    if (0 < types.Count())
                    {
                        foreach (var item in types)
                        {
                            subInstance.AddOrUpdate(item.Name, (T)System.Activator.CreateInstance(item, args), (key, oldValue) => oldValue);
                        }
                    }
                }
                catch { }
            }
            return subInstance;
        }
    }
    public class SubRequestInfoByte : SuperSocket.SocketBase.Protocol.RequestInfo<byte[]>, SuperWebSocket.SubProtocol.ISubRequestInfo
    {
        /// <summary>
        /// Gets the token of this request, used for callback
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubRequestInfo"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="token">The token.</param>
        /// <param name="data">The data.</param>
        public SubRequestInfoByte(string key, string token, byte[] data)
            : base(key, data)
        {
            Token = token;
        }
    }
    /// <summary>
    /// SubCommand interface
    /// </summary>
    /// <typeparam name="TWebSocketSession">The type of the web socket session.</typeparam>
    public interface ISubCommandByte<TWebSocketSession> : SuperSocket.SocketBase.Command.ICommand
        where TWebSocketSession : SuperWebSocket.WebSocketSession<TWebSocketSession>, new()
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="requestInfo">The request info.</param>
        void ExecuteCommand(TWebSocketSession session, SubRequestInfoByte requestInfo);
    }
    /// <summary>
    /// SubCommand base
    /// </summary>
    public abstract class SubCommandBaseByte : SubCommandBaseByte<WebSocketSession> { }

    /// <summary>
    /// SubCommand base
    /// </summary>
    /// <typeparam name="TWebSocketSession">The type of the web socket session.</typeparam>
    public abstract class SubCommandBaseByte<TWebSocketSession> : ISubCommandByte<TWebSocketSession>, SuperWebSocket.SubProtocol.ISubCommandFilterLoader
        where TWebSocketSession : SuperWebSocket.WebSocketSession<TWebSocketSession>, new()
    {
        #region ISubCommand Members

        /// <summary>
        /// Gets the name.
        /// </summary>
        public virtual string Name
        {
            get { return this.GetType().Name; }
        }

        void ISubCommandByte<TWebSocketSession>.ExecuteCommand(TWebSocketSession session, SubRequestInfoByte requestInfo)
        {
            var filters = m_Filters;

            if (filters == null || filters.Length == 0)
            {
                ExecuteCommand(session, requestInfo);
                return;
            }

            var commandContext = new SuperSocket.SocketBase.CommandExecutingContext();
            commandContext.Initialize(session, requestInfo, this);

            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];

                filter.OnCommandExecuting(commandContext);

                if (commandContext.Cancel)
                    break;
            }

            if (!commandContext.Cancel)
            {
                ExecuteCommand(session, requestInfo);

                for (var i = 0; i < filters.Length; i++)
                {
                    var filter = filters[i];
                    filter.OnCommandExecuted(commandContext);
                }
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="requestInfo">The request info.</param>
        public abstract void ExecuteCommand(TWebSocketSession session, SubRequestInfoByte requestInfo);

        #endregion

        private SuperWebSocket.SubProtocol.SubCommandFilterAttribute[] m_Filters;

        void SuperWebSocket.SubProtocol.ISubCommandFilterLoader.LoadSubCommandFilters(System.Collections.Generic.IEnumerable<SuperWebSocket.SubProtocol.SubCommandFilterAttribute> globalFilters)
        {
            var filters = new System.Collections.Generic.List<SuperWebSocket.SubProtocol.SubCommandFilterAttribute>();

            if (globalFilters.Any())
            {
                filters.AddRange(globalFilters);
            }

            var commandFilters = this.GetType().GetCustomAttributes(true).OfType<SuperWebSocket.SubProtocol.SubCommandFilterAttribute>().ToArray();

            if (commandFilters.Any())
            {
                filters.AddRange(commandFilters);
            }

            m_Filters = filters.OrderBy(f => f.Order).ToArray();
        }
    }
    #endregion
}
