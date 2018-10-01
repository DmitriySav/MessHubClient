using System;
using System.IO;
using MessageHubClient.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace MessageHubClient.Tests
{
    [TestFixture]
    class TokenTests
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
        public void GetToken_whenCalledWithCorrectCredentials_tokenIsNotEmpty()
        {
            string token = TokenService.GetToken(_settings);

            Assert.IsNotEmpty(token);
        }
        [Test]
        public void GetToken_whenCalledWithIncorrectCredentials_tokenIsEmpty()
        {
            var settings = new ClientSetting
            {
                host = _settings.host,
                userName = "rqrq",
                password = "23r3r32r"
            };

            string token = TokenService.GetToken(settings);

            Assert.IsEmpty(token);
        }

        [Test]
        public void GetToken_whenCalledWithIncorrectHosts_AggregateException()
        {
            var settings = new ClientSetting
            {
                host = "http://localhost:54839/",
                userName = "rqrq",
                password = "23r3r32r"
            };

            Assert.Throws<AggregateException>(() =>
                TokenService.GetToken(settings));
        }
    }
}
