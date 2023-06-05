using MattsTwitchBot.Core;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TwitchLib.Api;

namespace MattsTwitchBot.Tests.IntegrationTests.TwitchApiWrapperTests
{
    [TestFixture]
    public class TwitchApiWrapperTests
    {
        private TwitchApiWrapper _wrapper;
        private IConfigurationRoot _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Set the base path to the directory where the executable resides
                .AddJsonFile("appsettings.Development.json") // Specify the name of your appsettings.json file
                .Build();

            var api = new TwitchAPI();
            var apiClientId = _configuration.GetValue<string>("Twitch:ApiClientId");
            var apiClientSecret = _configuration.GetValue<string>("Twitch:ApiClientSecret");
            api.Settings.ClientId = apiClientId;
            api.Settings.Secret = apiClientSecret;
            
            _wrapper = new TwitchApiWrapper(api);
        }

        [TestCase("matthewdgroves", true)]
        [TestCase("w2CQSHwfX82461688689", false)] // I suppose it's possible this username could eventually exist
        public async Task DoesUserExist(string username, bool shouldExist)
        {
            // arrange

            // act
            var result = await _wrapper.DoesUserExist(username);

            // assert
            Assert.That(result, Is.EqualTo(shouldExist));
        }
    }
}