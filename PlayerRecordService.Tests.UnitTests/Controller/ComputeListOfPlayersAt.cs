using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlayerRecordService.api.DTOs;
using PlayerRecordService.Shared.Models.PlayerRecord;
using PlayerRecordService.Shared.Models.PlayerRecord.Primitives;
using PlayerRecordService.Tests.TestUtilities.Extensions;

namespace PlayerRecordService.Tests.UnitTests.Controller
{
    /// <summary>
    /// All input validation is handled by middleware, and is therefor
    /// only tested in the integration tests.
    /// All Unit test uses stubbed dependencies.
    /// </summary>
    public class ComputeListOfPlayersAt : BasePlayerControllerTest
    {
        [Fact]
        public  void
            ComputeListOfPLayersAt_Can_Compute_List_Of_Players_At_Position_With_Single_Player_At_Position_Returns_OK_Object()
        {
            //Arrange
            string playersPosition = "(1,1,1)";
            PlayerRecordModel playerRecordModel =
                PlayerRecordGenerator
                    .CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(playersPosition)
                    .AsModel();
            IEnumerable<PlayerRecordModel> collectionWithSinglePlayerModel
                = Enumerable.Empty<PlayerRecordModel>().Concat(new[] { playerRecordModel });

            MockPlayerRepository.Setup(repo => repo.GetPlayerRecordBy(It.IsAny<PlayerId>()))
                .ReturnsAsync(playerRecordModel);

            //Act
            IActionResult result = Controller.GetPlayerRecordById(playerRecordModel.PlayerId.Value).Result;
            OkObjectResult responseObject = result as OkObjectResult;
            PlayerRecord playerRecord = responseObject.Value as PlayerRecord;

            //Assert
            playerRecord.PlayerId.Should().Be(playerRecordModel.PlayerId.Value);

        }
        [Fact]
        public void ComputeListOfPLayersAt_Can_Compute_List_Of_Players_At_Position_With_Three_Player_At_Position_Returns_OK_Object()
        {
            string position = "(2,2,2)";
            var playersAtPosition = Enumerable.Range(1, 3)
                .Select(iteration => PlayerRecordGenerator
                    .CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(position)
                    .AsModel());

            MockPlayerRepository
                .Setup(repo => repo.GetActivePlayersAt(It.IsAny<Position>()))
                .ReturnsAsync(playersAtPosition);

            var returnedObject = Controller.ComputeListOfPlayersAt(position).Result as OkObjectResult;
            var retrievedPlayerRecords = returnedObject.Value as IEnumerable<PlayerRecord>;

            retrievedPlayerRecords.Count().Should().Be(3);
        }

        [Fact]
        public void ComputeListOfPLayersAt_Return_No_Content_When_No_Players_Are_In_The_Room_Returns_NotFound_Object()
        {
            // Arrange
            string position = "(3,3,3)";
            MockPlayerRepository.Setup(repo => repo.GetActivePlayersAt(It.IsAny<Position>()))
                .ReturnsAsync(Enumerable.Empty<PlayerRecordModel>());

            // Act
            IActionResult returnedObject = Controller.ComputeListOfPlayersAt(position).Result;
            var noContentResult = returnedObject as NoContentResult;

            // Assert
            noContentResult.Should().NotBeNull();
        }
        
    }
}
