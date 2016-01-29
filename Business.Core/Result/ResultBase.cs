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

namespace Business.Result
{
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class ResultBase<DataType> : IResult<DataType>
    {
        public static implicit operator ResultBase<DataType>(string value)
        {
            return Extensions.Help.JsonDeserialize<ResultBase<DataType>>(value);
        }
        public static implicit operator ResultBase<DataType>(byte[] value)
        {
            return Extensions.Help.ProtoBufDeserialize<ResultBase<DataType>>(value);
        }

        public ResultBase() { State = 1; }

        public ResultBase(int state) { State = state; }

        public ResultBase(int state, string message = null)
        {
            if (0 < state) { state = 0 - System.Math.Abs(state); }
            State = state;
            Message = message;
        }

        public ResultBase(DataType data = default(DataType), int state = 1)
        {
            if (1 > state) { state = System.Math.Abs(state); }
            State = state;
            Data = data;
        }

        /// <summary>
        /// The results of the state is greater than or equal to 1: success, equal to 0: not to capture the system level exceptions, less than 0: business class error.
        /// </summary>
        [ProtoBuf.ProtoMember(1, Name = "S")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "S")]
        public virtual System.Int32 State { get; set; }

        /// <summary>
        /// Success can be null
        /// </summary>
        [ProtoBuf.ProtoMember(2, Name = "M")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "M")]
        public virtual System.String Message { get; set; }

        /// <summary>
        /// Specific dynamic data objects
        /// </summary>
        dynamic IResult.Data { get { return this.Data; } }

        /// <summary>
        /// Specific Byte/Json data objects
        /// </summary>
        [ProtoBuf.ProtoMember(3, Name = "D")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "D")]
        public virtual DataType Data { get; set; }

        /// <summary>
        /// Json
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Json Data
        /// </summary>
        /// <returns></returns>
        public virtual string ToDataString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this.Data);
        }

        /// <summary>
        /// ProtoBuf
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToBytes()
        {
            return Extensions.Help.ProtoBufSerialize(this);
        }

        /// <summary>
        /// ProtoBuf Data
        /// </summary>
        /// <returns></returns>
        public virtual byte[] ToDataBytes()
        {
            return Extensions.Help.ProtoBufSerialize(this.Data);
        }
    }

    public static class ResultFactory
    {
        #region static

        public static IResult Create()
        {
            return Create<ResultBase<string>>();
        }

        public static IResult Create(int state)
        {
            return Create<ResultBase<string>>(state);
        }

        public static IResult Create(int state, string message)
        {
            return Create<ResultBase<string>>(state, message);
        }

        public static IResult<Data> Create<Data>(this Data data, int state = 1)
        {
            return new ResultBase<Data>(data, state);
        }

        public static IResult Create(Business.Mark.MarkItem mark)
        {
            return Create(System.Convert.ToInt32(mark), Business.Mark.Get<string>(mark));
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

        public static Result Create<Result>(Business.Mark.MarkItem mark)
            where Result : class, IResult, new()
        {
            return Create<Result>(System.Convert.ToInt32(mark), Business.Mark.Get<string>(mark));
        }

        //========================================================//
        
        public static IResult<DataType> DeserializeResultProtoBuf<DataType, Result>(this byte[] source)
    where Result : class, IResult<DataType>, new()
        {
            return Extensions.Help.ProtoBufDeserialize<Result>(source);
        }

        #endregion
    }
}