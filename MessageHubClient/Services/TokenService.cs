using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace MessageHubClient.Services
{

    /// <summary>
    /// Service for work with tokens
    /// </summary>
    public class TokenService
    {
        /// <summary>
        /// Method return bearer token
        /// </summary>
        /// <param name="setting">object which contains credentials and host address</param>
        /// <returns></returns>
        public static string GetToken(ClientSetting setting)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "grant_type", "password" ),
                new KeyValuePair<string, string>( "username", setting.userName ),
                new KeyValuePair<string, string> ( "Password", setting.password )
            };
            var content = new FormUrlEncodedContent(pairs);

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(setting.host + "token", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    Dictionary<string, string> tokenDictionary =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                    Console.WriteLine("Token {0}", tokenDictionary["access_token"]);

                    return tokenDictionary["access_token"];
                }

                return String.Empty;

            }
        }
    }
}
