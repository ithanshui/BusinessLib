using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLib.Extensions
{
    using Ninject;
    using Ninject.Extensions.Interception.Infrastructure.Language;

    public class InterceptorBind<T> : System.IDisposable
    {
        readonly IKernel Kernel = new StandardKernel();
        public readonly T Instance;

        public InterceptorBind(Ninject.Extensions.Interception.IInterceptor interceptor, params Ninject.Parameters.IParameter[] parameters)
        {
            Kernel.Bind<T>().ToSelf().InTransientScope().Intercept().With(interceptor);
            Instance = Kernel.Get<T>(parameters);
        }

        public void Dispose()
        {
            Kernel.Dispose();

            if (null != Instance)
            {
                var disp = Instance.GetType().GetInterface("System.IDisposable");

                if (null != disp)
                {
                    ((IDisposable)Instance).Dispose();
                }
            }
        }
    }
}
