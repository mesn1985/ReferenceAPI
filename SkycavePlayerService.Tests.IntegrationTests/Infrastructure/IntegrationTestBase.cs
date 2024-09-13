

using SkycavePlayerService.api.DTOs;
using SkycavePlayerService.Tests.TestUtilities;
using SkycavePlayerService.Tests.TestUtilities.Extensions;

namespace SkycavePlayerService.Tests.IntegrationTests.Infrastructure
{
    public abstract class IntegrationTestBase : IClassFixture<PlayerServiceWebApplicationFactory>
    {
        protected PlayerServiceWebApplicationFactory _factory;
        protected PlayerRecordGenerator _playerRecordGenerator;
        protected string ApiVersion = "v1";
        protected string UrlForPlayerUpdate;
        protected string UrlComputingListOfPlayersAt;
        protected string UrlGetPlayer;

        public IntegrationTestBase(PlayerServiceWebApplicationFactory factory)
        {
            _factory = factory;
            _playerRecordGenerator = new PlayerRecordGenerator();
            string baseUrl = $"http://localhost:5139/api/{ApiVersion}/players/";
            UrlForPlayerUpdate = baseUrl;
            UrlComputingListOfPlayersAt = baseUrl + "computerlistofplayersat";
            UrlGetPlayer = baseUrl;
        }

        protected async Task<PlayerRecord> CreateAndPutPlayerRecord(string playerPosition, HttpClient client)
        {
            PlayerRecord playerRecord = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(playerPosition);
            var requestContent = playerRecord.AsStringContent();
            await client.PutAsync(UrlForPlayerUpdate, requestContent);
            return playerRecord;
        }

    }
}
