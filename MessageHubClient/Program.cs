using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using MessageHubClient.Services;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MessageHubClient
{
    class Program
    {
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var settings = new ClientSetting();

            settings = configuration.GetSection("clientSetting").Get<ClientSetting>();
                
            


            string token = GetToken(settings);

            var connection = new HubConnectionManager();
            connection.Connect(settings.host);
            if (!string.IsNullOrEmpty(token))
            {
                connection.AddBearerToken(token);
            }
            var messageHubProxy = connection.GetHubProxy("MessageHub");
            messageHubProxy.On<string>("broadcastMessage", Console.WriteLine);
            connection.Start().Wait();

            messageHubProxy.Invoke("Start");

            Console.ReadKey();
        }


        static string GetToken(ClientSetting setting)
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
