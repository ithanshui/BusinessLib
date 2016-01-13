namespace Business.Authentication
{
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
}
