using System.Net.Http.Json;
using PlayerRecordService.api.DTOs;
using PlayerRecordService.Tests.IntegrationTests.Infrastructure;
using PlayerRecordService.Tests.TestUtilities.Extensions;

namespace PlayerRecordService.Tests.IntegrationTests.InProcessTests
{
    public class ComputeListOfPlayersInProcess : IntegrationTestBase
    {
        public ComputeListOfPlayersInProcess(
            PlayerServiceWebApplicationFactory factory) 
            : base(factory)
        { }
        [Fact]
        public async Task GET_Can_Compute_List_Of_Players_At_Position_With_Single_Player_At_Position_Returns_200()
        {
            //Arrange
            var client = _factory.CreateClient();
            string positionOfPlayer = "(1,1,1)";
            PlayerRecord playerRecord = _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(positionOfPlayer);

            var content = playerRecord.AsStringContent();
            await client.PutAsync(UrlForPlayerUpdate, content);
           
            //Act
            var response = 
               await  client.GetAsync(
                    UrlComputingListOfPlayersAt + $"?position={Uri.EscapeDataString(positionOfPlayer)}"
                    );

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);


            var playersFromResponse = 
                await response.Content.ReadFromJsonAsync<IEnumerable<PlayerRecord>>();

            playersFromResponse.Count().Should().Be(1);
            playersFromResponse.First().PlayerId.Should().Be(playerRecord.PlayerId);
        }
        [Fact]
        public async Task GET_Can_Compute_List_Of_Players_At_Position_With_Three_Player_At_Position_Returns_200()
        {
            //Arrange
            var client = _factory.CreateClient();
            string positionOfPlayer = "(2,2,2)";


            PlayerRecord player1 = await CreateAndPutPlayerRecord(positionOfPlayer, client);
            PlayerRecord player2 = await CreateAndPutPlayerRecord(positionOfPlayer, client);
            PlayerRecord player3 = await CreateAndPutPlayerRecord(positionOfPlayer, client);
            

            //Act
            var response = await client.GetAsync(UrlComputingListOfPlayersAt + $"?position={Uri.EscapeDataString(positionOfPlayer)}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var playersFromResponse = await response.Content.ReadFromJsonAsync<IEnumerable<PlayerRecord>>();

            playersFromResponse.Count().Should().Be(3);

            playersFromResponse.Any(playersFromResponse => playersFromResponse.PlayerId.Equals(player1.PlayerId)).Should().BeTrue();
            playersFromResponse.Any(playersFromResponse => playersFromResponse.PlayerId.Equals(player2.PlayerId)).Should().BeTrue();
            playersFromResponse.Any(playersFromResponse => playersFromResponse.PlayerId.Equals(player3.PlayerId)).Should().BeTrue();

        }
        [Fact]
        public async Task GET_Return_No_Content_When_No_Players_Are_In_The_Room_Returns_204()
        {
            //Arrange
            var client = _factory.CreateClient();
            string positionOfPlayerInARoom = "(3,3,3)";
            string positionWithNoPlayers = "(4,4,4)";

            var playerRecords = await Task.WhenAll(
                CreateAndPutPlayerRecord(positionOfPlayerInARoom, client),
                CreateAndPutPlayerRecord(positionOfPlayerInARoom, client),
                CreateAndPutPlayerRecord(positionOfPlayerInARoom, client)
            );

            //Act
            var response = await client.GetAsync(UrlComputingListOfPlayersAt + $"?position={Uri.EscapeDataString(positionWithNoPlayers)}");
            string contentString = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            contentString.Length.Should().Be(0);
        }

        [Theory]
        [InlineData("1,2,3)")]
        [InlineData("(1,2,3")]
        [InlineData("1,2,3")]
        [InlineData("(e,2,3)")]
        [InlineData("(1,e,3)")]
        [InlineData("(1,2,e)")]
        public async Task GET_Return_Bad_Request_With_Malformed_Position_String_Returns_404(string position)
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response =
                await client.GetAsync(
                    UrlComputingListOfPlayersAt + $"?position={Uri.EscapeDataString(position)}"
                );

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
    }

