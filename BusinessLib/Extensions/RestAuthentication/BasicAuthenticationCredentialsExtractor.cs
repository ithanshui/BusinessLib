﻿// BasicAuthenticationUsingWCF a library to add Basic Authenticaion
// to a WCF REST based service
//
// Patrick Kalkman  pkalkie@gmail.com
//
// (C) Patrick Kalkman http://www.semanticarchitecture.net
//

namespace RestAuthentication
{
   /// <summary>
   /// This class is responsible for extracting and decoding the 
   /// credentials from the encoded authorization string.
   /// </summary>
   internal class BasicAuthenticationCredentialsExtractor
   {
      private readonly Base64Decoder decoder;
      private readonly DecodedCredentialsExtractor extractor;

      internal BasicAuthenticationCredentialsExtractor(Base64Decoder decoder, DecodedCredentialsExtractor extractor)
      {
         this.decoder = decoder;
         this.extractor = extractor;
      }

      internal virtual Credentials Extract(string basicAuthenticationCredentials)
      {
         string authenticationString = RemoveBasicFromAuthenticationString(basicAuthenticationCredentials);
         return extractor.Extract(decoder.Decode(authenticationString));
      }

      private static string RemoveBasicFromAuthenticationString(string basicAuthenticationCredentials)
      {
         return basicAuthenticationCredentials.Replace("Basic", string.Empty);
      }
   }
}