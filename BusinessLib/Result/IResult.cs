namespace BusinessLib.Result
{
    public interface IResult
    {
        System.Int32 State { get; set; }

        System.String Message { get; set; }

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
        DataType Data { get; set; }
    }
}