﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.ServiceModel;

namespace RestAuthentication
{
    /// <summary>
    /// This class is responsible for creating a service security context that gets attached
    /// to the incoming request if successfully validated.
    /// </summary>
    internal class ServiceSecurityContextFactory
    {
        private readonly AuthorizationPolicyFactory authorizationPolicyFactory;

        public ServiceSecurityContextFactory(AuthorizationPolicyFactory authorizationPolicyFactory)
        {
            this.authorizationPolicyFactory = authorizationPolicyFactory;
        }

        internal ServiceSecurityContext Create(Credentials credentials)
        {
            var authorizationPolicies = new List<IAuthorizationPolicy>();
            authorizationPolicies.Add(authorizationPolicyFactory.Create(credentials));
            return new ServiceSecurityContext(authorizationPolicies.AsReadOnly());
        }
    }
}