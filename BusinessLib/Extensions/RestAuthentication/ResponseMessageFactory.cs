﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System.Globalization;
using System.Net;
using System.ServiceModel.Channels;

namespace RestAuthentication
{
    /// <summary>
    /// This class is responsible for creating the http response message when a 
    /// client does not send the appropriate authentication header.
    /// </summary>
    internal class ResponseMessageFactory
    {
        private const string BasicAuthenticationHeaderName = "WWW-Authenticate";
        private readonly string realm;

        internal ResponseMessageFactory(string realm)
        {
            this.realm = realm;
        }

        internal virtual Message CreateInvalidAuthorizationMessage()
        {
            var responseMessage = Message.CreateMessage(MessageVersion.None, null);
            HttpResponseMessageProperty responseProperty = CreateResponseProperty();
            responseMessage.Properties.Add(HttpResponseMessageProperty.Name, responseProperty);
            return responseMessage;
        }

        private HttpResponseMessageProperty CreateResponseProperty()
        {
            var responseProperty = new HttpResponseMessageProperty();
            responseProperty.StatusCode = HttpStatusCode.Unauthorized;
            responseProperty.Headers.Add(BasicAuthenticationHeaderName, string.Format(CultureInfo.InvariantCulture, "Basic realm=\"{0}\"", realm));
            return responseProperty;
        }
    }
}