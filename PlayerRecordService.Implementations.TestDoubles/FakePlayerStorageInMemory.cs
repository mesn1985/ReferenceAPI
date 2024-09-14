using SkycavePlayerService.Shared.Contracts;
using SkycavePlayerService.Shared.Models.PlayerRecord;
using SkycavePlayerService.Shared.Models.PlayerRecord.Primitives;

namespace SkycavePlayerService.Implementations.TestDoubles
{
    public class FakePlayerStorageInMemory : IPlayerStorage
    {
        private Dictionary<string, PlayerRecordModel> playersDictionary = new Dictionary<string, PlayerRecordModel>();

        public void Dispose()
        {
            //Left empty intentionally, as there is no resources that needs closing
        }

        public FakePlayerStorageInMemory(Dictionary<string,PlayerRecordModel> playersDictionary = null)
        {
                this.playersDictionary  = playersDictionary ?? new Dictionary<string, PlayerRecordModel>();
        }

        public async Task<IEnumerable<PlayerRecordModel>> GetPlayersWithNonNullAccessTokenAt(Position position)
        {
            return playersDictionary
                .Values
                .Where(
                    playerRecordModel => 
                        playerRecordModel.AccessToken.Value != null && playerRecordModel.Position.Value == position.Value
                    )
                .ToList();
        }

        public async Task<PlayerRecordModel> GetPlayerRecordBy(PlayerId playerId)
        {
            if (!playersDictionary.ContainsKey(playerId.Value))
            {
                return null;
            }

            return playersDictionary[playerId.Value];
        }

        public async Task AddPlayer(PlayerRecordModel player)
        {
            playersDictionary.Add(player.PlayerId.Value,player);
        }

        public async Task UpdatePlayer(PlayerRecordModel player)
        {

            playersDictionary[player.PlayerId.Value] = player;
        }

        public async Task<bool> DoesPlayerExist(PlayerId playerId)
        {
            return playersDictionary.ContainsKey(playerId.Value);
        }
    }
}