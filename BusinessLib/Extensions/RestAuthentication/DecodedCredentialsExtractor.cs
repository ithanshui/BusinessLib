﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//
using System;

namespace RestAuthentication
{
    /// <summary>
    /// This class is responsible for extracting the credentials 
    /// from a decoded basic authentication header string.
    /// </summary>
    internal class DecodedCredentialsExtractor
    {
        internal virtual Credentials Extract(string credentials)
        {
            if (!string.IsNullOrEmpty(credentials))
            {
                string[] credentialTokens = credentials.Split(':');
                if (credentialTokens.Length == 2)
                {
                    return new Credentials(credentialTokens[0], credentialTokens[1]);
                }
            }

            throw new ArgumentException("The supplied credential string is invalid, it should comply to [username:password]", "credentials");
        }
    }
}