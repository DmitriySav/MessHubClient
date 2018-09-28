using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace MessageHubClient.Interfaces
{
    public interface IHubConnectionManager
    {
        void Connect(string host);
        Task Start();
        void AddBearerToken(string token);
        IHubProxy GetHubProxy(string hubName);

    } 
    
}
