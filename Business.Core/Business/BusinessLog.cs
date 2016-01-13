namespace Business
{
    public enum BusinessLogType
    {
        /// <summary>
        /// Exception = 0
        /// </summary>
        Exception = 0,
        /// <summary>
        /// Record = 1
        /// </summary>
        Record = 1
    }

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public struct BusinessLogData : Authentication.ISerialize, System.IEquatable<BusinessLogData>
    {
        public static implicit operator BusinessLogData(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<BusinessLogData>(value);
        }
        public static implicit operator BusinessLogData(byte[] value)
        {
            return Extensions.Help.ProtoBufDeserialize<BusinessLogData>(value);
        }

        public bool Equals(BusinessLogData other) { return other.Guid.Equals(this.Guid); }

        public BusinessLogData(BusinessLogType type, string session, string account, object value, object result = null, double time = 0, [System.Runtime.CompilerServices.CallerMemberName] string member = null, string ip = null, string description = null)
        {
            this.guid = System.Guid.NewGuid().ToString("N");
            this.type = type;
            this.session = session;
            this.account = account;
            this.value = value;
            this.result = result;
            this.time = time;
            this.member = member;
            this.ip = ip;
            this.description = description;
        }

        string guid;
        [ProtoBuf.ProtoMember(1)]
        public string Guid { get { return guid; } }

        BusinessLogType type;
        [ProtoBuf.ProtoMember(2)]
        public BusinessLogType Type { get { return type; } }

        string session;
        [ProtoBuf.ProtoMember(3)]
        public string Session { get { return session; } }

        string account;
        [ProtoBuf.ProtoMember(4)]
        public string Account { get { return account; } }

        object value;
        [ProtoBuf.ProtoMember(5, AsReference = true)]
        public object Value { get { return value; } }

        object result;
        [ProtoBuf.ProtoMember(6, AsReference = true)]
        public object Result { get { return result; } }

        double time;
        [ProtoBuf.ProtoMember(7)]
        public double Time { get { return time; } }

        string member;
        [ProtoBuf.ProtoMember(8)]
        public string Member { get { return member; } }

        string ip;
        [ProtoBuf.ProtoMember(9)]
        public string IP { get { return ip; } }

        string description;
        [ProtoBuf.ProtoMember(10)]
        public string Description { get { return description; } }

        public override string ToString() { return Newtonsoft.Json.JsonConvert.SerializeObject(this); }

        public byte[] ToBytes() { return Extensions.Help.ProtoBufSerialize(this); }
    }
}