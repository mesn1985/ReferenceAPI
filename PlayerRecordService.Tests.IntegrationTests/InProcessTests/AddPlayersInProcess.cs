using PlayerRecordService.api.DTOs;
using PlayerRecordService.Shared.Models.PlayerRecord;
using PlayerRecordService.Tests.IntegrationTests.Infrastructure;
using PlayerRecordService.Tests.TestUtilities.Extensions;

namespace PlayerRecordService.Tests.IntegrationTests.InProcessTests
{
    public class AddPlayersInProcess : IntegrationTestBase
    {
        

        public AddPlayersInProcess(PlayerServiceWebApplicationFactory factory) : base(factory)
        { }

        [Fact]
        public async Task PUT_Can_Add_Player_When_Not_Already_Exsisting_Should_Return_Status_Code_201()
        {
            //Arrange
            var client 
                = _factory.CreateClient();
            PlayerRecord playerRecord 
                = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition();
            string playerId 
                = playerRecord.PlayerId;

            //Act
            var response 
                = await client.PutAsync(UrlForPlayerUpdate, playerRecord.AsStringContent());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().Be(UrlGetPlayer + playerId);

        }
        [Fact]
        public async Task PUT_Should_Not_Add_Player_With_Invalid_Position_Should_Return_Status_Code_400()
        {
            //Arrange
            var client = _factory.CreateClient();
            PlayerRecord playerRecord = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition();
            playerRecord.Position = "(1,2,4";

            //Act
            var response = await client.PutAsync(UrlForPlayerUpdate, playerRecord.AsStringContent());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }
        [Fact]
        public async Task PUT_Can_Add_Player_When_with_null_AccessToken_Should_Return_Status_Code_201()
        {
            //Arrange
            var client = _factory.CreateClient();
            PlayerRecord playerRecord = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition();
            string playerId = playerRecord.PlayerId;
            playerRecord.AccessToken = null;

            //Act
            var response = await client.PutAsync(UrlForPlayerUpdate, playerRecord.AsStringContent());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().Be(UrlGetPlayer + playerId);

        }

        [Theory]
        [InlineData(null,"playerName","groupName",Region.AALBORG,"(1,2,3)","accesToken")]
        [InlineData("playerId", null, "groupName", Region.AALBORG, "(1,2,3)", "accesToken")]
        [InlineData("playerId", "playerName", null, Region.AALBORG, "(1,2,3)", "accesToken")]
        [InlineData("playerId", "playerName", "groupName", null, "(1,2,3)", "accesToken")]
        [InlineData("playerId", "playerName", "groupName", Region.AALBORG, null, "accesToken")]
        public async Task PUT_Should_Not_Add_Player_With_Missing_Required_Attributes_Should_Return_Status_Code_400(
            string playerId,
            string playerName,
            string groupName,
            Region region,
            string  position,
            string accessToken
            )
        {
            //Arrange
            var client = _factory.CreateClient();
            PlayerRecord playerRecord = new PlayerRecord
            {
                PlayerId = playerId,
                PlayerName = playerName,
                GroupName = groupName,
                Region = region,
                Position = "(1,2,4",
                AccessToken = accessToken
            };
            var content = playerRecord.AsStringContent();

            //Act
            var response = await client.PutAsync(UrlForPlayerUpdate, content);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Theory]
        [InlineData("", "playerName", "groupName", Region.AALBORG, "(1,2,3)", "accesToken")]
        [InlineData("playerId", "", "groupName", Region.AALBORG, "(1,2,3)", "accesToken")]
        [InlineData("playerId", "playerName", "", Region.AALBORG, "(1,2,3)", "accesToken")]
        [InlineData("playerId", "playerName", "groupName", Region.AALBORG, "", "accesToken")]
        public async Task PUT_Should_Not_Add_Player_With_Empty_String_Attributes_Should_Return_Status_Code_400(
            string playerId,
            string playerName,
            string groupName,
            Region region,
            string position,
            string accessToken
        )
        {
            //Arrange
            var client = _factory.CreateClient();
            PlayerRecord playerRecord = new PlayerRecord
            {
                PlayerId = playerId,
                PlayerName = playerName,
                GroupName = groupName,
                Region = region,
                Position = position,
                AccessToken = accessToken
            };
            playerRecord.Position = "(1,2,4";
            var content = playerRecord.AsStringContent();

            //Act
            var reponse = await client.PutAsync(UrlForPlayerUpdate, content);

            //Assert
            reponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task PUT_Can_Update_Existing_Player_Should_Return_Status_Code_200()
        {
            //Arrange
            var client = _factory.CreateClient();
            PlayerRecord playerRecord = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition();
            await client.PutAsync(UrlForPlayerUpdate, playerRecord.AsStringContent());

            // Update player name
            playerRecord.PlayerName = "new PlayerName";
            var content = playerRecord.AsStringContent();

            //Act
            var response = await client.PutAsync(UrlForPlayerUpdate, content);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }




    }
}