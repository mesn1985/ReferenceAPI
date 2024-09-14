using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SkycavePlayerService.Exceptions;
using FluentAssertions;
using SkycavePlayerService.Shared.Models.PlayerRecord;
using SkycavePlayerService.Tests.TestUtilities.Extensions;

namespace SkycavePlayerService.Tests.UnitTests.Controller
{
    public class UpdatePlayerRecord : BasePlayerControllerTest
    {
        [Fact]
        public void UpdatePlayerRecord_Adds_New_Player_And_Returns_Created_Result()
        {
            // Arrange
            var playerRecordDto = PlayerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition();

            MockPlayerRepository.Setup(x => x.AddPlayer(It.IsAny<PlayerRecordModel>()));

            // Act
            var result = Controller.UpdatePlayerRecord(playerRecordDto).Result as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.GetType().Should().Be(typeof(CreatedAtActionResult));
    
        }

        [Fact]
        public void UpdatePlayerRecord_Updates_Existing_Player_And_Returns_Ok_Result()
        {
            // Arrange
            var playerRecordDto = PlayerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition();
            var playerRecordModel = playerRecordDto.AsModel();

            MockPlayerRepository.Setup(x => x.AddPlayer(It.IsAny<PlayerRecordModel>())).Throws<PlayerExistException>();


            // Act
            var result = Controller.UpdatePlayerRecord(playerRecordDto).Result as StatusCodeResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }
}
