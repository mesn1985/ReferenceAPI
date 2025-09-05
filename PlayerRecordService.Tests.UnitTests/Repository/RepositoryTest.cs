using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Moq;
using PlayerRecordService.Exceptions;
using PlayerRecordService.Implementations.Repositories;
using PlayerRecordService.Shared.Contracts;
using PlayerRecordService.Shared.Models.PlayerRecord;
using PlayerRecordService.Shared.Models.PlayerRecord.Primitives;

namespace PlayerRecordService.Tests.UnitTests.Repository
{
    public class PlayerRepositoryTests
    {
        private readonly Mock<IPlayerStorage> _storageMock;
        private readonly PlayerRepository _repository;

        public PlayerRepositoryTests()
        {
            _storageMock = new Mock<IPlayerStorage>();
            _repository = new PlayerRepository(_storageMock.Object);
        }

        [Fact]
        public async Task GetActivePlayersAt_Can_Get_Active_Player_At_Position_Returns_Initialized_PlayerRecordModelObject()
        {
            // Arrange
            var position = new Position("(0,0,0)");
            var expectedPlayers = new List<PlayerRecordModel>();

            _storageMock.Setup(s => s.GetPlayersWithNonNullAccessTokenAt(position)).ReturnsAsync(expectedPlayers);

            // Act
            var result =  await _repository.GetActivePlayersAt(position);

            // Assert
            Assert.Equal(expectedPlayers, result);
        }
        
        [Fact]
        public async Task GetPlayerRecordBy_PlayerId_ReturnsPlayerRecord()
        {
            // Arrange
            var playerId = new PlayerId("someId");
            var expectedPlayer
                = new PlayerRecordModel(playerId.Value, "John", "Group1", Region.AARHUS, "(1,1,1)", "token123");

            _storageMock.Setup(s => s.GetPlayerRecordBy(playerId)).ReturnsAsync(expectedPlayer);

            // Act
            var result =  await _repository.GetPlayerRecordBy(playerId);

            // Assert
            Assert.Equal(expectedPlayer, result);
        }

        [Fact]
        public async Task AddPlayer_PlayerDoesNotExist_AddsPlayer()
        {
            // Arrange
            var player = new PlayerRecordModel("someId", "John", "Group1", Region.AARHUS, "(2,2,2)", "token123");

            _storageMock.Setup(s => s.DoesPlayerExist(player.PlayerId)).ReturnsAsync(false);

            // Act
            await _repository.AddPlayer(player);

            // Assert
            _storageMock.Verify(s => s.AddPlayer(player), Times.Once);
        }

        [Fact]
        public async Task AddPlayer_PlayerExists_ThrowsPlayerExistException()
        {
            // Arrange
            var player = new PlayerRecordModel("someId", "John", "Group1", Region.AARHUS, "(3,3,3)", "token123");

            _storageMock.Setup(s => s.DoesPlayerExist(player.PlayerId)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<PlayerExistException>(() => _repository.AddPlayer(player));
        }

        [Fact]
        public async Task UpdatePlayer_PlayerExists_UpdatesPlayer()
        {
            // Arrange
            var player = new PlayerRecordModel("someId", "John", "Group1", Region.AARHUS, "(4,4,4)", "token123");

            _storageMock.Setup(s => s.DoesPlayerExist(player.PlayerId)).ReturnsAsync(true);

            // Act
            await _repository.UpdatePlayer(player);

            // Assert
            _storageMock.Verify(s => s.UpdatePlayer(player), Times.Once);
        }

        [Fact]
        public async Task UpdatePlayer_PlayerDoesNotExist_ThrowsPlayerDoesNotExistException()
        {
            // Arrange
            var player = new PlayerRecordModel("someId", "John", "Group1", Region.AARHUS, "(5,5,5)", "token123");

            _storageMock.Setup(s => s.DoesPlayerExist(player.PlayerId)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<PlayerDoesNotExistException>(() => _repository.UpdatePlayer(player));
        }
    }
}
