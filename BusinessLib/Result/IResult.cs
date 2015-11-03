namespace BusinessLib.Result
{
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
}