//-----------------------------------------------------------------------
//
// THE SOFTWARE IS PROVIDED "AS IS" WITHOUT ANY WARRANTIES OF ANY KIND, EXPRESS, IMPLIED, STATUTORY, 
// OR OTHERWISE. EXPECT TO THE EXTENT PROHIBITED BY APPLICABLE LAW, DIGI-KEY DISCLAIMS ALL WARRANTIES, 
// INCLUDING, WITHOUT LIMITATION, ANY IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, 
// SATISFACTORY QUALITY, TITLE, NON-INFRINGEMENT, QUIET ENJOYMENT, 
// AND WARRANTIES ARISING OUT OF ANY COURSE OF DEALING OR USAGE OF TRADE. 
// 
// DIGI-KEY DOES NOT WARRANT THAT THE SOFTWARE WILL FUNCTION AS DESCRIBED, 
// WILL BE UNINTERRUPTED OR ERROR-FREE, OR FREE OF HARMFUL COMPONENTS.
// 
//-----------------------------------------------------------------------

using System;

namespace ApiClient.Constants
{
    /// <summary>
    ///     Uri constants to talk to our OAuth2 server implementation.
    /// </summary>
    public static class DigiKeyUriConstants
    {
        /// <summary>
        /// Set sandbox urls
        /// </summary>
        public static void SetSandboxMode()
        {
            baseAddress = BaseAddress_SB;
            tokenEndpoint = TokenEndpoint_SB;
            authorizationEndpoint = AuthorizationEndpoint_SB;
        }

        /// <summary>
        /// Set productions urls. Already is by default, only required to call after SetSandboxMode
        /// </summary>
        public static void SetProductionMode()
        {
            baseAddress = BaseAddress_PROD;
            tokenEndpoint = TokenEndpoint_PROD;
            authorizationEndpoint = AuthorizationEndpoint_PROD;
        }

        // Public realtime uris
        public static Uri BaseAddress { get => baseAddress; }
        public static Uri TokenEndpoint { get => tokenEndpoint; }
        public static Uri AuthorizationEndpoint { get => authorizationEndpoint; }

        // Production Sandbox instance
        private static readonly Uri BaseAddress_SB = new Uri("https://sandbox-api.digikey.com");
        private static readonly Uri TokenEndpoint_SB = new Uri("https://sandbox-api.digikey.com/v1/oauth2/token");
        private static readonly Uri AuthorizationEndpoint_SB = new Uri("https://sandbox-api.digikey.com/v1/oauth2/authorize");

        // Production instance
        private static readonly Uri BaseAddress_PROD = new Uri("https://api.digikey.com");
        private static readonly Uri TokenEndpoint_PROD = new Uri("https://api.digikey.com/v1/oauth2/token");
        private static readonly Uri AuthorizationEndpoint_PROD = new Uri("https://api.digikey.com/v1/oauth2/authorize");

        // Private realtime uris
        private static Uri baseAddress = BaseAddress_PROD;
        private static Uri tokenEndpoint = TokenEndpoint_PROD;
        private static Uri authorizationEndpoint = AuthorizationEndpoint_PROD;
    }
}
