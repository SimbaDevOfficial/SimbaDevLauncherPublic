using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimbaDevPublicLauncher.utils
{
    internal class Auth
    {
        static string tokenresponse { get; set; }

        static string token { get; set; }

        static string Exchangeresponselol { get; set; }
        static readonly string IosClientId = "MzQ0NmNkNzI2OTRjNGE0NDg1ZDgxYjc3YWRiYjIxNDE6OTIwOWQ0YTVlMjVhNDU3ZmI5YjA3NDg5ZDMxM2I0MWE=";
        public static async Task<string> GetExchange(string authorization_code)
        {
            try
            {
                //get token
                var url = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.Headers["Authorization"] = "Basic " + IosClientId;
                httpRequest.ContentType = "application/x-www-form-urlencoded";

                var data = "grant_type=authorization_code&scope=friends%20basic_profile%20presence&code=" + authorization_code;

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    tokenresponse = streamReader.ReadToEnd();
                }
                JObject parsedtokenresponse = JObject.Parse(tokenresponse);

                // Extract the version and Discord URL from the launcher info
                token = (string)parsedtokenresponse["access_token"];
                if(String.IsNullOrEmpty(token))
                {
                    return "invalidauth";
                } else
                {
                    if(token.Length == 32)
                    {
                        var ExchangeCode = await GetExchangeCode(token);
                        if(ExchangeCode == "invalidauth")
                        {
                            return "invalidauth";
                        } else
                        {
                            return ExchangeCode;
                        }
                    } else
                    {
                        return "invalidauth";
                    }
                }

            }
            catch (Exception ex)
            {
                return "Error With Auth: " + ex;
            }
        }
        public static async Task<string> GetExchangeCode(string tokenlol)
        {
            Logging.Log("Getting Exchange Code");
            try
            {
                var url = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer " + tokenlol;

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    Exchangeresponselol = streamReader.ReadToEnd();
                }
                if(String.IsNullOrEmpty(Exchangeresponselol))
                {
                    return "invalidauth";
                } else
                {
                    JObject parsedexchangeresponse = JObject.Parse(Exchangeresponselol);

                    // Extract the version and Discord URL from the launcher info
                    var exchange = (string)parsedexchangeresponse["code"];
                    if (String.IsNullOrEmpty(exchange))
                    {
                        return "invalidauth";
                    }
                    else
                    {
                        return exchange;
                    }
                }
            } catch (Exception ex)
            {
                return "Error with Auth or sum: " + ex;
            }
        }
    }
}
