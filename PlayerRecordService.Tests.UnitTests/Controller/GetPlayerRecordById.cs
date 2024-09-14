using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlayerRecordService.api.DTOs;
using PlayerRecordService.Shared.Models.PlayerRecord.Primitives;
using PlayerRecordService.Tests.TestUtilities.Extensions;

namespace PlayerRecordService.Tests.UnitTests.Controller
{
    public class GetPlayerRecordById : BasePlayerControllerTest
    {
        [Fact]
        public void GetPlayerRecordById_Can_Get_Player_Based_on_Player_Id_Returns_Ok_Object()
        {
            //Arrange
            string playersPosition = "(1,1,1)";
            var playerRecordModel = PlayerRecordGenerator
                .CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(playersPosition)
                .AsModel();
            MockPlayerRepository
                .Setup(repo => repo.GetPlayerRecordBy(It.IsAny<PlayerId>()))
                .ReturnsAsync(playerRecordModel);

            //Act
            var playerRecord = (Controller.GetPlayerRecordById(playerRecordModel.PlayerId.Value).Result as OkObjectResult)?.Value as PlayerRecord;

            //Assert
            playerRecord.PlayerId.Should().Be(playerRecordModel.PlayerId.Value);
        }
        [Fact]
        public void GetPlayerRecordById_Can_Request_Non_Exisisting_Player_Returns_NotFound_Object()
        {

            //Act
            IActionResult result = Controller.GetPlayerRecordById("NonExsistingPlayerId").Result;

            //Assert
            NotFoundObjectResult responseObject = result as NotFoundObjectResult;

        }


    }
}
