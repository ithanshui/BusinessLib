﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System.ServiceModel.Channels;

namespace RestAuthentication
{
    /// <summary>
    /// This class is responsible for extracting the  
    /// authentication string from the http message header.
    /// </summary>
    internal class AuthorizationStringExtractor
    {
        const string BasicAuthenticationHeaderName = "Authorization";

        internal virtual bool TryExtractAuthorizationHeader(Message message, out string authenticationString)
        {
            var requestMessageProperty = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
            authenticationString = requestMessageProperty.Headers[BasicAuthenticationHeaderName];
            return !string.IsNullOrEmpty(authenticationString);
        }
    }
}