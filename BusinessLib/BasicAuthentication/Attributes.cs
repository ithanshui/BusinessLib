namespace BusinessLib.Attributes
{
    using BusinessLib.Extensions;
    using System.Linq;
    
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public class ArgumentsAttribute : System.Attribute
    {
        public bool TrimAllChar { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ArgumentAttribute : System.Attribute { }

    public abstract class CheckedAttribute : ArgumentAttribute
    {
        public abstract int Code { get; set; }
        public string Message { get; set; }

        public abstract BusinessLib.Result.IResult Checked(System.Type type, object value, string name);
    }

    public class CanNotNullAttribute : CheckedAttribute
    {
        int code = -999;
        public override int Code { get { return code; } set { code = value; } }

        public override Result.IResult Checked(System.Type type, object value, string name = null)
        {
            if (null == value)
            {
                var msg = System.String.IsNullOrEmpty(Message) ? string.Format("argument \"{0}\" can not null.", name) : Message;
                return BusinessLib.Result.ResultExtensions.Result(Code, msg);
            }
            return null;
        }
    }

    public class SizeAttribute : CheckedAttribute
    {
        int code = -998;
        public override int Code { get { return code; } set { code = value; } }

        public object Min { get; set; }
        public object Max { get; set; }

        public override Result.IResult Checked(System.Type type, object value, string name)
        {
            if (null == value) { return null; }

            var msg = System.String.Empty;

            switch (type.FullName)
            {
                case "System.String":
                    var _ags1 = System.Convert.ToString(value).Trim();
                    if (null != Min && Help.ChangeType<System.Int32>(Min) > _ags1.Length)
                    {
                        msg = string.Format("argument \"{0}\" minimum length value {1}.", name, Min);
                    }
                    if (null != Max && Help.ChangeType<System.Int32>(Max) < _ags1.Length)
                    {
                        msg = string.Format("argument \"{0}\" maximum length value {1}.", name, Max);
                    }
                    break;
                case "System.DateTime":
                    var _ags2 = System.Convert.ToDateTime(value);
                    if (null != Min && Help.ChangeType<System.DateTime>(Min) > _ags2)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Help.ChangeType<System.DateTime>(Max) < _ags2)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                case "System.Int32":
                    var _ags3 = System.Convert.ToInt32(value);
                    if (null != Min && Help.ChangeType<System.Int32>(Min) > _ags3)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Help.ChangeType<System.Int32>(Max) < _ags3)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                case "System.Int64":
                    var _ags4 = System.Convert.ToInt64(value);
                    if (null != Min && Help.ChangeType<System.Int64>(Min) > _ags4)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Help.ChangeType<System.Int64>(Max) < _ags4)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                case "System.Decimal":
                    var _ags5 = System.Convert.ToDecimal(value);
                    if (null != Min && Help.ChangeType<System.Decimal>(Min) > _ags5)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Help.ChangeType<System.Decimal>(Max) < _ags5)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                case "System.Double":
                    var _ags6 = System.Convert.ToDouble(value);
                    if (null != Min && Help.ChangeType<System.Double>(Min) > _ags6)
                    {
                        msg = string.Format("argument \"{0}\" minimum value {1}.", name, Min);
                    }
                    if (null != Max && Help.ChangeType<System.Double>(Max) < _ags6)
                    {
                        msg = string.Format("argument \"{0}\" maximum value {1}.", name, Max);
                    }
                    break;
                default:
                    var iList = type.GetInterface("System.Collections.IList");
                    if (null != iList)
                    {
                        var list = value as System.Collections.IList;
                        if (null != Min && Help.ChangeType<System.Int32>(Min) > list.Count)
                        {
                            msg = string.Format("argument \"{0}\" minimum count value {1}.", name, Min);
                        }
                        if (null != Max && Help.ChangeType<System.Int32>(Max) < list.Count)
                        {
                            msg = string.Format("argument \"{0}\" maximum count value {1}.", name, Max);
                        }
                    }
                    break;
            }

            if (!System.String.IsNullOrEmpty(msg))
            {
                msg = System.String.IsNullOrEmpty(Message) ? msg : Message;
                return BusinessLib.Result.ResultExtensions.Result(Code, msg);
            }
            return null;
        }
    }

    public class ScaleAttribute : CheckedAttribute
    {
        int code = -997;
        public override int Code { get { return code; } set { code = value; } }

        public int Scale { get; set; }

        public override Result.IResult Checked(System.Type type, object value, string name = null)
        {
            return null;
        }
    }

    public class CheckEmailAttribute : CheckedAttribute
    {
        int code = -996;
        public override int Code { get { return code; } set { code = value; } }

        public override Result.IResult Checked(System.Type type, object value, string name)
        {
            if (null == value) { return null; }

            var _value = System.Convert.ToString(value).Trim();
            if (!System.String.IsNullOrEmpty(_value) && !_value.CheckEmail())
            {
                var msg = System.String.IsNullOrEmpty(Message) ? string.Format("argument \"{0}\" email error.", name) : Message;
                return BusinessLib.Result.ResultExtensions.Result(Code, msg);
            }
            return null;
        }
    }

    public class CheckCharAttribute : CheckedAttribute
    {
        int code = -995;
        public override int Code { get { return code; } set { code = value; } }

        BusinessLib.Extensions.Help.CheckCharMode mode = Extensions.Help.CheckCharMode.All;
        public BusinessLib.Extensions.Help.CheckCharMode Mode { get { return mode; } set { mode = value; } }

        public override Result.IResult Checked(System.Type type, object value, string name)
        {
            if (null == value) { return null; }

            var _value = System.Convert.ToString(value).Trim();
            if (!System.String.IsNullOrEmpty(_value) && !_value.CheckChar(Mode))
            {
                var msg = System.String.IsNullOrEmpty(Message) ? string.Format("argument \"{0}\" char verification failed.", name) : Message;
                return BusinessLib.Result.ResultExtensions.Result(Code, msg);
            }
            return null;
        }
    }

    //public class LogRecordAttribute : ArgumentAttribute
    //{
    //    public bool AllowNotRecord { get; set; }
    //}
}
