using ApiClient.Models;
using ESNLib.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ApiClient
{
    public class ApiClientWrapper
    {

        private void Write(string text, Logger.LogLevels logLevel = Logger.LogLevels.Debug)
        {
            Logger.Instance.Write($"[ApiClientWrapper] {text}", logLevel);
        }

        /// <summary>
        /// Get Access token (and Refresh Token)
        /// </summary>
        public async Task<bool> GetAccessToken()
        {
            // read clientSettings values from apiclient.config
            var _clientSettings = ApiClientSettings.GetInstance();
            Write(_clientSettings.ToString());

            // start up a HttpListener for the callback(RedirectUri) from the OAuth2 server
            HttpListenerContext context;
            ApiClient.OAuth2.OAuth2Service oAuth2Service;
            using (var httpListener = new HttpListener())
            {
                httpListener.Prefixes.Add(_clientSettings.ListenUri);
                Write($"listening to {_clientSettings.ListenUri}");
                try
                {
                    httpListener.Start();
                }
                catch (HttpListenerException ex)
                {
                    Write("Unable to listen on this address...", Logger.LogLevels.Error);
                    Write(ex.Message, Logger.LogLevels.Error);
                    return false;
                }

                // Initialize our OAuth2 service
                oAuth2Service = new ApiClient.OAuth2.OAuth2Service(_clientSettings);

                // create Authorize url and send call it thru Process.Start
                var authUrl = oAuth2Service.GenerateAuthUrl();
                Write("Auth URL : " + authUrl);
                Process.Start(authUrl);

                // get the URL returned from the callback(RedirectUri)
                context = await httpListener.GetContextAsync();
                var response = context.Response;

                // Send a response to the user
                string responseString = "<html><body><h1>Authorization completed !</h1><h2>You may close this window</h2></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/html";
                // wait for response sent
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

                // Unregister http listener
                response.Close();
                httpListener.Stop();
            }

            // extract the query parameters from the returned URL
            var queryString = context.Request.Url.Query;
            var queryColl = HttpUtility.ParseQueryString(queryString);

            string errorCode = queryColl["error"];
            if (errorCode != null)
            {
                string error = errorCode + "\n" + queryColl["error_description"];
                Write("Error : " + error, Logger.LogLevels.Error);

                return false;
            }

            // Grab the needed query parameter code from the query collection
            var code = queryColl["code"];
            Write($"Authorization code is {code}");

            // Pass the returned code value to finish the OAuth2 authorization ; get access token
            var result = await oAuth2Service.FinishAuthorization(code);

            // Check if you got an error during finishing the OAuth2 authorization
            if (result.IsError)
            {
                Write(string.Format("\n\nError            : {0}", result.Error), Logger.LogLevels.Error);
                Write(string.Format("\n\nError.Description: {0}", result.ErrorDescription), Logger.LogLevels.Error);

                return false;
            }
            else
            {
                _clientSettings.UpdateAndSave(result);

                return true;
            }
        }

        /// <summary>
        /// Refresh the acces token without user interaction. Require a valid refresh token
        /// </summary>
        public async Task<bool> RefreshAccessToken()
        {
            Write("Refreshing...");
            // read clientSettings values from apiclient.config
            var _clientSettings = ApiClientSettings.GetInstance();
            Write(_clientSettings.ToString());

            // Initialize our OAuth2 service
            var oAuth2Service = new ApiClient.OAuth2.OAuth2Service(_clientSettings);
            var result = await oAuth2Service.RefreshTokenAsync();

            // Check if you got an error during finishing the OAuth2 authorization
            if (result.AccessToken == null)
            {
                Write("Failed", Logger.LogLevels.Error);
                return false;
            }
            else
            {
                Write("Success");
                _clientSettings.UpdateAndSave(result);
                return true;
            }
        }

        /// <summary>
        /// Create required certificate to allow listening on localhost https
        /// </summary>
        public bool RegisterListener()
        {
            var _clientSettings = ApiClientSettings.GetInstance();

            Write("Checking admin privileges");

            bool hasPrivileges = MiscTools.HasAdminPrivileges();
            if (!hasPrivileges)
            {
                Write("Admin privileges missing. Aborting...", Logger.LogLevels.Error);
                return false;
            }

            Write("Registration...");

            using (Process proc = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C netsh http add urlacl url=\"{_clientSettings.ListenUri}\" user=everyone",
                    Verb = "runas",
                    UseShellExecute = false
                };
                Write(string.Format("Running : \"{0} {1}\"", startInfo.FileName, startInfo.Arguments));
                proc.StartInfo = startInfo;
                startInfo.RedirectStandardOutput = true;
                proc.Start();
                proc.WaitForExit();

                string output = proc.StandardOutput.ReadToEnd();
                Write("Output : " + output);

                bool res;
                if (output.Contains("URL reservation successfully added")) // The text that should be contained if sucessfull
                {
                    Write("Listener registered successfully !");
                    res = true;
                }
                else
                {
                    Write("Unable to register the listener !", Logger.LogLevels.Error);
                    res = false;
                }

                return res;
            }
        }

        /// <summary>
        /// Delete required certificate to allow listening on localhost https
        /// </summary>
        public bool UnregisterListener()
        {
            var _clientSettings = ApiClientSettings.GetInstance();

            bool hasPrivileges = MiscTools.HasAdminPrivileges();
            if (!hasPrivileges)
            {
                Write("Admin privileges must be granted to run this. Please restart as admin");
                return false;
            }

            Write("Unregistration...");

            using (Process proc = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C netsh http delete urlacl url=\"{_clientSettings.ListenUri}\"",
                    Verb = "runas",
                    UseShellExecute = false
                };
                Write(string.Format("Running : \"{0} {1}\"", startInfo.FileName, startInfo.Arguments));
                proc.StartInfo = startInfo;
                startInfo.RedirectStandardOutput = true;
                proc.Start();
                proc.WaitForExit();

                string output = proc.StandardOutput.ReadToEnd();
                Write("Output : " + output);

                bool res;
                if (output.Contains("URL reservation successfully deleted")) // The text that should be contained if sucessfull
                {
                    Write("Listener unregistered successfully !");
                    res = true;
                }
                else
                {
                    Write("Unable to unregister the listener !", Logger.LogLevels.Error);
                    res = false;
                }

                return res;
            }
        }
    }
}
