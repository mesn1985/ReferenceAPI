using PlayerRecordService.Exceptions;
using PlayerRecordService.Shared.Contracts;
using PlayerRecordService.Shared.Models.PlayerRecord;
using PlayerRecordService.Shared.Models.PlayerRecord.Primitives;

namespace PlayerRecordService.Implementations.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private IPlayerStorage _storage;

        public PlayerRepository(IPlayerStorage storage)
        {
            _storage = storage;
        }

        public void Dispose()
        {
            _storage.Dispose();
        }
        /// <summary>
        /// Returns active players at position (Players with null accesstoken or a token that says void, is not considered active players)
        /// </summary>
        /// <param name="position"></param>
        /// <returns>A list of all active players at the specified position</returns>
        public async Task<IEnumerable<PlayerRecordModel>> GetActivePlayersAt(Position position)
        {
            return await _storage.GetPlayersWithNonNullAccessTokenAt(position);
        }
        /// <summary>
        /// Get player with specific ID.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns>PlayerRecordModel if the player is found, else null is returned</returns>
        public async Task<PlayerRecordModel> GetPlayerRecordBy(PlayerId playerId)
        {
            return await _storage.GetPlayerRecordBy(playerId);
        }
        /// <summary>
        /// Using Exception here is crucial, as overwritting adding an already existing player
        /// Could be done without any special note, using storage such as reddis and alike. Other storage types might not be prone to this risk.
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <exception cref="PlayerExistException"></exception>
        public async Task AddPlayer(PlayerRecordModel player)
        {
            Boolean playerExist = await _storage.DoesPlayerExist(player.PlayerId);

            if (playerExist)
            {
                throw new PlayerExistException(
                    $"Player with id: ${player.PlayerId} already exist, and cannot be updated"
                    );
            }
            else
            {
               await _storage.AddPlayer(player);
            }
        }
        /// <summary>
        /// Exception is used to signal wheter the player to update already exist.
        /// This is to keep consistent with the AddPlayer method, as these to will be used togheter
        /// </summary>
        /// <param name="player"></param>
        /// <exception cref="PlayerExistException"></exception>
        public async Task UpdatePlayer(PlayerRecordModel player)
        {
            Boolean playerExist = await _storage.DoesPlayerExist(player.PlayerId);

            if (playerExist)
            {

               await _storage.UpdatePlayer(player);
            }
            else
            {
                throw new PlayerDoesNotExistException($"Player with id: ${player.PlayerId} does not exist, and cannot be updated");
            }


        }

    }
}