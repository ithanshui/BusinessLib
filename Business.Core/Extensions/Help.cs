namespace Business.Extensions
{
    public static class Help
    {
        public static System.IO.MemoryStream StreamCopy(this System.IO.Stream stream)
        {
            var outStream = new System.IO.MemoryStream();
            const int bufferLen = 4096;//4k size
            var buffer = new byte[bufferLen];
            int count = 0;
            while ((count = stream.Read(buffer, 0, bufferLen)) > 0) { outStream.Write(buffer, 0, count); }
            return outStream;
        }
        /// <summary>
        /// gzip to byte[]
        /// </summary>
        /// <param name="value">byte[]</param>
        /// <returns>byte[]</returns>
        public static System.Byte[] GZipCompressByte(this System.Byte[] value)
        {
            if (null == value) { throw new System.ArgumentNullException("value"); }
            using (var m = new System.IO.MemoryStream())
            {
                //序列化压缩时要在返回字节数组时必须先关闭压缩对象，不然小于4k大小的字节数组会解压不出来就会产生空置的问题
                using (var g = new System.IO.Compression.GZipStream(m, System.IO.Compression.CompressionMode.Compress)) { g.Write(value, 0, value.Length); }
                return m.GetBuffer();
            }
        }
        /// <summary>
        /// gzip to byte[]
        /// </summary>
        /// <param name="value">byte[]</param>
        /// <returns>byte[]</returns>
        public static System.Byte[] GZipDecompressByte(this System.Byte[] value)
        {
            using (var m = GZipDecompressStream(value)) { return m.ToArray(); }
        }
        /// <summary>
        /// gzip to byte[]
        /// </summary>
        /// <param name="value">byte[]</param>
        /// <returns>MemoryStream</returns>
        public static System.IO.MemoryStream GZipDecompressStream(this System.Byte[] value)
        {
            using (var m = new System.IO.MemoryStream(value))
            {
                m.Seek(0, System.IO.SeekOrigin.Begin);
                using (var g = new System.IO.Compression.GZipStream(m, System.IO.Compression.CompressionMode.Decompress, true))
                {
                    return StreamCopy(g);
                }
            }
        }

        public static string MD5Encoding(this string str, string encodingNmae = "UTF-8", bool isUpper = false)
        {
            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                var result = System.BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.GetEncoding(encodingNmae).GetBytes(str))).Replace("-", System.String.Empty);
                return isUpper ? result.ToUpperInvariant() : result.ToLowerInvariant();
            }
        }

        public static long ConvertTime(this System.DateTime time)
        {
            return (time.Ticks - System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks) / 10000;//除10000调整为13位
            //return (time.ToUniversalTime().Ticks - System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks) / 10000;
        }
        public static System.DateTime ConvertTime(this long time)
        {
            //return new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(time);
            //10位处理 补毫秒000
            //(time + 8 * 60 * 60) * 10000000 + System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks;
            return new System.DateTime(System.Convert.ToInt64(time.ToString().PadRight(13, '0')) * 10000 + System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks);
        }
        /*
        public static string EncryptDES(this string str, string key, string iv = IV)
        {
            var rgbKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            var rgbIV = System.Text.Encoding.UTF8.GetBytes(iv.Substring(0, 8));
            var inputByteArray = System.Text.Encoding.UTF8.GetBytes(str);
            using (var des = new System.Security.Cryptography.DESCryptoServiceProvider())
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(rgbKey, rgbIV), System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        return System.Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public static string DecryptDES(this string str, string key, string iv = IV)
        {
            var rgbKey = System.Text.Encoding.UTF8.GetBytes(key);
            var rgbIV = System.Text.Encoding.UTF8.GetBytes(iv.Substring(0, 8));
            var inputByteArray = System.Convert.FromBase64String(str);
            using (var des = new System.Security.Cryptography.DESCryptoServiceProvider())
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(rgbKey, rgbIV), System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        return System.Text.Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
        */
        //public void dsa()
        //{
        //    var aes = System.Security.Cryptography.Rijndael.Create().;
        //    using (var cStream = new System.Security.Cryptography.CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), System.Security.Cryptography.CryptoStreamMode.Write))
        //    {
        //        cStream.Write(byteArray, 0, byteArray.Length);
        //        cStream.FlushFinalBlock();
        //        encrypt = Convert.ToBase64String(mStream.ToArray());
        //    }
        //}

        [System.Flags]
        public enum CheckCharMode
        {
            /// <summary>
            /// 限制为 阿拉伯数字 大写字母 小写字母 中文
            /// </summary>
            All = 0,
            /// <summary>
            /// 限制为 阿拉伯数字
            /// </summary>
            Number = 2,
            /// <summary>
            /// 限制为 大写字母
            /// </summary>
            Upper = 4,
            /// <summary>
            /// 限制为 小写字母
            /// </summary>
            Lower = 8,
            /// <summary>
            /// 限制为 中文
            /// </summary>
            Chinese = 16
        }

        /// <summary>
        /// 限制为 阿拉伯数字 大写字母 小写字母 中文
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool CheckChar(this string value, CheckCharMode mode = CheckCharMode.All)
        {
            if (null == value || System.String.IsNullOrEmpty(value)) { return false; }

            //if (0 < length && length < value.Length) { return false; }
            /*==================================================================
             小于33的为控制符 33-47 为半角符号 48-57 为阿拉伯数字 58-64为半角符号
             65-90为大写字母 91-96为半角符号 97 -122为小写字母 123- 126半角符号 127控制符 大于127为中文
             ==================================================================*/
            var list = new System.Collections.Generic.List<int>();
            for (int i = 0; i < value.Length; i++) { list.Add(value[i]); }

            System.Predicate<int> number = delegate(int c) { return !(c >= 48 && c <= 57); };
            System.Predicate<int> upper = delegate(int c) { return !(c >= 65 && c <= 90); };
            System.Predicate<int> lower = delegate(int c) { return !(c >= 97 && c <= 122); };
            System.Predicate<int> chinese = delegate(int c) { return !(c >= 0x4e00 && c <= 0x9fbb); };

            switch (mode)
            {
                case CheckCharMode.All:
                    return !list.Exists(c =>
                number(c) && //阿拉伯数字
                upper(c) && //大写字母
                lower(c) && //小写字母
                chinese(c));//中文
                case CheckCharMode.Number:
                    return !list.Exists(c => number(c));
                case CheckCharMode.Upper:
                    return !list.Exists(c => upper(c));
                case CheckCharMode.Lower:
                    return !list.Exists(c => lower(c));
                case CheckCharMode.Chinese:
                    return !list.Exists(c => chinese(c));
                //==============Number==============//
                case CheckCharMode.Number | CheckCharMode.Upper | CheckCharMode.Lower:
                    return !list.Exists(c => number(c) && upper(c) && lower(c));
                case CheckCharMode.Number | CheckCharMode.Upper | CheckCharMode.Chinese:
                    return !list.Exists(c => number(c) && upper(c) && chinese(c));
                case CheckCharMode.Number | CheckCharMode.Lower | CheckCharMode.Chinese:
                    return !list.Exists(c => number(c) && lower(c) && chinese(c));
                case CheckCharMode.Number | CheckCharMode.Upper:
                    return !list.Exists(c => number(c) && upper(c));
                case CheckCharMode.Number | CheckCharMode.Lower:
                    return !list.Exists(c => number(c) && lower(c));
                case CheckCharMode.Number | CheckCharMode.Chinese:
                    return !list.Exists(c => number(c) && chinese(c));
                //==============Upper==============//
                case CheckCharMode.Upper | CheckCharMode.Lower | CheckCharMode.Chinese:
                    return !list.Exists(c => upper(c) && lower(c) && chinese(c));
                case CheckCharMode.Upper | CheckCharMode.Lower:
                    return !list.Exists(c => upper(c) && lower(c));
                case CheckCharMode.Upper | CheckCharMode.Chinese:
                    return !list.Exists(c => upper(c) && chinese(c));
                //==============Lower==============//
                case CheckCharMode.Lower | CheckCharMode.Chinese:
                    return !list.Exists(c => lower(c) && chinese(c));
                case CheckCharMode.Number | CheckCharMode.Upper | CheckCharMode.Lower | CheckCharMode.Chinese:
                    return !list.Exists(c => number(c) && upper(c) && lower(c) && chinese(c));
                default: return false;
            }
        }

        /*
        public static BusinessLib.Result.IResult CheckObject<T>(this T ags, string memberName = null, int state = -999, object min = null, object max = null)
        {
            var type = ags.GetType();

            switch (type.FullName)
            {
                case "System.String":
                    var _ags1 = null == ags ? System.String.Empty
                        : System.Convert.ToString(ags).Trim();
                    if (null != min && System.Convert.ToInt32(min) > _ags1.Length)
                    {
                        return BusinessLib.Result.ResultFactory.Create(state, string.Format("\"{0}\" minimum length exceeds", memberName));
                    }
                    if (null != max && System.Convert.ToInt32(max) < _ags1.Length)
                    {
                        return BusinessLib.Result.ResultFactory.Create(state, string.Format("\"{0}\" maximum length exceeds", memberName));
                    }
                    return BusinessLib.Result.ResultFactory.Create<T>(BusinessLib.Extensions.Help.ChangeType<T>(_ags1));
                case "System.DateTime":
                    var _ags2 = System.Convert.ToDateTime(ags);
                    if (System.Data.SqlTypes.SqlDateTime.MinValue.Value > _ags2)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" minimum value error", memberName));
                    }
                    return BusinessLib.Result.ResultExtensions.Result<T>(BusinessLib.Extensions.Help.ChangeType<T>(_ags2));
                case "System.Int32":
                    var _ags3 = System.Convert.ToInt32(ags);
                    if (null != min && System.Convert.ToInt32(min) > _ags3)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" minimum length exceeds", memberName));
                    }
                    if (null != max && System.Convert.ToInt32(max) < _ags3)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" maximum length exceeds", memberName));
                    }
                    return BusinessLib.Result.ResultExtensions.Result<T>(BusinessLib.Extensions.Help.ChangeType<T>(_ags3));
                case "System.Int64":
                    var _ags4 = System.Convert.ToInt64(ags);
                    if (null != min && System.Convert.ToInt64(min) > _ags4)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" minimum length exceeds", memberName));
                    }
                    if (null != max && System.Convert.ToInt64(max) < _ags4)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" maximum length exceeds", memberName));
                    }
                    return BusinessLib.Result.ResultExtensions.Result<T>(BusinessLib.Extensions.Help.ChangeType<T>(_ags4));
                case "System.Decimal":
                    var _ags5 = System.Convert.ToDecimal(ags);
                    if (null != min && System.Convert.ToDecimal(min) > _ags5)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" minimum length exceeds", memberName));
                    }
                    if (null != max && System.Convert.ToDecimal(max) < _ags5)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" maximum length exceeds", memberName));
                    }
                    return BusinessLib.Result.ResultExtensions.Result<T>(BusinessLib.Extensions.Help.ChangeType<T>(_ags5));
                case "System.Double":
                    var _ags6 = System.Convert.ToDouble(ags);
                    if (null != min && System.Convert.ToDouble(min) > _ags6)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" minimum length exceeds", memberName));
                    }
                    if (null != max && System.Convert.ToDouble(max) < _ags6)
                    {
                        return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" maximum length exceeds", memberName));
                    }
                    return BusinessLib.Result.ResultExtensions.Result<T>(BusinessLib.Extensions.Help.ChangeType<T>(_ags6));
                default:
                    return BusinessLib.Result.ResultExtensions.Result(state, string.Format("\"{0}\" unknown error", memberName));
            }
        }
        */

        public static string NewGuidNumber()
        {
            return System.BitConverter.ToUInt32(System.Guid.NewGuid().ToByteArray(), 0).ToString();
        }

        public static void MailSend(this string subject, string content, string from, string displayName, string host, string credentialsUserName, string credentialsPassword, int port = 25, bool enableSsl = false, System.Text.Encoding contentEncoding = null, string mediaType = "text/html", params string[] to)
        {
            using (var mailMsg = new System.Net.Mail.MailMessage())
            {
                mailMsg.From = new System.Net.Mail.MailAddress(from, displayName);
                foreach (var item in to) { mailMsg.CC.Add(item); }
                mailMsg.Subject = subject;
                using (var view = System.Net.Mail.AlternateView.CreateAlternateViewFromString(content, contentEncoding, mediaType))
                {
                    mailMsg.AlternateViews.Add(view);
                    using (var smtpClient = new System.Net.Mail.SmtpClient(host, port))
                    {
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.Credentials = new System.Net.NetworkCredential(credentialsUserName, credentialsPassword);
                        smtpClient.Send(mailMsg);
                    }
                }
            };
        }

        public static string GetMethodFullName(this System.Reflection.MethodInfo methodInfo)
        {
            return string.Format("{0}.{1}", methodInfo.DeclaringType.FullName, methodInfo.Name);
        }

        public static Type ChangeType<Type>(this object value)
        {
            return (Type)ChangeType(value, typeof(Type));
        }

        public static object ChangeType(this object value, System.Type type)
        {
            if (null == value) { return System.Activator.CreateInstance(type); }

            try
            {
                return System.Convert.ChangeType(value, type);
            }
            catch { return System.Activator.CreateInstance(type); }
        }

        public static Type JsonDeserialize<Type>(this string value)
        {
            try { return Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(value); }
            catch { return default(Type); }
        }

        public static Type JsonDeserialize<Type>(this string value, out string error)
        {
            error = null;

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(value);
            }
            catch (System.Exception ex)
            {
                error = System.Convert.ToString(ex);
                return default(Type);
            }
        }
        public static object JsonDeserialize(this string value, System.Type type, out string error)
        {
            error = null;

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
            }
            catch (System.Exception ex)
            {
                error = System.Convert.ToString(ex);
                return null;
            }
        }

        //public static int GetRandomSeed()
        //{
        //    var bytes = new byte[4];
        //    var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        //    rng.GetBytes(bytes);
        //    return System.BitConverter.ToInt32(bytes, 0);
        //}

        public static int Random(int minValue, int maxValue)
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return new System.Random(System.BitConverter.ToInt32(bytes, 0)).Next(minValue, maxValue);
            }
        }
        public static int Random(this int maxValue)
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return new System.Random(System.BitConverter.ToInt32(bytes, 0)).Next(maxValue);
            }
        }

        public static bool CheckEmail(this string email)
        {
            var _email = null == email ? System.String.Empty : email.Trim();

            if (System.String.IsNullOrEmpty(_email))
            {
                return false;
            }

            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        }

        #region ProtoBuf Serialize
        public static T ProtoBufDeserialize<T>(this System.Byte[] source)
        {
            using (var stream = new System.IO.MemoryStream(source))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }
        public static System.Byte[] ProtoBufSerialize<T>(this T instance)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, instance);
                return stream.ToArray();
            }
        }
        public static object ProtoBufDeserialize(this byte[] source, System.Type type)
        {
            using (var stream = new System.IO.MemoryStream(source))
            {
                return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
            }
        }

        #endregion
    }

    #region Equals

    public static class Equality<T>
    {
        public static System.Collections.Generic.IEqualityComparer<T> CreateComparer<V>(System.Func<T, V> keySelector)
        {
            return new CommonEqualityComparer<V>(keySelector);
        }
        public static System.Collections.Generic.IEqualityComparer<T> CreateComparer<V>(System.Func<T, V> keySelector, System.Collections.Generic.IEqualityComparer<V> comparer)
        {
            return new CommonEqualityComparer<V>(keySelector, comparer);
        }

        class CommonEqualityComparer<V> : System.Collections.Generic.IEqualityComparer<T>
        {
            private System.Func<T, V> keySelector;
            private System.Collections.Generic.IEqualityComparer<V> comparer;

            public CommonEqualityComparer(System.Func<T, V> keySelector, System.Collections.Generic.IEqualityComparer<V> comparer)
            {
                this.keySelector = keySelector;
                this.comparer = comparer;
            }
            public CommonEqualityComparer(System.Func<T, V> keySelector)
                : this(keySelector, System.Collections.Generic.EqualityComparer<V>.Default)
            { }

            public bool Equals(T x, T y)
            {
                return comparer.Equals(keySelector(x), keySelector(y));
            }
            public int GetHashCode(T obj)
            {
                return comparer.GetHashCode(keySelector(obj));
            }
        }
    }

    public static class ComparisonHelper<T>
    {
        public static System.Collections.Generic.IComparer<T> CreateComparer<V>(System.Func<T, V> keySelector)
        {
            return new CommonComparer<V>(keySelector);
        }
        public static System.Collections.Generic.IComparer<T> CreateComparer<V>(System.Func<T, V> keySelector, System.Collections.Generic.IComparer<V> comparer)
        {
            return new CommonComparer<V>(keySelector, comparer);
        }

        class CommonComparer<V> : System.Collections.Generic.IComparer<T>
        {
            private System.Func<T, V> keySelector;
            private System.Collections.Generic.IComparer<V> comparer;

            public CommonComparer(System.Func<T, V> keySelector, System.Collections.Generic.IComparer<V> comparer)
            {
                this.keySelector = keySelector;
                this.comparer = comparer;
            }
            public CommonComparer(System.Func<T, V> keySelector)
                : this(keySelector, System.Collections.Generic.Comparer<V>.Default)
            { }

            public int Compare(T x, T y)
            {
                return comparer.Compare(keySelector(x), keySelector(y));
            }
        }
    }

    #endregion

}