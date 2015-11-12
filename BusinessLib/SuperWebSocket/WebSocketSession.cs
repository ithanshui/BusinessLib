using BusinessLib.Extensions;
namespace Common.Net
{
    public class WebSocketSession : SuperWebSocket.WebSocketSession<WebSocketSession>
    {
        public void Send(string key, byte[] data, int offset, int length, bool compres = false)
        {
            var _data = DataConvert(key, data, offset, length, compres);
            base.Send(_data, 0, _data.Length);
        }
        public void Send(string key, byte[] data, bool compres = false) { this.Send(key, data, 0, data.Length, compres); }
        public override void Send(byte[] data, int offset, int length) { this.Send(System.String.Empty, data, offset, length); }

        public bool TrySend(string key, byte[] data, int offset, int length, bool compres = false)
        {
            var _data = DataConvert(key, data, offset, length, compres);
            return base.TrySend(_data, 0, _data.Length);
        }
        public bool TrySend(string key, byte[] data, bool compres = false) { return this.TrySend(key, data, 0, data.Length, compres); }
        public override bool TrySend(byte[] data, int offset, int length) { return this.TrySend(System.String.Empty, data, offset, length); }

        static unsafe byte[] DataConvert(string key, byte[] data, int offset, int length, bool compres)
        {
            var _key = System.Text.Encoding.UTF8.GetBytes(key);
            var l = key.Length;
            var _data = new byte[length];
            System.Buffer.BlockCopy(data, offset, _data, 0, length);//data
            //compres data
            if (compres) { using (var stream = _data.CompressionByteToStream(Help.CompressionMode.Compress)) { _data = stream.ToArray(); } }
            //compres end
            var send_data = new byte[2 + l + _data.Length];//count
            send_data[0] = (byte)l;//key length
            System.Buffer.BlockCopy(_key, 0, send_data, 1, l);//key
            send_data[1 + l] = (byte)(compres ? 1 : 0);//compres
            System.Buffer.BlockCopy(_data, 0, send_data, 2 + l, _data.Length);//data
            return send_data;
        }
    }
}
