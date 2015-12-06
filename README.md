# BusinessLib

NuGet:https://www.nuget.org/packages/BusinessLib/

# This is a Server framework

# Please refer to the Template.cs

# Arguments receive the beginning
    
    [Json]
    public struct Register
    {
        [CanNotNull(Code = -11, Message = "\"account\" not is null")]
        [Size(Min = 4, Max = "8", Code = -12)]
        [CheckChar(Mode = Help.CheckCharMode.All, Code = -13, Message = "\" char account\" verification failed")]
        public string account;
    }

# IResult end

    public interface IResult
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
        /// Json
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// ProtoBuf
        /// </summary>
        /// <returns></returns>
        byte[] ToBytes();
    }

    public interface IResult<DataType> : IResult
    {
        /// <summary>
        /// Specific Byte/Json data objects
        /// </summary>
       new DataType Data { get; set; }
    }
    
# Include session handling
    
    public interface ISession
    {
        System.String Site { get; set; }
        System.String Account { get; set; }
        System.String Password { get; set; }
        System.String SecurityCode { get; set; }

        System.String Key { get; set; }
        System.String IP { get; set; }

        System.DateTime Time { get; set; }
        RoleCompetence RoleCompetence { get; set; }

        ISession Clone();
    }
