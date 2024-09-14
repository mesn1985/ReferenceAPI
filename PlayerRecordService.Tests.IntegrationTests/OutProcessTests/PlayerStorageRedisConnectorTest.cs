using Microsoft.Extensions.Configuration;
using PlayerRecordService.api.DTOs;
using PlayerRecordService.Implementations.Storage;
using PlayerRecordService.Shared.Contracts;
using PlayerRecordService.Shared.Models.PlayerRecord;
using PlayerRecordService.Shared.Models.PlayerRecord.Primitives;
using PlayerRecordService.Tests.TestUtilities;
using PlayerRecordService.Tests.TestUtilities.Extensions;

namespace PlayerRecordService.Tests.IntegrationTests.OutProcessTests;

public class PlayerStorageRedisConnectorTest : IClassFixture<RedisContainerFixture>
{
    private PlayerRecordGenerator _playerRecordGenerator;
    private RedisContainerFixture containerFixture;

    public PlayerStorageRedisConnectorTest(RedisContainerFixture containerFixture)
    {
        _playerRecordGenerator = new PlayerRecordGenerator();
        this.containerFixture = containerFixture;

        
    }
    [Fact]
    public async Task GetPlayersWithNonNullAccessTokenAt_Returns_List_With_Single_Player_With_Non_Null_Access_Token_At_Position()
    {
        Position positionToGetPlayerFrom = new Position("(1,1,1)");

        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );
        PlayerRecordModel playerAtPostion =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(positionToGetPlayerFrom
                .Value).AsModel();
        await AddPlayerToStorage(playerAtPostion,playerStorage);

        PlayerRecordModel playerNotAtPosition = createPlayerRecordModelWithNullToken(positionToGetPlayerFrom.Value);
        await AddPlayerToStorage(playerNotAtPosition,playerStorage);

        IEnumerable<PlayerRecordModel> retrievedPlayers = await playerStorage.GetPlayersWithNonNullAccessTokenAt(positionToGetPlayerFrom);

        PlayerRecordModel retrievedPlayer = retrievedPlayers.First();

