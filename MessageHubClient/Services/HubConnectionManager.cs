using System.Threading.Tasks;
using MessageHubClient.Interfaces;
using Microsoft.AspNet.SignalR.Client;


namespace MessageHubClient.Services
{
    class HubConnectionManager: IHubConnectionManager
    {
        private HubConnection hubConnection;
        public void Connect(string host)
        {
            hubConnection = new HubConnection(host);
        }

        public async Task Start()
        {
          await hubConnection.Start();
        }

        public void AddBearerToken(string token)
        {
            hubConnection.Headers.Add("Authorization", "Bearer " + token);
        }

        public IHubProxy GetHubProxy(string hubName)
        {
            var proxy = hubConnection.CreateHubProxy(hubName);
            return proxy;
        }
    }
}
