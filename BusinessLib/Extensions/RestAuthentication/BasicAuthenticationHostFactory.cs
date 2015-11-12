using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using RestAuthentication;
using Microsoft.ServiceModel.Web;

namespace RestAuthentication
{
    /// <summary>
    /// This class is responsible for creating a servicehost that includes a basic 
    /// authentication request interceptor.
    /// </summary>
    public class BasicAuthenticationHostFactory : System.ServiceModel.Activation.ServiceHostFactory
    {
        string realm = "WebService";

        internal static readonly BasicAuthenticationHostFactory Factory = new BasicAuthenticationHostFactory();
        public static ServiceHost CreateServiceHost2(Type serviceType, string realm = "WebService", params Uri[] baseAddresses)
        {
            Factory.realm = realm;
            return Factory.CreateServiceHost(serviceType, baseAddresses);
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, params Uri[] baseAddresses)
        {
            var serviceHost = new WebServiceHost2(serviceType, true, baseAddresses);

            var provider = new CustomMembershipProvider();

            var fieldValidateUser = serviceType.GetField("ValidateUser");

            if (null != fieldValidateUser)
            {
                var value = fieldValidateUser.GetValue(null);
                if (null != value)
                {
                    provider.ValidateUserFunc = value as System.Func<string, string, string, bool>;
                }
            }

            serviceHost.Interceptors.Add(RequestInterceptorFactory.Create(realm, provider));

            return serviceHost;
        }
    }
}