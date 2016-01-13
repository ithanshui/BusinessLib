namespace Business.Authentication
{
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public class Token : IToken
    {
        public static implicit operator Token(string value) { return Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(value); }
        public static implicit operator Token(byte[] value) { return Extensions.Help.ProtoBufDeserialize<Token>(value); }

        [ProtoBuf.ProtoMember(1)]
        public System.String Key { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public System.String IP { get; set; }

        public override string ToString() { return Newtonsoft.Json.JsonConvert.SerializeObject(this); }

        public byte[] ToBytes() { return Extensions.Help.ProtoBufSerialize(this); }
    }
}
