﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System.Web.Security;
using Microsoft.ServiceModel.Web;

namespace RestAuthentication
{
    /// <summary>
    /// This class is responsible for creating a basic authentication request interceptor.
    /// </summary>
    public class RequestInterceptorFactory
    {
        public static RequestInterceptor Create(string realm, CustomMembershipProvider membershipProvider)
        {
            var basicAuthenticationCredentialsExtractor = new BasicAuthenticationCredentialsExtractor(new Base64Decoder(), new DecodedCredentialsExtractor());
            var httpRequestAuthorizationExtractor = new AuthorizationStringExtractor();
            var responseMessageFactory = new ResponseMessageFactory(realm);
            var serviceSecurityContextFactory = new ServiceSecurityContextFactory(new AuthorizationPolicyFactory());
            var basicAuthenticationManager = new BasicAuthenticationManager(basicAuthenticationCredentialsExtractor, httpRequestAuthorizationExtractor, membershipProvider, responseMessageFactory, serviceSecurityContextFactory);
            return new BasicAuthenticationInterceptor(basicAuthenticationManager);
        }
    }
}
