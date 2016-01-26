# BusinessLib

NuGet:https://www.nuget.org/packages/Business.Lib/

# This is a Server framework

# Please refer to the UnitTest

# Arguments receive the beginning
    
    [Json]
    public struct Register
    {
        [CanNotNull(-11, "\"account\" not is null")]
        [Size(-12, Min = 4, Max = "8")]
        [CheckChar(Mode = Help.CheckCharMode.All, Code = -13, Message = "\" char account\" verification failed")]
        public string account;
    }

# IResult end

    public interface ISerialize
    {
        /// <summary>
        /// ProtoBuf
        /// </summary>
        /// <returns></returns>
        byte[] ToBytes();

        /// <summary>
        /// Json
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
    
    public interface IResult : Authentication.ISerialize
    {
        /// <summary>
        /// The results of the state is greater than or equal to 1: success, equal to 0: not to capture the system level exceptions, less than 0: business class error.
        /// </summary>
        System.Int32 State { get; set; }

        /// <summary>
        /// Success can be null
        /// </summary>
        System.String Message { get; set; }

        /// <summary>
        /// Specific Byte/Json data objects
        /// </summary>
        dynamic Data { get; }

        /// <summary>
        /// Json Data
        /// </summary>
        /// <returns></returns>
        string ToDataString();

        /// <summary>
        /// ProtoBuf Data
        /// </summary>
        /// <returns></returns>
        byte[] ToDataBytes();
    }

    public interface IResult<DataType> : IResult
    {
        /// <summary>
        /// Specific Byte/Json data objects
        /// </summary>
        new DataType Data { get; set; }
    }
    
# Include session handling
    
    public interface ISession : ISerialize
    {
        System.String Site { get; set; }
        System.String Account { get; set; }
        System.String Password { get; set; }

        System.String Key { get; set; }
        System.String IP { get; set; }

        System.DateTime Time { get; set; }

        System.Collections.Generic.List<string> Competences { get; set; }

        ISession Clone();
    }

    public interface ISession<DataType> : ISession
    {
        new ISession<DataType> Clone();

        DataType Data { get; set; }
    }
