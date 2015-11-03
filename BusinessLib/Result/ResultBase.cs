namespace BusinessLib.Result
{
    public class ResultBase<DataType> : IResult<DataType>
    {
        //public const int Undefined = -99;
        ResultBase() { }

        public ResultBase(int state, string message)
        {
            if (0 < state) { state = 0 - System.Math.Abs(state); }
            State = state;
            Message = message;
        }

        //public ResultBase(string message)
        //{
        //    State = -99;// undefined
        //    Messag = message;
        //}

        public ResultBase(int state)
            : this(default(DataType), state: state) { }
        public ResultBase(DataType data = default(DataType), int state = 1)
        {
            if (1 > state) { state = System.Math.Abs(state); }
            State = state;
            Data = data;
        }

        /// <summary>
        /// 结果状态 大于等于1：成功，等于0：未捕捉系统级异常，小于0：业务级错误。
        /// </summary>
        public System.Int32 State { get; set; }
        /// <summary>
        /// 成功可为 null。
        /// </summary>
        public System.String Message { get; set; }
        /// <summary>
        /// 具体 Json 数据对象。
        /// </summary>
        public DataType Data { get; set; }

        /// <summary>
        /// Json。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        public string ToString(Newtonsoft.Json.JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, settings);
        }
        /// <summary>
        /// ProtoBuf。
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            return BusinessLib.Extensions.Help.ProtoBufSerialize(this);
        }
    }

    public static class ResultExtensions
    {
        static BusinessLib.Mark.MarkBase MarkBase = new BusinessLib.Mark.MarkBase();

        public static ResultBase<System.Object> Result()
        {
            return new ResultBase<System.Object>();
        }
        public static ResultBase<System.Object> Result(int state)
        {
            return new ResultBase<System.Object>(state);
        }
        //=========================//
        public static ResultBase<System.Object> Result(int state, string message)
        {
            return new ResultBase<System.Object>(state, message);
        }
        public static ResultBase<System.Object> Result(BusinessLib.Mark.MarkEnum mark)
        {
            return Result(System.Convert.ToInt32(mark), System.Convert.ToString(MarkBase.Get(mark)));
        }
        //=========================//
        //=========================//
        public static ResultBase<DataType> Result<DataType>(int state, string message)
        {
            return new ResultBase<DataType>(state, message);
        }
        //=========================//
        public static ResultBase<DataType> Result<DataType>(this DataType data, int state = 1)
        {
            return new ResultBase<DataType>(data, state);
        }
        //=========================//
        //=========================//
        public static ResultBase<DataType> Deserialize<DataType>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResultBase<DataType>>(json);
        }

        //public static ResultBase<DataType> DeserializeJson<DataType>(string json)
        //{
        //    var result = Deserialize<System.Object>(json);

        //    var _result = new ResultBase<DataType> { State = result.State, Message = result.Message };

        //    if (null != result.Data)
        //    {
        //        _result.Data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataType>(System.Convert.ToString(result.Data));
        //    }

        //    return _result;
        //}

        public static ResultBase<DataType> Deserialize<DataType>(byte[] bytes)
        {
            return BusinessLib.Extensions.Help.ProtoBufDeserialize<ResultBase<DataType>>(bytes);
        }
    }
}