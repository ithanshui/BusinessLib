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

namespace Business.Extensions
{
    public static class Command
    {
        const char HeadSplit = '|';

        public struct CommandData
        {
            /// <summary>
            /// Gets the cmd of this request.
            /// </summary>
            public string Cmd { get; set; }

            /// <summary>
            /// Gets the Command token of this request, used for callback
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// Gets the login key of this request.
            /// </summary>
            public bool Key { get; set; }

            /// <summary>
            /// Gets the data of this request.
            /// </summary>
            public byte[] Data { get; set; }
        }

        public class MemberData
        {
            /// <summary>
            /// Gets the cmd of this request.
            /// </summary>
            public string Cmd { get; set; }

            /// <summary>
            /// Gets the Command token of this request, used for callback
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// Gets the data of this request.
            /// </summary>
            public object Data { get; set; }
        }

        /// <summary>
        /// {cmd-callbackToken-BusinessLibKey-data}
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <param name="keyMaxLength"></param>
        /// <returns></returns>
        static CommandData GetCommandData(byte[] data, out string error, int keyMaxLength = 64)
        {
            error = null;

            if (0 == data.Length) { error = "data length error"; return default(CommandData); }
            // head length
            var headLength = data[0];
            if (keyMaxLength < headLength || headLength >= data.Length) { error = "head length error"; return default(CommandData); }
            // head
            var head = new byte[headLength];
            System.Buffer.BlockCopy(data, 1, head, 0, head.Length);
            string headChar;
            try
            {
                headChar = System.Text.Encoding.UTF8.GetString(head);
            }
            catch { error = "head encoding error"; return default(CommandData); }
            // key
            var key = System.BitConverter.ToBoolean(data, headLength + 1);
            // token
            var cmd = System.String.Empty;// cmd -
            var token = System.String.Empty;// Socket token -

            var headData = headChar.Split(HeadSplit);
            switch (headData.Length)
            {
                case 2://Not Login
                    cmd = headData[0];//cmd
                    token = headData[1];//callback
                    break;
                case 1://Not Result Not callback Not Login
                    cmd = headData[0];
                    break;
                default: error = "head data error"; return default(CommandData);
            }

            // data
            var _data = new byte[data.Length - (2 + headLength)];
            System.Buffer.BlockCopy(data, 2 + headLength, _data, 0, _data.Length);
            //End
            return new CommandData { Cmd = cmd, Token = token, Key = key, Data = _data };
        }