        retrievedPlayers.Count().Should().Be(1);
        retrievedPlayer.IsEqualTo(playerAtPostion).Should().BeTrue();
    }

    [Fact]
    public async Task GetPlayersWithNonNullAccessTokenAt_Returns_List_Of_Players_With_Non_Null_Access_Token_At_Position()
    {
        Position positionToGetPlayerFrom = new Position("(2,2,2)");
        Position positionNotToGetPlayersFrom = new Position("(3,3,3)");

        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );
        PlayerRecordModel playerAtPosition1 =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(
                positionToGetPlayerFrom.Value).AsModel();
        PlayerRecordModel playerAtPosition2 =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(
                positionToGetPlayerFrom.Value).AsModel();
        PlayerRecordModel playerAtPosition3 =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(
                positionToGetPlayerFrom.Value).AsModel();

        await playerStorage.AddPlayer(playerAtPosition1);
        await playerStorage.AddPlayer(playerAtPosition2);
        await playerStorage.AddPlayer(playerAtPosition3);
        IEnumerable<PlayerRecordModel> playersNotAtPosition =
            createListOfPlayersWithAccessTokenPosition(positionNotToGetPlayersFrom.Value,4);
        await AddPlayerModelsToStorage(playersNotAtPosition,playerStorage);


        IEnumerable<PlayerRecordModel> playerRecordModels =
            await playerStorage.GetPlayersWithNonNullAccessTokenAt(positionToGetPlayerFrom);


        playerRecordModels.Should().HaveCount(3);
        playerRecordModels.Any(recordFromRedis =>
            recordFromRedis.IsEqualTo(playerAtPosition1)).Should().BeTrue();
        playerRecordModels.Any(recordFromRedis =>
            recordFromRedis.IsEqualTo(playerAtPosition2)).Should().BeTrue();
        playerRecordModels.Any(recordFromRedis =>
            recordFromRedis.IsEqualTo(playerAtPosition3)).Should().BeTrue();
    }
    [Fact]
    public async Task GetPlayersWithNonNullAccessTokenAt_Returns_Empty_List_When_All_Players_At_Postion_Have_Null_Access_Token()
    {
        Position positionToGetPlayerFrom = new Position("(4,4,4)");

        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );
        
        IEnumerable<PlayerRecordModel> playersAtPositionWithNullAccesToken
            = createListOfPlayersWithNullAccessTokenPosition(positionToGetPlayerFrom.Value, 3);
        await AddPlayerModelsToStorage(playersAtPositionWithNullAccesToken, playerStorage);



        IEnumerable<PlayerRecordModel> retrievedPlayersAtPosition
            = await playerStorage.GetPlayersWithNonNullAccessTokenAt(positionToGetPlayerFrom);

        retrievedPlayersAtPosition.Count().Should().Be(0);
    }
    [Fact]
    public async Task GetPlayersWithNonNullAccessTokenAt_Returns_Empty_List_When_All_Players_At_Postion_Have_Void_Access_Token()
    {
        Position positionToGetPlayerFrom = new Position("(5,5,5)");

        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );

        IEnumerable<PlayerRecordModel> playersAtPositionWithNullAccesToken
            = createListOfPlayersWithVoidAccessTokenPosition(positionToGetPlayerFrom.Value, 3);
        await AddPlayerModelsToStorage(playersAtPositionWithNullAccesToken, playerStorage);



        IEnumerable<PlayerRecordModel> retrievedPlayersAtPosition
            = await playerStorage.GetPlayersWithNonNullAccessTokenAt(positionToGetPlayerFrom);

        retrievedPlayersAtPosition.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetPlayerRecordBy_Returns_PlayerRecord()
    {
        //Arrange
        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );
        PlayerRecordModel playerToRetrieve =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition("(6,6,6)").AsModel();
        await playerStorage.AddPlayer(playerToRetrieve);

        //Act
        PlayerRecordModel retrievedPlayerRecordModel 
            = await playerStorage.GetPlayerRecordBy(playerToRetrieve.PlayerId);

        //Assert
        retrievedPlayerRecordModel.IsEqualTo(playerToRetrieve).Should().BeTrue();

    }

    [Fact]
    public async Task GetPlayerRecordBy_With_Not_Existing_Player_Should_Return_Null()
    {
        //Arrange
        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );

        //Act
        PlayerRecordModel retrievedPlayerRecordModel
            = await playerStorage.GetPlayerRecordBy(new PlayerId("Non Existing id"));

        //Assert
        retrievedPlayerRecordModel.Should().BeNull();

    }

    [Fact]
    public async Task AddPlayer_AddsPlayerToStorage_Can_Retrieve_Player_After_Adding_Player()
    {

        IPlayerStorage playerStorage 
                = new PlayerServiceRedisStorage(
                    GetConnectionConfigurationForRedisContainer()
            );

        PlayerRecordModel playerToAdd =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition("(7,7,7)").AsModel();

        await playerStorage.AddPlayer(playerToAdd);

        PlayerRecordModel retrievedPlayer =  await playerStorage.GetPlayerRecordBy(playerToAdd.PlayerId);

        retrievedPlayer.IsEqualTo(playerToAdd).Should().BeTrue();

    }

    [Fact]
    public async Task UpdatePlayer_UpdatesPlayerInStorage()
    {
        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );
        PlayerRecordModel playerToAdd =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition("(8,8,8)").AsModel();
        await playerStorage.AddPlayer(playerToAdd);
        PlayerRecordModel playerWithUpdatedPosition = playerToAdd.CreateDeepCloneWithNewPosition("(9,9,9)");

        //Act
        await playerStorage.UpdatePlayer(playerWithUpdatedPosition);
        PlayerRecordModel retrievedPlayer = await playerStorage.GetPlayerRecordBy(playerToAdd.PlayerId);


        //Assert
        retrievedPlayer.Position.Value.Should().Be(playerWithUpdatedPosition.Position.Value);


    }

    [Fact]
    public async Task DoesPlayerExist_With_Exsisting_Player_Returns_True()
    {
        //Arrange
        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );
        PlayerRecordModel playerToAdd =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition("(10,10,10)").AsModel();
        await playerStorage.AddPlayer(playerToAdd);

        //Act
        bool playerExist = await playerStorage.DoesPlayerExist(playerToAdd.PlayerId);

        playerExist.Should().BeTrue();

    }

    [Fact]
    public async Task DoesPlayerExist_With_Non_Existing_Player_Returns_False()
    {
        //Arrange
        IPlayerStorage playerStorage
            = new PlayerServiceRedisStorage(
                GetConnectionConfigurationForRedisContainer()
            );

        //Act
        bool playerExist = await playerStorage.DoesPlayerExist(new PlayerId("Non Existing player id"));

        playerExist.Should().BeFalse();
    }

    private IConfiguration GetConnectionConfigurationForRedisContainer()
    {
        return containerFixture.GetConnectionConfigurationForRedisContainer();
    }

    private IEnumerable<PlayerRecordModel> createListOfPlayersWithAccessTokenPosition(string position, int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            yield return _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(position).AsModel();
        }
    }
    private IEnumerable<PlayerRecordModel> createListOfPlayersWithNullAccessTokenPosition(string position, int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            PlayerRecord playerRecord =
                _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(position);
            playerRecord.AccessToken = null;
            yield return playerRecord.AsModel();
        }
    }
    private IEnumerable<PlayerRecordModel> createListOfPlayersWithVoidAccessTokenPosition(string position, int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            PlayerRecord playerRecord =
                _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(position);
            playerRecord.AccessToken = "void";
            yield return playerRecord.AsModel();
        }
    }

    

    private PlayerRecordModel createPlayerRecordModelWithNullToken(string position)
    {
        PlayerRecord playerRecord =
            _playerRecordGenerator.CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(position);
        playerRecord.AccessToken = null;
        return playerRecord.AsModel();

    }

    private async Task AddPlayerToStorage(PlayerRecordModel playerRecord, IPlayerStorage playerStorage)
    {
        await playerStorage.AddPlayer(playerRecord);
    }

    private async Task AddPlayerModelsToStorage(IEnumerable<PlayerRecordModel> playerModelsToAdd,
        IPlayerStorage playerStorage)
    {
        foreach (PlayerRecordModel playerRecordModel in playerModelsToAdd)
        {
            await playerStorage.AddPlayer(playerRecordModel);
        }
    }

    

}