namespace Business.Authentication
{
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class Session : ISession
    {
        public static implicit operator Session(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Session>(value);
        }
        public static implicit operator Session(byte[] value)
        {
            return Extensions.Help.ProtoBufDeserialize<Session>(value);
        }

        //user data
        [ProtoBuf.ProtoMember(1)]
        public virtual System.String Account { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public virtual System.String Password { get; set; }
        //front clinet
        [ProtoBuf.ProtoMember(3)]
        public virtual System.String Site { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public virtual System.String IP { get; set; }

        //sys set
        [ProtoBuf.ProtoMember(5)]
        public virtual System.String Key { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public virtual System.DateTime Time { get; set; }

        [ProtoBuf.ProtoMember(7)]
        public virtual System.Collections.Generic.List<string> Competences { get; set; }

        public ISession Clone()
        {
            return new Session { Site = Site, Account = Account, Password = Password, Key = Key, IP = IP, Time = Time, Competences = Competences };
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public virtual byte[] ToBytes()
        {
            return Extensions.Help.ProtoBufSerialize(this);
        }
    }

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class Session<DataType> : Session, ISession<DataType>
    {
        public static implicit operator Session<DataType>(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Session<DataType>>(value);
        }
        public static implicit operator Session<DataType>(byte[] value)
        {
            return Extensions.Help.ProtoBufDeserialize<Session<DataType>>(value);
        }

        //user data
        [ProtoBuf.ProtoMember(1)]
        public override System.String Account { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public override System.String Password { get; set; }
        //front clinet
        [ProtoBuf.ProtoMember(3)]
        public override System.String Site { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public override System.String IP { get; set; }

        //sys set
        [ProtoBuf.ProtoMember(5)]
        public override System.String Key { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public override System.DateTime Time { get; set; }

        [ProtoBuf.ProtoMember(7)]
        public override System.Collections.Generic.List<string> Competences { get; set; }

        [ProtoBuf.ProtoMember(8, AsReference = true)]
        public virtual DataType Data { get; set; }

        ISession ISession.Clone()
        {
            return this.Clone();
        }

        public new ISession<DataType> Clone()
        {
            return new Session<DataType> { Site = Site, Account = Account, Password = Password, Key = Key, IP = IP, Time = Time, Data = Data, Competences = Competences };
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public override byte[] ToBytes()
        {
            return Extensions.Help.ProtoBufSerialize(this);
        }
    }

    /*
    public class SysInfo : ISysInfo
    {
        public string OS { get; set; }

        public System.Collections.Generic.List<string> Mac { get; set; }

        public string Cpu { get; set; }

        public string Memory { get; set; }

        public string Board { get; set; }

        public string Disk { get; set; }

        public string Browser { get; set; }

        public string Sign { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public struct BoarStruct
    {
        public object SerialNumber;
        public object Manufacturer;
        public object Product;
        public object Version;
    }
    public struct OSStruct
    {
        public object Name;
        public object Version;
        public object SystemDirectory;
    }
    public struct DiskStruct
    {
        public object SerialNumber;
        public object Model;
        public object Size;
    }
    public struct CpuStruct
    {
        public object Name;
        public object ProcessorId;
        public object SystemName;
    }
    public struct MemoryStruct
    {
        public object TotalPhysicalMemory;
    }
    */
}