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

namespace Business.Auth
{
    using Result;

    public abstract class InterceptorBase : Castle.DynamicProxy.IInterceptor
    {
        public abstract void Intercept(Castle.DynamicProxy.IInvocation invocation);

        internal protected System.Collections.Concurrent.ConcurrentDictionary<string, Extensions.InterceptorMetaData> MetaData { get; set; }

        internal protected Business.IBusiness Business { get; set; }

        internal protected Attributes.BusinessLogAttribute BusinessLogAttr { get; set; }

        sealed class CheckAgsObj
        {
            public System.Collections.Generic.List<Attributes.CheckedAttribute> checkAtts;
            public System.Type type;
            public object value;
        }

        internal virtual Result CheckAgs<Result>(Extensions.InterceptorMetaData metaData, object arguments)
            where Result : class, IResult, new()
        {
            var checks = new System.Collections.Generic.Dictionary<string, CheckAgsObj>(metaData.CheckedAtts.Count);
            foreach (var item in metaData.CheckedAtts)
            {
                checks.Add(item.Key, new CheckAgsObj { checkAtts = item.Value.Item2, type = item.Value.Item1 });
            }

            //=========================================//
            foreach (var item in metaData.MemberAccessor)
            {
                var name = item.Key;
                var check = checks.ContainsKey(name);
                var trim = metaData.Arguments.Item3.TrimAllChar && typeof(System.String).Equals(item.Value.Item1);

                if (check || trim)
                {
                    var value = item.Value.Item2(arguments);

                    if (check)
                    {
                        checks[name].value = value;
                    }

                    if (trim && null != value)
                    {
                        item.Value.Item3(arguments, System.Convert.ToString(value).Trim());
                    }
                }
            }

            //check
            foreach (var argument in checks)
            {
                foreach (var item in argument.Value.checkAtts)
                {
                    var result = item.Checked<Result>(argument.Value.type, argument.Value.value, argument.Key);
                    if (null != result) { return result; }
                }
            }

            return null;
        }
    }
}
