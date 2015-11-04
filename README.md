# BusinessLib

# This is a Server framework

    public abstract class DataBase<IConnection> : IData, IData2 where IConnection : class, BusinessLib.Data.IConnection

//write data

    using (var con = this.DB.GetConnection())
    {
        con.BeginTransaction();

        member[0].loginIp = _ip;
        member[0].loginDtt = time;
        if (1 == result) { member[0].errorCount = 0; }
        if (0 == con.Update(member[0])) { con.Rollback(); throw new System.Data.DBConcurrencyException(typeof(SysAccount).Name); }
        if (-2 != result)//freeze
        {
            if (0 == con.Save(new SysLogin { category = 0, session = session, account = _account1, ip = _ip, data = _data, result = result, describe = _error, dtt = time })) { con.Rollback(); throw new System.Data.DBConcurrencyException(typeof(SysLogin).Name); }
        }

        con.Commit();
    }

    [Arguments]
    public struct Register
    {
        [CanNotNull(Code = -11, Message = "\"account\" not is null")]
        [Size(Min = 4, Max = "8", Code = -12)]
        [CheckChar(Mode = Help.CheckCharMode.All, Code = -13, Message = "\" char account\" verification failed")]
        public string account;
    }

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

    public abstract class InterceptorBase : Ninject.Extensions.Interception.IInterceptor

    public InterceptorBase(BusinessLib.Log.ILog log = null, BusinessLib.Cache.ICache cache = null, BusinessLib.Data.IData db = null, bool isLogRecord = true)
    {
        this.log = log;
        this.cache = cache;
        this.db = db;
        this.IsLogRecord = isLogRecord;
    }

    public interface IResult<DataType>
    {
        [ProtoBuf.ProtoMember(1, Name = "S")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "S")]
        System.Int32 State { get; set; }

        [ProtoBuf.ProtoMember(2, Name = "M")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "M")]
        System.String Message { get; set; }

        [ProtoBuf.ProtoMember(3, Name = "D")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "D")]
        DataType Data { get; set; }
    }
