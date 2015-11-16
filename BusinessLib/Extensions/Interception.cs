using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLib.Extensions
{
    using Ninject;
    using Ninject.Extensions.Interception.Infrastructure.Language;

    public interface IInterceptorMetaData
    {
        System.Collections.Generic.Dictionary<string, InterceptorMetaData> MetaData
        { get; set; }
    }

    public class InterceptorBind<T> : System.IDisposable
    {
        readonly IKernel Kernel = new StandardKernel();
        public readonly T Instance;

        public InterceptorBind(Ninject.Extensions.Interception.IInterceptor interceptor, params Ninject.Parameters.IParameter[] parameters)
        {
            Kernel.Bind<T>().ToSelf().InTransientScope().Intercept().With(interceptor);
            Instance = Kernel.Get<T>(parameters);

            var metaData = interceptor as IInterceptorMetaData;
            if (null != metaData)
            {
                metaData.MetaData = GetInterceptorMetaData(typeof(T));
            }
        }

        public void Dispose()
        {
            Kernel.Dispose();

            if (null != Instance)
            {
                var disp = Instance as System.IDisposable;

                if (null != disp)
                {
                    disp.Dispose();
                }
            }
        }

        static System.Collections.Generic.Dictionary<string, InterceptorMetaData> GetInterceptorMetaData(System.Type type)
        {
            var interceptorMetaData = new System.Collections.Generic.Dictionary<string, InterceptorMetaData>();

            //atts
            //var methods = type.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).Where(c => c.IsVirtual && !c.IsFinal && c.IsSecurityCritical && !c.IsSecuritySafeCritical && !c.IsSecurityTransparent);
            var methodsAll = type.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var methods = System.Array.FindAll(methodsAll, c => c.IsVirtual && !c.IsFinal && c.IsSecurityCritical && !c.IsSecuritySafeCritical && !c.IsSecurityTransparent);

            foreach (var item in methods)
            {
                //var agsTypes = item.GetParameters().Select(c => c.ParameterType).ToArray();
                var ags = item.GetParameters();
                var agsTypes = new System.Type[ags.Length];
                for (int i2 = 0; i2 < ags.Length; i2++) { agsTypes[i2] = ags[i2].ParameterType; }

                var i = -1;
                Attributes.ArgumentsAttribute attrs = null;
                System.Type agsType = null;
                for (int i1 = 0; i1 < agsTypes.Length; i1++)
                {
                    var argumentsAttrs = agsTypes[i1].GetCustomAttributes(typeof(Attributes.ArgumentsAttribute), true);
                    if (0 < argumentsAttrs.Length) { i = i1; agsType = agsTypes[i1]; attrs = argumentsAttrs[0] as Attributes.ArgumentsAttribute; break; }
                }

                var metaData = new InterceptorMetaData(System.Array.FindIndex(agsTypes, p => p.Equals(typeof(BusinessLib.BasicAuthentication.ISession))), new System.Tuple<int, System.Type, Attributes.ArgumentsAttribute>(i, agsType, attrs), new System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>>(), new System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.CheckedAttribute>>>(), new System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.ArgumentAttribute>>>(), item.GetMethodFullName());

                if (-1 < i)
                {
                    var fields = agsTypes[i].GetFields();
                    foreach (var field in fields)
                    {
                        var atts = new List<Attributes.ArgumentAttribute>(field.GetCustomAttributes(typeof(Attributes.ArgumentAttribute), true) as Attributes.ArgumentAttribute[]);
                        var checkedAtts1 = new List<Attributes.CheckedAttribute>();
                        var atts1 = new List<Attributes.ArgumentAttribute>();
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
                        var atts = new List<Attributes.ArgumentAttribute>(property.GetCustomAttributes(typeof(Attributes.ArgumentAttribute), true) as Attributes.ArgumentAttribute[]);
                        var checkedAtts1 = new List<Attributes.CheckedAttribute>();
                        var atts1 = new List<Attributes.ArgumentAttribute>();
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

                interceptorMetaData.Add(item.Name, metaData);
            }

            return interceptorMetaData;
        }
    }

    public struct InterceptorMetaData
    {
        public InterceptorMetaData(int sessionPosition, System.Tuple<int, System.Type, Attributes.ArgumentsAttribute> arguments, System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> memberAccessor, System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.CheckedAttribute>>> checkedAtts, System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.ArgumentAttribute>>> argumentAtts, string fullName)
        {
            this.sessionPosition = sessionPosition;
            this.arguments = arguments;
            this.memberAccessor = memberAccessor;
            this.checkedAtts = checkedAtts;
            this.argumentAtts = argumentAtts;
            this.fullName = fullName;
        }

        readonly int sessionPosition;
        public int SessionPosition { get { return sessionPosition; } }
        readonly System.Tuple<int, System.Type, Attributes.ArgumentsAttribute> arguments;
        public System.Tuple<int, System.Type, Attributes.ArgumentsAttribute> Arguments { get { return arguments; } }
        readonly System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> memberAccessor;
        public System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> MemberAccessor { get { return memberAccessor; } }
        readonly System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.CheckedAttribute>>> checkedAtts;
        public System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.CheckedAttribute>>> CheckedAtts { get { return checkedAtts; } }
        readonly System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.ArgumentAttribute>>> argumentAtts;
        public System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Collections.Generic.List<Attributes.ArgumentAttribute>>> ArgumentAtts { get { return argumentAtts; } }
        readonly string fullName;
        public string FullName { get { return fullName; } }
    }
}
