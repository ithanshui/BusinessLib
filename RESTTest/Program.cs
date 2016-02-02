using Funq;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AppHost host = new AppHost();
            host.Init().Start("http://localhost:8099/");

            System.Console.Read();
        }
    }

    public class AppHost : AppSelfHostBase
    {
        public AppHost()
            : base("AppHost", typeof(AppService).Assembly) { }

        public override void Configure(Container container) { }
    }

    [Route("/AppHost", "GET")]
    public class Get : Business.Extensions.Command.MemberData, IReturn<Response> { }

    [Route("/AppHost", "POST")]
    public class Post : Business.Extensions.Command.MemberData, IReturn<Response> { }

    public class Response
    {
        public string Result { get; set; }
    }

    public class AppService : Service
    {
        public object Get(Get request)
        {
            return Business.Extensions.Command.MemberCall(TestLib.Common.Interceptor, TestLib.Common.InterceptorNot, this.Request.UserHostAddress, request).ToString();
        }

        public object Post(Post request)
        {
            return Business.Extensions.Command.MemberCall(TestLib.Common.Interceptor, TestLib.Common.InterceptorNot, this.Request.UserHostAddress, request).ToString();
        }
    }
}
