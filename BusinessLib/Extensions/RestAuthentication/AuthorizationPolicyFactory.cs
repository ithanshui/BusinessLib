﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System.IdentityModel.Policy;
using System.Security.Principal;

namespace RestAuthentication
{
    /// <summary>
    /// This class is repsonsible for creating a authorization policy based on the 
    /// given credentials.
    /// </summary>
    internal class AuthorizationPolicyFactory
    {
        public virtual IAuthorizationPolicy Create(Credentials credentials)
        {
            var genericIdentity = new GenericIdentity(credentials.UserName);
            var genericPrincipal = new GenericPrincipal(genericIdentity, new string[] { });
            return new PrincipalAuthorizationPolicy(genericPrincipal);
        }
    }
}