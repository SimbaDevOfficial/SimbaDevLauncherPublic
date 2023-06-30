using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.Auth
{
    public class AccountInfo
    {
        public static string DisplayName { get; set; }
        public static string AccountId { get; set; }
    }

    public class Auth
    {
        private static RestClient restClient;

        static Auth()
        {
            restClient = new RestClient("https://account-public-service-prod03.ol.epicgames.com/account/api/oauth");
        }

        public static string GetDevicecodetoken()
        {
            RestRequest restRequest = CreateRestRequest(Method.Post, "token");
            restRequest.AddHeader("Authorization", "Basic OThmN2U0MmMyZTNhNGY4NmE3NGViNDNmYmI0MWVkMzk6MGEyNDQ5YTItMDAxYS00NTFlLWFmZWMtM2U4MTI5MDFjNGQ3");
            restRequest.AddParameter("grant_type", "client_credentials");

            try
            {
                var response = restClient.Execute(restRequest);
                Logging.Logging.LogToFile("DeviceCodeToken: " + response.Content);
                return ParseToken(response.Content);
            }
            catch
            {
                Logging.Logging.Error("Oops! It looks like there's a problem with your internet connection. Please check it and try again.");
                Logging.Logging.LogToFile("Oops! It looks like there's a problem with your internet connection. Please check it and try again.");
                Process.GetCurrentProcess().Kill();
                return "error";
            }
        }

        public static string GetDeviceCode(string authToken)
        {
            RestRequest restRequest = CreateRestRequest(Method.Post, "deviceAuthorization");
            restRequest.AddHeader("Authorization", "bearer " + authToken);
            restRequest.AddParameter("Content-Type", "application/x-www-form-urlencoded");

            var response = restClient.Execute(restRequest);
            var json = JObject.Parse(response.Content);
            string deviceCode = json.Value<string>("device_code");
            string verificationUriComplete = json.Value<string>("verification_uri_complete");

            Process.Start(verificationUriComplete);
            Logging.Logging.LogToFile("Started verification process. Please check your browser.");

            string accessToken = WaitForAccessToken(deviceCode);
            Logging.Logging.LogToFile("AccessToken: " + accessToken);
            return accessToken;
        }

        public static string GetExchange(string token)
        {
            RestRequest restRequest = CreateRestRequest(Method.Get, "exchange");
            restRequest.AddHeader("Authorization", "bearer " + token);

            var response = restClient.Execute(restRequest);
            Logging.Logging.LogToFile("Exchange: " + response.Content);
            return ParseExchange(response.Content);
        }

        private static RestRequest CreateRestRequest(Method method, string resource)
        {
            RestRequest restRequest = new RestRequest(resource);
            restRequest.Method = method;
            Logging.Logging.LogToFile("RestRequest: " + restRequest);
            return restRequest;
        }

        private static string ParseToken(string content)
        {
            var token = JObject.Parse(content);
            Logging.Logging.LogToFile("Token: " + token);
            return token.Value<string>("access_token");
        }

        private static string ParseExchange(string content)
        {
            var exchange = JObject.Parse(content);
            Logging.Logging.LogToFile("Exchange: " + exchange);
            return exchange.Value<string>("code");
        }

        private static string WaitForAccessToken(string deviceCode)
        {
            while (true)
            {
                RestRequest restRequest = CreateRestRequest(Method.Post, "token");
                restRequest.AddHeader("Authorization", "basic OThmN2U0MmMyZTNhNGY4NmE3NGViNDNmYmI0MWVkMzk6MGEyNDQ5YTItMDAxYS00NTFlLWFmZWMtM2U4MTI5MDFjNGQ3");
                restRequest.AddParameter("grant_type", "device_code");
                restRequest.AddParameter("device_code", deviceCode);

                var response = restClient.Execute(restRequest);
                var json = JObject.Parse(response.Content);

                if (json.ContainsKey("error"))
                {
                    Thread.Sleep(200);
                    Logging.Logging.LogToFile("Waiting for user to authorize the app.");
                }
                else
                {
                    AccountInfo.AccountId = json.Value<string>("account_id");
                    AccountInfo.DisplayName = json.Value<string>("displayName");
                    Logging.Logging.LogToFile("AccountInfo: " + AccountInfo.AccountId + ":" + AccountInfo.DisplayName);
                    return json.Value<string>("access_token");
                }
            }
        }
    }
}