        public static Result.IResult GetCommandResult(this byte[] value, Auth.IInterception command, Auth.IInterception commandNot, string ip, string commandId, out Extensions.InterceptorCommand commandMeta, out CommandData commandData)
        {
            commandMeta = default(Extensions.InterceptorCommand);
            string error = null;

            commandData = GetCommandData(value, out error);

            if (!System.String.IsNullOrEmpty(error))
            {
                return Result.ResultFactory.Create((int)Mark.MarkItem.Command_DataError, error);
            }

            var cmd = commandData.Cmd;
            var token = commandData.Token;
            var key = commandData.Key;
            var data = commandData.Data;

            Result.IResult result;

            if ("Login".Equals(cmd, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var _value = System.Text.Encoding.UTF8.GetString(data);

                string _error = null;
                var _result = command.Business.Login(_value, out _error, commandId);

                if (!System.String.IsNullOrEmpty(_error))
                {
                    result = Result.ResultFactory.Create((int)Mark.MarkItem.Login_Error, _error);
                }
                else
                {
                    result = Result.ResultFactory.Create(System.Text.Encoding.UTF8.GetBytes(_result));

                    //commandMeta = new InterceptorCommand(null, Attributes.CommandAttribute.DataType.Byte, false);
                }

                return result;
            }
            else
            {
                //var hasKey = !System.String.IsNullOrEmpty(key);

                if (key)//Interceptor
                {
                    if (!command.Command.TryGetValue(cmd, out commandMeta))
                    {
                        result = Result.ResultFactory.Create((int)Mark.MarkItem.Command_KeyError, string.Format("Not Cmd {0}", cmd));
                        return result;
                    }
                }
                else//InterceptorNot
                {
                    if (!commandNot.Command.TryGetValue(cmd, out commandMeta))
                    {
                        result = Result.ResultFactory.Create((int)Mark.MarkItem.Command_KeyError, string.Format("Not Cmd {0}", cmd));
                        return result;
                    }
                }

                var _token = new Auth.Token { Key = key ? commandId : "Socket", IP = ip }.ToString();

                //Result
                return commandMeta.Member(_token, data);

                ////Result
                //if (commandMeta.HasReturn && !System.String.IsNullOrEmpty(token))
                //{
                //    return result;
                //}

                //return null;
            }
        }

        public static Result.IResult GetMemberResult(Auth.IInterception command, Auth.IInterception commandNot, string ip, MemberData memberData)
        {
            var cmd = memberData.Cmd;
            var token = memberData.Token;
            var data = memberData.Data;

            if (null == cmd) { return Result.ResultFactory.Create((int)Mark.MarkItem.Command_KeyError, "Cmd Error"); }

            Result.IResult result;

            System.Func<object, object, Result.IResult> commandMeta;

            if ("Login".Equals(cmd, System.StringComparison.InvariantCultureIgnoreCase))
            {
                string _error = null;
                var _result = command.Business.Login(System.Convert.ToString(data), out _error);

                if (!System.String.IsNullOrEmpty(_error))
                {
                    result = Result.ResultFactory.Create((int)Mark.MarkItem.Login_Error, _error);
                }
                else
                {
                    result = Result.ResultFactory.Create(_result);
                }

                return result;
            }
            else
            {
                var hasKey = !System.String.IsNullOrEmpty(token);

                if (hasKey)//Interceptor
                {
                    if (!command.Member.TryGetValue(cmd, out commandMeta))
                    {
                        result = Result.ResultFactory.Create((int)Mark.MarkItem.Command_KeyError, string.Format("Not Cmd {0}", cmd));
                        return result;
                    }
                }
                else//InterceptorNot
                {
                    if (!commandNot.Member.TryGetValue(cmd, out commandMeta))
                    {
                        result = Result.ResultFactory.Create((int)Mark.MarkItem.Command_KeyError, string.Format("Not Cmd {0}", cmd));
                        return result;
                    }
                }

                var _token = new Auth.Token { Key = hasKey ? token : "Service", IP = ip }.ToString();

                //Result
                return commandMeta(_token, data);
            }
        }

        public static byte[] GetCommandResult(this byte[] value, Auth.IInterception command, Auth.IInterception commandNot, string ip, string commandId)
        {
            Extensions.InterceptorCommand commandMeta;
            CommandData commandData;
            var result = GetCommandResult(value, command, commandNot, ip, commandId, out commandMeta, out commandData);

            if (null == result) { return null; }

            byte[] head = null;
            byte[] data = null;

            if (0 < result.State)
            {
                switch (commandMeta.ResultDataType)
                {
                    case Attributes.CommandAttribute.DataType.Byte:
                        data = result.Data as byte[];
                        break;
                    case Attributes.CommandAttribute.DataType.ProtoBuf:
                        data = result.ToDataBytes();
                        break;
                    case Attributes.CommandAttribute.DataType.Json:
                        data = System.Text.Encoding.UTF8.GetBytes(result.ToDataString());
                        break;
                }

                head = System.Text.Encoding.UTF8.GetBytes(commandData.Token);
            }
            else
            {
                head = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}{1}{2}", commandData.Token, HeadSplit, result.Message));
            }

            var state = System.BitConverter.GetBytes((short)result.State);

            var headLength = state.Length + 1 + head.Length;
            var _data = new byte[headLength + (null != data ? data.Length : 0)];

            System.Buffer.BlockCopy(state, 0, _data, 0, state.Length);
            _data[state.Length] = (byte)head.Length;
            System.Buffer.BlockCopy(head, 0, _data, state.Length + 1, head.Length);
            if (null != data) { System.Buffer.BlockCopy(data, 0, _data, headLength, data.Length); }

            return _data;
        }

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

        public static CommandResult GetCommandResult(byte[] value)
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
    }
}