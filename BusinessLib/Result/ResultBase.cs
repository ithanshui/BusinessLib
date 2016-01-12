namespace BusinessLib.Result
{
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class ResultBase : IResult
    {
        public ResultBase() { State = 1; }

        public ResultBase(int state) { State = state; }

        public ResultBase(int state, string message)
        {
            if (0 < state) { state = 0 - System.Math.Abs(state); }
            State = state;
            Message = message;
        }

        System.Func<object, System.Type, object> deserialize = new System.Func<object, System.Type, object>((ags, type) => { return Newtonsoft.Json.JsonConvert.DeserializeObject(System.Convert.ToString(ags), type); });

        public ResultBase(System.Func<object, System.Type, object> deserialize)
        {
            this.deserialize = deserialize;
        }

        /// <summary>
        /// 结果状态 大于等于1：成功，等于0：未捕捉系统级异常，小于0：业务级错误。
        /// </summary>
        [ProtoBuf.ProtoMember(1, Name = "S")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "S")]
        public virtual System.Int32 State { get; set; }

        /// <summary>
        /// 成功可为 null
        /// </summary>
        [ProtoBuf.ProtoMember(2, Name = "M")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "M")]
        public virtual System.String Message { get; set; }

        /// <summary>
        /// Json
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// ProtoBuf
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToBytes()
        {
            return BusinessLib.Extensions.Help.ProtoBufSerialize(this);
        }

        public virtual object Deserialize(object ags, System.Type type)
        {
            return deserialize(ags, type);
        }
    }

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class ResultBase<DataType> : ResultBase, IResult<DataType>
    {
        public ResultBase() : base() { }

        public ResultBase(int state) : base(state) { }

        public ResultBase(int state, string message) : base(state, message) { }

        public ResultBase(DataType data = default(DataType), int state = 1)
        {
            if (1 > state) { state = System.Math.Abs(state); }
            State = state;
            Data = data;
        }

        /// <summary>
        /// 结果状态 大于等于1：成功，等于0：未捕捉系统级异常，小于0：业务级错误。
        /// </summary>
        [ProtoBuf.ProtoMember(1, Name = "S")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "S")]
        public override System.Int32 State { get; set; }

        /// <summary>
        /// 成功可为 null
        /// </summary>
        [ProtoBuf.ProtoMember(2, Name = "M")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "M")]
        public override System.String Message { get; set; }

        /// <summary>
        /// 具体 Json 数据对象
        /// </summary>
        [ProtoBuf.ProtoMember(3, Name = "D")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "D")]
        public virtual DataType Data { get; set; }
    }

    public static class ResultFactory
    {
        #region static

        static BusinessLib.Mark.MarkBase MarkBase = new BusinessLib.Mark.MarkBase();

        public static IResult Create()
        {
            return Create<ResultBase>();
        }

        public static IResult Create(int state)
        {
            return Create<ResultBase>(state);
        }

        public static IResult Create(int state, string message)
        {
            return Create<ResultBase>(state, message);
        }

        public static ResultBase<Data> Create<Data>(this Data data, int state = 1)
        {
            return new ResultBase<Data>(data, state);
        }

        public static IResult Create(BusinessLib.Mark.MarkEnum mark)
        {
            return Create(System.Convert.ToInt32(mark), System.Convert.ToString(MarkBase.Get(mark)));
        }

        //========================================================//

        public static Result Create<Result>()
            where Result : class, IResult, new()
        {
            return new Result() { State = 1 };
        }

        public static Result Create<Result>(int state)
            where Result : class, IResult, new()
        {
            return new Result() { State = state };
        }

        public static Result Create<Result>(int state, string message)
           where Result : class, IResult, new()
        {
            if (0 < state) { state = 0 - System.Math.Abs(state); }
            return new Result() { State = state, Message = message };
        }

        public static IResult<Data> Create<Data>(this Data data, IResult<Data> result, int state = 1)
        {
            if (1 > state) { state = System.Math.Abs(state); }

            result.State = state;
            result.Data = data;

            return result;
        }

        public static Result Create<Result>(BusinessLib.Mark.MarkEnum mark)
            where Result : class, IResult, new()
        {
            return Create<Result>(System.Convert.ToInt32(mark), System.Convert.ToString(MarkBase.Get(mark)));
        }

        //========================================================//

        public static ResultBase<DataType> DeserializeResultJson<DataType>(this string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResultBase<DataType>>(value);
        }

        public static IResult<DataType> DeserializeResultProtoBuf<DataType>(this byte[] source)
        {
            return DeserializeResultProtoBuf<DataType, ResultBase<DataType>>(source);
        }
        public static IResult<DataType> DeserializeResultProtoBuf<DataType, Result>(this byte[] source)
            where Result : class, IResult<DataType>, new()
        {
            using (var stream = new System.IO.MemoryStream(source))
            {
                return ProtoBuf.Serializer.Deserialize<Result>(stream);
            }
        }

        #endregion
    }
}