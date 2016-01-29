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

namespace Business.Extensions
{
    public class BusinessAllMethodsHook : Castle.DynamicProxy.AllMethodsHook
    {
        readonly System.Reflection.MethodInfo[] notInterceptMethods;

        public BusinessAllMethodsHook(params System.Reflection.MethodInfo[] method)
            : base() { this.notInterceptMethods = method; }

        public override bool ShouldInterceptMethod(System.Type type, System.Reflection.MethodInfo methodInfo)
        {
            if (System.Array.Exists(notInterceptMethods, c => c.GetMethodFullName().Equals(methodInfo.GetMethodFullName()))) { return false; }

            return base.ShouldInterceptMethod(type, methodInfo);
        }
    }

    public class InterceptorBind<T> : Auth.IInterception, System.IDisposable
        where T : class, Business.IBusiness
    {
        readonly BusinessAllMethodsHook hook;
        readonly Castle.DynamicProxy.ProxyGenerator ProxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
        public readonly T Instance;

        public InterceptorBind(Auth.InterceptorBase interceptor)
        {
            var type = typeof(T);

            var methods = GetMethods(type);

            var notIntercept = NotIntercept(methods);

            hook = new BusinessAllMethodsHook(notIntercept.Item1);

            Instance = ProxyGenerator.CreateClassProxy<T>(new Castle.DynamicProxy.ProxyGenerationOptions(hook), interceptor);

            this.Member = GetInterceptorMember(notIntercept.Item2, Instance);
            this.Command = GetInterceptorCommand(notIntercept.Item2, Instance);
            this.Business = Instance;

            interceptor.MetaData = GetInterceptorMetaData(notIntercept.Item2);
            interceptor.Business = this.Business;
            interceptor.BusinessLogAttr = LogAttr(type);
        }

        public void Dispose()
        {
            if (typeof(System.IDisposable).IsAssignableFrom(Instance.GetType()))
            {
                ((System.IDisposable)Instance).Dispose();
            }
        }

        public System.Collections.Concurrent.ConcurrentDictionary<string, System.Func<object, object, Result.IResult>> Member { get; set; }

        public System.Collections.Concurrent.ConcurrentDictionary<string, InterceptorCommand> Command { get; set; }

        public IBusiness Business { get; set; }

        static System.Tuple<System.Reflection.MethodInfo[], System.Reflection.MethodInfo[]> NotIntercept(System.Reflection.MethodInfo[] methods)
        {
            var notList = new System.Collections.Generic.List<System.Reflection.MethodInfo>();
            var list = new System.Collections.Generic.List<System.Reflection.MethodInfo>();
            foreach (var item in methods)
            {
                var notInterceptAttrs = GetAttributes<Attributes.NotInterceptAttribute>(item);
                if (0 < notInterceptAttrs.Length)
                {
                    notList.Add(item);
                }
                else
                {
                    list.Add(item);
                }
            }
            return System.Tuple.Create(notList.ToArray(), list.ToArray());
        }

        static Attributes.BusinessLogAttribute LogAttr(System.Reflection.ICustomAttributeProvider member)
        {
            Attributes.BusinessLogAttribute logAttr;
            var logAttrs = GetAttributes<Attributes.BusinessLogAttribute>(member);
            logAttr = 0 < logAttrs.Length ? logAttrs[0] : new Attributes.BusinessLogAttribute();
            return logAttr;
        }

        static Attributes.CommandAttribute CmdAttr(System.Reflection.ICustomAttributeProvider member, string name)
        {
            Attributes.CommandAttribute commandAttr = null;
            var commandAttrs = GetAttributes<Attributes.CommandAttribute>(member);
            commandAttr = 0 < commandAttrs.Length ? commandAttrs[0] : new Attributes.CommandAttribute(name);
            if (System.String.IsNullOrEmpty(commandAttr.OnlyName)) { commandAttr.OnlyName = name; }
            return commandAttr;
        }

        static object[] GetAgsObj(object[] agsObj, object token, object arguments = null) { agsObj[0] = token; if (1 < agsObj.Length && null != arguments) { agsObj[1] = arguments; } return agsObj; }

        static System.Tuple<System.Type[], System.Object[]> GetParameters(System.Reflection.MethodInfo method)
        {
            var ags = method.GetParameters();
            var agsTypes = new System.Type[ags.Length];
            var agsObjs = new object[ags.Length];
            for (int i1 = 0; i1 < ags.Length; i1++)
            {
                var _type = ags[i1].ParameterType;
                agsTypes[i1] = _type;
                if (ags[i1].HasDefaultValue && _type.IsValueType && (System.DBNull.Value != ags[i1].DefaultValue || null == ags[i1].DefaultValue))
                {
                    agsObjs[i1] = System.Activator.CreateInstance(_type);
                }
                else if (System.DBNull.Value != ags[i1].DefaultValue && null != ags[i1].DefaultValue)
                {
                    agsObjs[i1] = ags[i1].DefaultValue;
                }
            }

            return System.Tuple.Create(agsTypes, agsObjs);
        }

        static System.Reflection.MethodInfo[] GetMethods(System.Type type)
        {
            return System.Array.FindAll(type.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic), c => c.IsVirtual && !c.IsFinal && !IsFinalize(c) && !IsGetType(c) && !IsMemberwiseClone(c) && !IsEquals(c) && !IsGetHashCode(c) && !IsToString(c));
        }

