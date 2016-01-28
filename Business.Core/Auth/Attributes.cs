namespace Business.Attributes
{
    using Result;

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public abstract class ArgumentsAttribute : System.Attribute
    {
        public bool TrimAllChar { get; set; }
    }

    public class DeserializeAttribute : ArgumentsAttribute
    {
        public virtual object Deserialize(object ags, System.Type type)
        {
            return ags;
        }
    }

    public class JsonAttribute : DeserializeAttribute
    {
        public override object Deserialize(object ags, System.Type type)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(System.Convert.ToString(ags), type);
            }
            catch { throw new System.Exception("Arguments Json deserialize error"); }
        }
    }

    public class ProtoBufAttribute : DeserializeAttribute
    {
        public override object Deserialize(object ags, System.Type type)
        {
            try
            {
                using (var stream = new System.IO.MemoryStream((byte[])ags))
                {
                    return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
                }
            }
            catch { throw new System.Exception("Arguments ProtoBuf deserialize error"); }
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ArgumentAttribute : System.Attribute { }

    public abstract class CheckedAttribute : ArgumentAttribute
    {
        public CheckedAttribute(int code, string message = null)
        {
            this.code = code;
            this.message = message;
        }

        readonly int code;
        public int Code { get { return code; } }

        readonly string message;
        public string Message { get { return message; } }

        public abstract Result Checked<Result>(System.Type type, object value, string name) where Result : class, IResult, new();
    }

    public class CanNotNullAttribute : CheckedAttribute
    {
        public CanNotNullAttribute(int code = -999, string message = null)
            : base(code, message) { }

        public override Result Checked<Result>(System.Type type, object value, string name = null)
        {
            if (null == value)
            {
                var msg = System.String.IsNullOrEmpty(Message) ? string.Format("argument \"{0}\" can not null.", name) : Message;
                return ResultFactory.Create<Result>(Code, msg);
            }
            return null;
        }
    }

    public class SizeAttribute : CheckedAttribute
    {
        public SizeAttribute(int code = -998, string message = null)
            : base(code, message) { }

        public object Min { get; set; }
        public object Max { get; set; }

        public override Result Checked<Result>(System.Type type, object value, string name)
        {
            if (null == value) { return null; }

            var msg = System.String.Empty;

            switch (type.FullName)
            {
                case "System.String":
                    var _ags1 = System.Convert.ToString(value).Trim();
                    if (null != Min && Extensions.Help.ChangeType<System.Int32>(Min) > _ags1.Length)
                    {
                        msg = string.Format("argument \"{0}\" minimum length value {1}.", name, Min);
                    }
                    if (null != Max && Extensions.Help.ChangeType<System.Int32>(Max) < _ags1.Length)
                    {
                        msg = string.Format("argument \"{0}\" maximum length value {1}.", name, Max);
                    }
                    break;
                case "System.DateTime":
                    var _ags2 = System.Convert.ToDateTime(value);
                    if (null != Min && Extensions.Help.ChangeType<System.DateTime>(Min) > _ags2)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Extensions.Help.ChangeType<System.DateTime>(Max) < _ags2)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                case "System.Int32":
                    var _ags3 = System.Convert.ToInt32(value);
                    if (null != Min && Extensions.Help.ChangeType<System.Int32>(Min) > _ags3)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Extensions.Help.ChangeType<System.Int32>(Max) < _ags3)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                case "System.Int64":
                    var _ags4 = System.Convert.ToInt64(value);
                    if (null != Min && Extensions.Help.ChangeType<System.Int64>(Min) > _ags4)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Extensions.Help.ChangeType<System.Int64>(Max) < _ags4)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                case "System.Decimal":
                    var _ags5 = System.Convert.ToDecimal(value);
                    if (null != Min && Extensions.Help.ChangeType<System.Decimal>(Min) > _ags5)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Extensions.Help.ChangeType<System.Decimal>(Max) < _ags5)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                case "System.Double":
                    var _ags6 = System.Convert.ToDouble(value);
                    if (null != Min && Extensions.Help.ChangeType<System.Double>(Min) > _ags6)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Extensions.Help.ChangeType<System.Double>(Max) < _ags6)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                default:
                    var iList = type.GetInterface("System.Collections.IList");
                    if (null != iList)
                    {
                        var list = value as System.Collections.IList;
                        if (null != Min && Extensions.Help.ChangeType<System.Int32>(Min) > list.Count)
                        {
                            msg = string.Format("argument \"{0}\" minimum count value {1}.", name, Min);
                        }
                        if (null != Max && Extensions.Help.ChangeType<System.Int32>(Max) < list.Count)
                        {
                            msg = string.Format("argument \"{0}\" maximum count value {1}.", name, Max);
                        }
                    }
                    break;
            }

            if (!System.String.IsNullOrEmpty(msg))
            {
                msg = System.String.IsNullOrEmpty(Message) ? msg : Message;
                return ResultFactory.Create<Result>(Code, msg);
            }
            return null;
        }
    }

    public class ScaleAttribute : CheckedAttribute
    {
        public ScaleAttribute(int code = -997, string message = null)
            : base(code, message) { }

        public int Scale { get; set; }

        public override Result Checked<Result>(System.Type type, object value, string name)
        {
            throw new System.NotImplementedException();
        }
    }

    public class CheckEmailAttribute : CheckedAttribute
    {
        public CheckEmailAttribute(int code = -996, string message = null)
            : base(code, message) { }

        public override Result Checked<Result>(System.Type type, object value, string name)
        {
            if (null == value) { return null; }

            var _value = System.Convert.ToString(value).Trim();
            if (!System.String.IsNullOrEmpty(_value) && !Extensions.Help.CheckEmail(_value))
            {
                var msg = System.String.IsNullOrEmpty(Message) ? string.Format("argument \"{0}\" email error.", name) : Message;
                return ResultFactory.Create<Result>(Code, msg);
            }
            return null;
        }
    }

    public class CheckCharAttribute : CheckedAttribute
    {
        public CheckCharAttribute(int code = -995, string message = null)
            : base(code, message) { }

        CheckCharMode mode = CheckCharMode.All;
        public CheckCharMode Mode { get { return mode; } set { mode = value; } }

        public override Result Checked<Result>(System.Type type, object value, string name)
        {
            if (null == value) { return null; }

            var _value = System.Convert.ToString(value).Trim();
            if (!System.String.IsNullOrEmpty(_value) && !CheckChar(_value, Mode))
            {
                var msg = System.String.IsNullOrEmpty(Message) ? string.Format("argument \"{0}\" char verification failed.", name) : Message;
                return ResultFactory.Create<Result>(Code, msg);
            }
            return null;
        }

        [System.Flags]
        public enum CheckCharMode
        {
            All = 0,
            Number = 2,
            Upper = 4,
            Lower = 8,
            Chinese = 16
        }

        public static bool CheckChar(string value, CheckCharMode mode = CheckCharMode.All)
        {
            if (null == value || System.String.IsNullOrEmpty(value)) { return false; }

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
                number(c) &&
                upper(c) &&
                lower(c) &&
                chinese(c));
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
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandAttribute : System.Attribute
    {
        public CommandAttribute(string onlyName = null)
        {
            this.OnlyName = onlyName;
        }

        public enum DataType
        {
            Byte,
            ProtoBuf,
            Json,
        }

        public string OnlyName { get; internal set; }

        DataType resultDataType = DataType.ProtoBuf;
        public DataType ResultDataType { get { return resultDataType; } set { resultDataType = value; } }

        public virtual object Deserialize(object ags, System.Type type)
        {
            return ags;
        }
    }

    public class JsonCommandAttribute : CommandAttribute
    {
        public JsonCommandAttribute(string onlyName = null) : base(onlyName) { }

        public override object Deserialize(object ags, System.Type type)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(System.Convert.ToString(ags), type);
            }
            catch { throw new System.Exception("Arguments Json deserialize error"); }
        }
    }

    public class ProtoBufCommandAttribute : CommandAttribute
    {
        public ProtoBufCommandAttribute(string onlyName = null) : base(onlyName) { }

        public override object Deserialize(object ags, System.Type type)
        {
            try
            {
                using (var stream = new System.IO.MemoryStream((byte[])ags))
                {
                    return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
                }
            }
            catch { throw new System.Exception("Arguments ProtoBuf deserialize error"); }
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BusinessLogAttribute : System.Attribute
    {
        public BusinessLogAttribute(bool notRecord = false)
        {
            this.notRecord = notRecord;
        }

        readonly bool notRecord;
        public bool NotRecord { get { return notRecord; } }

        public bool NotValue { get; set; }

        public bool NotResult { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotInterceptAttribute : System.Attribute { }
}
