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

namespace Business.Cache
{
    public struct CacheValue
    {
        public CacheValue(byte[] value)
        {
            this.value = value;
            this.hasValue = null != value;
        }

        byte[] value;
        public byte[] Value { get { return value; } }

        bool hasValue;
        public bool HasValue { get { return hasValue; } }

        /// <summary>
        /// Creates a new CacheValue from a Boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CacheValue(bool value)
        {
            return new CacheValue(System.BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Creates a new CacheValue from a Byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CacheValue(byte[] value)
        {
            return new CacheValue(value);
        }
        /// <summary>
        /// Creates a new CacheValue from a Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CacheValue(double value)
        {
            return new CacheValue(System.BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Creates a new CacheValue from an Int16
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CacheValue(short value)
        {
            return new CacheValue(System.BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Creates a new CacheValue from an Int32
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CacheValue(int value)
        {
            return new CacheValue(System.BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Creates a new CacheValue from an Int64
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CacheValue(long value)
        {
            return new CacheValue(System.BitConverter.GetBytes(value));
        }
        /// <summary>
        /// Creates a new CacheValue from a String
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CacheValue(string value)
        {
            return new CacheValue(System.Text.Encoding.UTF8.GetBytes(value));
        }
        /// <summary>
        /// Creates a new CacheValue from a Char
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator CacheValue(char value)
        {
            return new CacheValue(System.BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Converts the value to a Bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator bool(CacheValue value)
        {
            return System.BitConverter.ToBoolean(value.Value, 0);
        }
        /// <summary>
        /// Converts the value to a byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator byte[](CacheValue value)
        {
            return value.Value;
        }
        /// <summary>
        /// Converts the value to a Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator double(CacheValue value)
        {
            return System.BitConverter.ToDouble(value.Value, 0);
        }
        /// <summary>
        /// Converts the value to a Int16
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator short(CacheValue value)
        {
            return System.BitConverter.ToInt16(value.Value, 0);
        }
        /// <summary>
        /// Converts the value to a Int32
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator int(CacheValue value)
        {
            return System.BitConverter.ToInt32(value.Value, 0);
        }
        /// <summary>
        /// Converts the value to a Int64
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator long(CacheValue value)
        {
            return System.BitConverter.ToInt64(value.Value, 0);
        }
        /// <summary>
        /// Converts the value to a String
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator string(CacheValue value)
        {
            return System.Text.Encoding.UTF8.GetString(value.Value);
        }
        /// <summary>
        /// Converts the value to a Char
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator char(CacheValue value)
        {
            return System.BitConverter.ToChar(value.Value, 0);
        }
    }
}
