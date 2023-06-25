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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ApiClient.Core.Configuration;
using ApiClient.OAuth2.Models;

namespace ApiClient.Models
{
    public class ApiClientSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string ListenUri { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpirationDateTime { get; set; }

        private static ApiClientSettings _instance = CreateFromConfigFile();

        public static ApiClientSettings GetInstance()
        {
            return _instance;
        }

        public void Save()
        {
            var t = ApiClientConfigHelper.Instance();
            t.ClientId = ClientId;
            t.ClientSecret = ClientSecret;
            t.RedirectUri = RedirectUri;
            t.ListenUri = ListenUri;
            t.AccessToken = AccessToken;
            t.RefreshToken = RefreshToken;
            t.ExpirationDateTime = ExpirationDateTime;
            t.Save();
        }

        private static ApiClientSettings CreateFromConfigFile()
        {
            var t = ApiClientConfigHelper.Instance();
            return new ApiClientSettings()
            {
                ClientId = t.ClientId,
                ClientSecret = t.ClientSecret,
                RedirectUri = t.RedirectUri,
                ListenUri = t.ListenUri,
                AccessToken = t.AccessToken,
                RefreshToken = t.RefreshToken,
                ExpirationDateTime = t.ExpirationDateTime,
            };
        }

        public void UpdateAndSave(OAuth2AccessToken oAuth2AccessToken)
        {
            AccessToken = oAuth2AccessToken.AccessToken;
            RefreshToken = oAuth2AccessToken.RefreshToken;
            ExpirationDateTime = DateTime.Now.AddSeconds(oAuth2AccessToken.ExpiresIn);
            Save();
        }

        /// <summary>
        /// Clear token informations. Only AccessToken, RefreshToken, ExpirationDateTime are cleared
        /// </summary>
        public void ClearAndSave()
        {
            AccessToken = null;
            RefreshToken = null;
            ExpirationDateTime = DateTime.MinValue;
            Save();
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"   ------------ [ ApiClientSettings ] -------------");
            sb.AppendLine(@"     ClientId            : " + ClientId);
            sb.AppendLine(@"     ClientSecret        : " + ClientSecret);
            sb.AppendLine(@"     RedirectUri         : " + RedirectUri);
            sb.AppendLine(@"     ListenUri           : " + ListenUri);
            sb.AppendLine(@"     AccessToken         : " + AccessToken);
            sb.AppendLine(@"     RefreshToken        : " + RefreshToken);
            sb.AppendLine(@"     ExpirationDateTime  : " + ExpirationDateTime);
            sb.AppendLine(@"   ---------------------------------------------");

            return sb.ToString();
        }

        /// <summary>
        /// Set sandbox urls
        /// </summary>
        public static void SetSandboxMode()
        {
            Constants.DigiKeyUriConstants.SetSandboxMode();
        }

        /// <summary>
        /// Set productions urls. Already is by default, only required to call after SetSandboxMode
        /// </summary>
        public static void SetProductionMode()
        {
            Constants.DigiKeyUriConstants.SetProductionMode();
        }
    }
}
