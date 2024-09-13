using SkycavePlayerService.Implementations.TestDoubles;
using SkycavePlayerService.Shared.Models.PlayerRecord;
using SkycavePlayerService.Shared.Models.PlayerRecord.Primitives;

namespace SkycavePlayerService.Tests
{
    public class FakePlayerStorageInMemoryTests
    {
        [Fact]
        public void GetPlayersWithNonNullAccessTokenAt_Returns_Players_With_Non_Null_Access_Token_At_Position()
        {
            // Arrange
            var position = new Position("(2,2,2)");
            var player1 = new PlayerRecordModel("player1", "Player 1", "Group1", Region.AARHUS, position.Value, "token1");
            var player2 = new PlayerRecordModel("player2", "Player 2", "Group2", Region.COPENHAGEN, position.Value, null); // Null AccessToken
            var player3 = new PlayerRecordModel("player3", "Player 3", "Group3", Region.ODENSE, position.Value, "token3");

            var playersDictionary = new Dictionary<string, PlayerRecordModel>
            {
                { player1.PlayerId.Value, player1 },
                { player2.PlayerId.Value, player2 },
                { player3.PlayerId.Value, player3 }
            };

            var storage = new FakePlayerStorageInMemory(playersDictionary);

            // Act
            var result = storage.GetPlayersWithNonNullAccessTokenAt(position).Result;

            // Assert
            Assert.Equal(2, result.Count());
            Assert.True(result.Any(playerRecordModel 
                            => playerRecordModel.PlayerId.Value.Equals(player1.PlayerId.Value)));
            Assert.True(result.Any(playerRecordModel
                => playerRecordModel.PlayerId.Value.Equals(player3.PlayerId.Value)));

        }

        [Fact]
        public void GetPlayerRecordBy_ReturnsPlayerRecordIfExists()
        {
            // Arrange
            var playerId = new PlayerId("player1");
            var player = new PlayerRecordModel(playerId.Value, "Player 1", "Group1", Region.AARHUS, "(5,5,5)", "token1");

            var playersDictionary = new Dictionary<string, PlayerRecordModel>
            {
                { player.PlayerId.Value, player }
            };

            var storage = new FakePlayerStorageInMemory(playersDictionary);

            // Act
            var result = storage.GetPlayerRecordBy(playerId).Result;

            // Assert
            Assert.Equal(player, result);
        }

        [Fact]
        public void GetPlayerRecordBy_ReturnsNullIfPlayerDoesNotExist()
        {
            // Arrange
            var playerId = new PlayerId("player1");

            var storage = new FakePlayerStorageInMemory();

            // Act
            var result = storage.GetPlayerRecordBy(playerId).Result;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddPlayer_AddsPlayerToStorage()
        {
            // Arrange
            var player = new PlayerRecordModel("player1", "Player 1", "Group1", Region.AARHUS, "(6,6,6)", "token1");

            var storage = new FakePlayerStorageInMemory();

            // Act
            storage.AddPlayer(player);
            
            // Assert
            Assert.True(storage.DoesPlayerExist(player.PlayerId).Result);
        }

        [Fact]
        public void UpdatePlayer_UpdatesPlayerInStorage()
        {
            // Arrange
            var player = new PlayerRecordModel("player1", "Player 1", "Group1", Region.AARHUS, "(7,7,7)", "token1");

            var storage = new FakePlayerStorageInMemory();
            storage.AddPlayer(player);

            var updatedPlayer = new PlayerRecordModel("player1", "Updated Player", "Group2", Region.COPENHAGEN, "(8,8,8)", "token2");

            // Act
            storage.UpdatePlayer(updatedPlayer);

            // Assert
            Assert.True(storage.DoesPlayerExist(player.PlayerId).Result);
        }

        [Fact]
        public void DoesPlayerExist_ReturnsTrueIfPlayerExists()
        {
            // Arrange
            var playerId = new PlayerId("player1");
            var player = new PlayerRecordModel(playerId.Value, "Player 1", "Group1", Region.AARHUS, "(9,9,9)", "token1");

            var storage = new FakePlayerStorageInMemory();
            storage.AddPlayer(player);

            // Act
            var result = storage.DoesPlayerExist(playerId).Result;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DoesPlayerExist_ReturnsFalseIfPlayerDoesNotExist()
        {
            // Arrange
            var playerId = new PlayerId("player1");

            var storage = new FakePlayerStorageInMemory();

            // Act
            var result = storage.DoesPlayerExist(playerId).Result;

            // Assert
            Assert.False(result);
        }
    }
}
