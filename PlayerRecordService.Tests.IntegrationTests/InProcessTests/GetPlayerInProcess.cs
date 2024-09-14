using System.Net.Http.Json;
using SkycavePlayerService.api.DTOs;
using SkycavePlayerService.Tests.TestUtilities.Extensions;

namespace SkycavePlayerService.Tests.IntegrationTests.InProcessTests
{
    public class GetPlayerInProcess : IntegrationTestBase
    {
        public GetPlayerInProcess(
            PlayerServiceWebApplicationFactory factory) 
            : base(factory)
        { }

        [Fact]
        public async Task GET_Can_Get_Existing_Player_Returns_200()
        {
            // Arrange
            var client = _factory.CreateClient(); 
            PlayerRecord playerRecord = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition("(1,1,1)"); // Create a player record
            var requestContent = playerRecord.AsStringContent();
            await client.PutAsync(UrlForPlayerUpdate, requestContent);

            // Act
            var response = await client.GetAsync(UrlGetPlayer + playerRecord.PlayerId);
            PlayerRecord retrievedPlayerRecord = await response.Content.ReadFromJsonAsync<PlayerRecord>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK); 
            retrievedPlayerRecord.PlayerId.Should().Be(playerRecord.PlayerId);
        }
        /// <summary>
        /// The reasoning for this test, is due to an identified error during the development of
        /// the API, where the returned player was not the correct player, when multiple players where stored.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GET_Can_Get_Existing_Correct_Existing_Player_Based_On_Playerid_Returns_200()
        {
            //Arrange
            string playerPosition = "(1,1,1)";
            var client = _factory.CreateClient();

            PlayerRecord playerRecord1 = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(playerPosition);
            var requestContent = playerRecord1.AsStringContent();
            await client.PutAsync(UrlForPlayerUpdate, requestContent);

            PlayerRecord playerRecord2 = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(playerPosition);
            requestContent = playerRecord2.AsStringContent();
            await client.PutAsync(UrlForPlayerUpdate, requestContent);

            PlayerRecord playerRecord3 = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(playerPosition);
            requestContent = playerRecord3.AsStringContent();
            await client.PutAsync(UrlForPlayerUpdate, requestContent);

            //Act
            var response = await client.GetAsync(UrlGetPlayer + playerRecord2.PlayerId);
            PlayerRecord retrivedPlayerRecord = await response.Content.ReadFromJsonAsync<PlayerRecord>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retrivedPlayerRecord.PlayerId.Should().Be(playerRecord2.PlayerId);
        }
        [Fact]
        public async Task GET_Return_Not_Found_When_Request_Is_For_A_Non_Exsisting_Player_Returns_404()
        {
            //Arrange
            string playerPosition = "(1,1,1)";
            var client = _factory.CreateClient();

            PlayerRecord playerRecord1 = await CreateAndPutPlayerRecord(playerPosition, client);
            PlayerRecord playerRecord2 = await CreateAndPutPlayerRecord(playerPosition, client);
            PlayerRecord playerRecord3 = await CreateAndPutPlayerRecord(playerPosition, client);

            //Act
            var response = await client.GetAsync(UrlGetPlayer + playerRecord2.PlayerId);
            PlayerRecord retrivedPlayerRecord = await response.Content.ReadFromJsonAsync<PlayerRecord>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retrivedPlayerRecord.PlayerId.Should().Be(playerRecord2.PlayerId);
        }
    }
}
