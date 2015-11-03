﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Web.Security;

namespace RestAuthentication
{
    /// <summary>
    /// This class is responsible for managing basic authentication when using WCF REST.
    /// </summary>
    internal class BasicAuthenticationManager
    {
        private readonly BasicAuthenticationCredentialsExtractor basicAuthenticationCredentialsExtractor;
        private readonly AuthorizationStringExtractor httpRequestAuthorizationExtractor;
        private readonly CustomMembershipProvider membershipProvider;
        private readonly ResponseMessageFactory responseMessageFactory;
        private readonly ServiceSecurityContextFactory serviceSecurityContextFactory;

        internal BasicAuthenticationManager(BasicAuthenticationCredentialsExtractor basicAuthenticationCredentialsExtractor,
           AuthorizationStringExtractor httpRequestAuthorizationExtractor,
           CustomMembershipProvider membershipProvider,
           ResponseMessageFactory responseMessageFactory,
           ServiceSecurityContextFactory serviceSecurityContextFactory)
        {
            this.basicAuthenticationCredentialsExtractor = basicAuthenticationCredentialsExtractor;
            this.httpRequestAuthorizationExtractor = httpRequestAuthorizationExtractor;
            this.membershipProvider = membershipProvider;
            this.responseMessageFactory = responseMessageFactory;
            this.serviceSecurityContextFactory = serviceSecurityContextFactory;
        }

        internal bool AuthenticateRequest(Message requestMessage)
        {
            string authenticationString;
            if (httpRequestAuthorizationExtractor.TryExtractAuthorizationHeader(requestMessage, out authenticationString))
            {
                Credentials credentials = basicAuthenticationCredentialsExtractor.Extract(authenticationString);

                if (null != requestMessage.Properties.Via.Segments && 0 < requestMessage.Properties.Via.Segments.Length && membershipProvider.ValidateUser(requestMessage.Properties.Via.Segments[requestMessage.Properties.Via.Segments.Length - 1], credentials.UserName, credentials.Password))
                {
                    AddSecurityContextToMessage(requestMessage, credentials);
                    return true;
                }
            }
            return false;
        }

        private void AddSecurityContextToMessage(Message requestMessage, Credentials credentials)
        {
            if (requestMessage.Properties.Security == null)
            {
                requestMessage.Properties.Security = new SecurityMessageProperty();
            }
            requestMessage.Properties.Security.ServiceSecurityContext = serviceSecurityContextFactory.Create(credentials);
        }

        internal Message CreateInvalidAuthenticationRequest()
        {
            return responseMessageFactory.CreateInvalidAuthorizationMessage();
        }
    }
}