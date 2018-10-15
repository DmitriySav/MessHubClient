using System;

using System.IO;
using MessageHubClient.Services;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Extensions.Configuration;


namespace MessageHubClient
{
    public class Program
    {
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();           

            var settings = configuration.GetSection("clientSetting").Get<ClientSetting>();

            string token = TokenService.GetToken(settings);

            var connection = new HubConnection(settings.host);
            if (!string.IsNullOrEmpty(token))
            {
                connection.Headers.Add("Authorization", $"Bearer {token}");
            }
            var messageHubProxy = connection.CreateHubProxy(settings.hubName);
            messageHubProxy.On<string>("broadcastMessage", Console.WriteLine);
            connection.Start().Wait();

            messageHubProxy.Invoke("Start");

            Console.ReadKey();
        }


        
    }
}
