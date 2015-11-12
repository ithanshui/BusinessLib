﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;

namespace RestAuthentication
{
    /// <summary>
    /// This class implements an AuthorizationPolicy, this policy is assigned to the
    /// incoming request when it is successfully authenticated.
    /// </summary>
    internal class PrincipalAuthorizationPolicy : IAuthorizationPolicy
    {
        private readonly IPrincipal principal;

        private readonly string policyId = Guid.NewGuid().ToString();

        public PrincipalAuthorizationPolicy(IPrincipal principal)
        {
            this.principal = principal;
        }

        public string Id
        {
            get { return policyId; }
        }

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            evaluationContext.AddClaimSet(this, new DefaultClaimSet(Claim.CreateNameClaim(principal.Identity.Name)));
            evaluationContext.Properties["Identities"] = new List<IIdentity>(new[] { principal.Identity });
            evaluationContext.Properties["Principal"] = principal;
            return true;
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }
    }
}