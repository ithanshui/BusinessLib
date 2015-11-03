﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Web;

namespace RestAuthentication
{
    /// <summary>
    /// This class is responsible for the interception of a http request and perform basic 
    /// authentication of that request.
    /// </summary>
    internal class BasicAuthenticationInterceptor : RequestInterceptor
    {
        private readonly BasicAuthenticationManager manager;

        public BasicAuthenticationInterceptor(BasicAuthenticationManager manager)
            : base(false)
        {
            this.manager = manager;
        }

        public override void ProcessRequest(ref RequestContext requestContext)
        {
            if (!manager.AuthenticateRequest(requestContext.RequestMessage))
            {
                requestContext.Reply(manager.CreateInvalidAuthenticationRequest());
                requestContext = null;
            }
        }
    }
}
