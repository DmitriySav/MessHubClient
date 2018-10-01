using System;
using System.IO;
using System.Threading.Tasks;
using MessageHubClient.Services;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace MessageHubClient.Tests
{
    [TestFixture]
    public class HubConnectionTest
    {
        private ClientSetting _settings;

        [SetUp]
        public void Init()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            _settings = new ClientSetting();

            _settings = configuration.GetSection("clientSetting").Get<ClientSetting>();
        }

        [Test]
        public void Start_whenCalledWithInvalidHost_TrowException()
        {
            //arrange
            var hubConnection = new HubConnection("TestHost");
            //
            Assert.ThrowsAsync<UriFormatException>(async () =>
                await hubConnection.Start() );
        }

        [Test]
        public void Start_whenCalledWithCorrectHost_HubStateIsConnected()
        {
            //arrange
            
            var hubConnection = new HubConnection(_settings.host);

            //act
            hubConnection.Start().Wait();
            
            //assert
            var hubState = hubConnection.State;
            Assert.AreEqual( ConnectionState.Connected, hubState);
        }

        [Test]
        public void CreateHubProxy_whenCalledWithCorrectHubName()
        {
            var hubConnection = new HubConnection(_settings.host);
            var messageHubProxy = hubConnection.CreateHubProxy(_settings.hubName);

            hubConnection.Start().Wait();

            Assert.IsNotNull(messageHubProxy);
        }
        [Test]
        public void CreateHubProxy_whenCalledWithIncorrectHubName()
        {
            var hubConnection = new HubConnection(_settings.host);
            hubConnection.CreateHubProxy("BadHub");

            Assert.ThrowsAsync<HttpClientException>(async () =>
                await hubConnection.Start());
        }
        [Test]
        public async Task Program_InvokeServerMethodStartWithoutToken_returnFalse()
        {
            //arrange
            var hubConnection = new HubConnection(_settings.host);
            var messageHubProxy = hubConnection.CreateHubProxy(_settings.hubName);

            //act
            hubConnection.Start().Wait();
            var authorize = await messageHubProxy.Invoke<bool>("Start");

            //assert
            Assert.False(authorize);
        }
        [Test]
        public async Task Program_InvokeServerMethodStartWithToken_returnTrue()
        {
            //arrange
            var hubConnection = new HubConnection(_settings.host);
            var messageHubProxy = hubConnection.CreateHubProxy(_settings.hubName);

            //act
            var token = TokenService.GetToken(_settings);
            if (!string.IsNullOrEmpty(token))
            {
                hubConnection.Headers.Add("Authorization", $"Bearer {token}");
            }
            
            hubConnection.Start().Wait();
            var authorize = await messageHubProxy.Invoke<bool>("Start");

            //assert
            Assert.True(authorize);
        }
        [Test]
        public async Task Program_InvokeServerStartMethodWithIncorrectToken_returnFalse()
        {
            //arrange
            var hubConnection = new HubConnection(_settings.host);
            var messageHubProxy = hubConnection.CreateHubProxy(_settings.hubName);
            hubConnection.Headers.Add("Authorization", "Bearer fjo3rj2323p3jiru9023ru23ropjrop3jk2oprj23oprj2pj23orj");
            //act
            hubConnection.Start().Wait();
            var authorize = await messageHubProxy.Invoke<bool>("Start");
            //assert
            Assert.False(authorize);
        }
    }
}