        #region

        static bool IsFinalize(System.Reflection.MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType == typeof(object) && string.Equals("Finalize", methodInfo.Name, System.StringComparison.OrdinalIgnoreCase);
        }

        static bool IsGetType(System.Reflection.MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType == typeof(object) && string.Equals("GetType", methodInfo.Name, System.StringComparison.OrdinalIgnoreCase);
        }

        static bool IsMemberwiseClone(System.Reflection.MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType == typeof(object) && string.Equals("MemberwiseClone", methodInfo.Name, System.StringComparison.OrdinalIgnoreCase);
        }

        static bool IsEquals(System.Reflection.MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType == typeof(object) && string.Equals("Equals", methodInfo.Name, System.StringComparison.OrdinalIgnoreCase);
        }

        static bool IsGetHashCode(System.Reflection.MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType == typeof(object) && string.Equals("GetHashCode", methodInfo.Name, System.StringComparison.OrdinalIgnoreCase);
        }

        static bool IsToString(System.Reflection.MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType == typeof(object) && string.Equals("ToString", methodInfo.Name, System.StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        /// <summary>
        ///   Gets the attributes.
        /// </summary>
        /// <param name = "member">The member.</param>
        /// <returns>The member attributes.</returns>
        static T[] GetAttributes<T>(System.Reflection.ICustomAttributeProvider member, bool inherit = true) where T : class
        {
            if (typeof(T) != typeof(object))
            {
                return (T[])member.GetCustomAttributes(typeof(T), inherit);
            }
            return (T[])member.GetCustomAttributes(inherit);
        }

        internal static System.Collections.Concurrent.ConcurrentDictionary<string, InterceptorMetaData> GetInterceptorMetaData(System.Reflection.MethodInfo[] methods)
        {
            var interceptorMetaData = new System.Collections.Concurrent.ConcurrentDictionary<string, InterceptorMetaData>();

            foreach (var item in methods)
            {
                var parameters = GetParameters(item);
                var agsTypes = parameters.Item1;

                var i = -1;
                Attributes.ArgumentsAttribute attr = null;
                Attributes.DeserializeAttribute deserialize = null;
                System.Type agsType = null;
                for (int i2 = 0; i2 < agsTypes.Length; i2++)
                {
                    var argumentsAttrs = GetAttributes<Attributes.ArgumentsAttribute>(agsTypes[i2]);
                    if (0 < argumentsAttrs.Length)
                    {
                        i = i2; agsType = agsTypes[i2]; attr = argumentsAttrs[0];

                        var deserializeAttrs = GetAttributes<Attributes.DeserializeAttribute>(agsTypes[i2]);
                        if (0 < deserializeAttrs.Length) { deserialize = deserializeAttrs[0]; }
                    }
                }

                //======LogAttribute======//
                Attributes.BusinessLogAttribute logAttr = LogAttr(item);
                //======CmdAttribute======//
                Attributes.CommandAttribute commandAttr = CmdAttr(item, item.Name);

                var metaData = new InterceptorMetaData(System.Array.FindIndex(agsTypes, p => typeof(Auth.ISession).IsAssignableFrom(p)), new System.Tuple<int, System.Type, Attributes.ArgumentsAttribute, Attributes.DeserializeAttribute>(i, agsType, attr, deserialize), new System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>>(), new System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.CheckedAttribute>>>(), new System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.ArgumentAttribute>>>(), logAttr, commandAttr, item.GetMethodFullName(), !typeof(void).Equals(item.ReturnType.FullName));

                if (-1 < i)
                {
                    var fields = agsTypes[i].GetFields();
                    foreach (var field in fields)
                    {
                        var atts = new System.Collections.Generic.List<Attributes.ArgumentAttribute>(GetAttributes<Attributes.ArgumentAttribute>(field));

                        var checkedAtts1 = new System.Collections.Generic.List<Attributes.CheckedAttribute>();
                        var atts1 = new System.Collections.Generic.List<Attributes.ArgumentAttribute>();
                        foreach (var att in atts)
                        {
                            if (att is Attributes.CheckedAttribute) { checkedAtts1.Add(att as Attributes.CheckedAttribute); }
                            else { atts1.Add(att); }
                        }

                        metaData.MemberAccessor.Add(field.Name, System.Tuple.Create(field.FieldType, cmstar.RapidReflection.Emit.FieldAccessorGenerator.CreateGetter(field), cmstar.RapidReflection.Emit.FieldAccessorGenerator.CreateSetter(field)));
                        metaData.ArgumentAtts.Add(field.Name, System.Tuple.Create(field.FieldType, atts1));
                        if (0 < checkedAtts1.Count) { metaData.CheckedAtts.Add(field.Name, System.Tuple.Create(field.FieldType, checkedAtts1)); }
                    }

                    var propertys = agsTypes[i].GetProperties();
                    foreach (var property in propertys)
                    {
                        var atts = new System.Collections.Generic.List<Attributes.ArgumentAttribute>(GetAttributes<Attributes.ArgumentAttribute>(property));

                        var checkedAtts1 = new System.Collections.Generic.List<Attributes.CheckedAttribute>();
                        var atts1 = new System.Collections.Generic.List<Attributes.ArgumentAttribute>();
                        foreach (var att in atts)
                        {
                            if (att is Attributes.CheckedAttribute) { checkedAtts1.Add(att as Attributes.CheckedAttribute); }
                            else { atts1.Add(att); }
                        }

                        metaData.MemberAccessor.Add(property.Name, System.Tuple.Create(property.PropertyType, cmstar.RapidReflection.Emit.PropertyAccessorGenerator.CreateGetter(property), cmstar.RapidReflection.Emit.PropertyAccessorGenerator.CreateSetter(property)));
                        metaData.ArgumentAtts.Add(property.Name, System.Tuple.Create(property.PropertyType, atts1));
                        if (0 < checkedAtts1.Count) { metaData.CheckedAtts.Add(property.Name, System.Tuple.Create(property.PropertyType, checkedAtts1)); }
                    }
                }

                if (!interceptorMetaData.TryAdd(item.Name, metaData)) { throw new System.Exception(string.Format("MetaData Name Exists {0}!", item.Name)); }
            }

            return interceptorMetaData;
        }

        internal static System.Collections.Concurrent.ConcurrentDictionary<string, InterceptorCommand> GetInterceptorCommand(System.Reflection.MethodInfo[] methods, object proxy)
        {
            var interceptorCommand = new System.Collections.Concurrent.ConcurrentDictionary<string, InterceptorCommand>();

            foreach (var item in methods)
            {
                Attributes.CommandAttribute commandAttr = CmdAttr(item, item.Name);

                var parameters = GetParameters(item);
                var agsObjs = parameters.Item2;

                var command = new InterceptorCommand(new System.Func<object, object, Result.IResult>((token, arguments) => cmstar.RapidReflection.Emit.MethodInvokerGenerator.CreateDelegate(proxy.GetType().GetMethod(item.Name), false)(proxy, GetAgsObj(agsObjs, token, new CommandAgs(arguments))) as Result.IResult), commandAttr.ResultDataType, !typeof(void).Equals(item.ReturnType.FullName));

                if (!interceptorCommand.TryAdd(commandAttr.OnlyName, command)) { throw new System.Exception(string.Format("Command Name Exists {0}!", commandAttr.OnlyName)); }
            }

            return interceptorCommand;
        }

        internal static System.Collections.Concurrent.ConcurrentDictionary<string, System.Func<object, object, Result.IResult>> GetInterceptorMember(System.Reflection.MethodInfo[] methods, object proxy)
        {
            var member = new System.Collections.Concurrent.ConcurrentDictionary<string, System.Func<object, object, Result.IResult>>();

            foreach (var item in methods)
            {
                var parameters = GetParameters(item);
                var agsObjs = parameters.Item2;

                if (!member.TryAdd(item.Name, new System.Func<object, object, Result.IResult>((token, arguments) => cmstar.RapidReflection.Emit.MethodInvokerGenerator.CreateDelegate(proxy.GetType().GetMethod(item.Name), false)(proxy, GetAgsObj(agsObjs, token, arguments)) as Result.IResult))) { throw new System.Exception(string.Format("Member Name Exists {0}!", item.Name)); }
            }

            return member;
        }
    }

    #region Meta

    internal struct CommandAgs
    {
        public CommandAgs(object ags) { this.ags = ags; }

        object ags;
        public object Ags { get { return ags; } }
    }

    public struct InterceptorMetaData
    {
        public InterceptorMetaData(int sessionPosition, System.Tuple<int, System.Type, Attributes.ArgumentsAttribute, Attributes.DeserializeAttribute> arguments, System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> memberAccessor, System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.CheckedAttribute>>> checkedAtts, System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.ArgumentAttribute>>> argumentAtts, Attributes.BusinessLogAttribute logAttr, Attributes.CommandAttribute commandAttr, string fullName, bool hasReturn)
        {
            this.sessionPosition = sessionPosition;
            this.arguments = arguments;
            this.memberAccessor = memberAccessor;
            this.checkedAtts = checkedAtts;
            this.argumentAtts = argumentAtts;
            this.businessLogAttr = logAttr;
            this.commandAttr = commandAttr;
            this.fullName = fullName;
            this.hasReturn = hasReturn;
        }

        //===============session==================//
        readonly int sessionPosition;
        public int SessionPosition { get { return sessionPosition; } }
        //===============arguments==================//
        readonly System.Tuple<int, System.Type, Attributes.ArgumentsAttribute, Attributes.DeserializeAttribute> arguments;
        public System.Tuple<int, System.Type, Attributes.ArgumentsAttribute, Attributes.DeserializeAttribute> Arguments { get { return arguments; } }
        //===============memberAccessor==================//
        readonly System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> memberAccessor;
        public System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> MemberAccessor { get { return memberAccessor; } }
        //===============checkedAtts==================//
        readonly System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.CheckedAttribute>>> checkedAtts;
        public System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.CheckedAttribute>>> CheckedAtts { get { return checkedAtts; } }
        //==============argumentAtts===================//
        readonly System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.ArgumentAttribute>>> argumentAtts;
        public System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.ArgumentAttribute>>> ArgumentAtts { get { return argumentAtts; } }
        //==============logAttribute===================//
        readonly Attributes.BusinessLogAttribute businessLogAttr;
        public Attributes.BusinessLogAttribute BusinessLogAttr { get { return businessLogAttr; } }
        //==============cmdAttribute===================//
        readonly Attributes.CommandAttribute commandAttr;
        public Attributes.CommandAttribute CommandAttr { get { return commandAttr; } }
        //==============fullName===================//
        readonly string fullName;
        public string FullName { get { return fullName; } }
        //==============hasReturn===================//
        readonly bool hasReturn;
        public bool HasReturn { get { return hasReturn; } }
    }

    public struct InterceptorCommand
    {
        public InterceptorCommand(System.Func<object, object, Result.IResult> member, Attributes.CommandAttribute.DataType resultDataType, bool hasReturn)
        {
            this.member = member;
            this.resultDataType = resultDataType;
            this.hasReturn = hasReturn;
        }

        //===============member==================//
        readonly System.Func<object, object, Result.IResult> member;
        public System.Func<object, object, Result.IResult> Member { get { return member; } }
        //===============resultDataType==================//
        readonly Attributes.CommandAttribute.DataType resultDataType;
        public Attributes.CommandAttribute.DataType ResultDataType { get { return resultDataType; } }
        //==============hasReturn===================//
        readonly bool hasReturn;
        public bool HasReturn { get { return hasReturn; } }
    }

    #endregion
}
