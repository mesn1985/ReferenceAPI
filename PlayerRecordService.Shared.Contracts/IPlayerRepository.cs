using PlayerRecordService.Shared.Models.PlayerRecord;
using PlayerRecordService.Shared.Models.PlayerRecord.Primitives;

namespace PlayerRecordService.Shared.Contracts
{
    /// <summary>
    /// The repository class handles all the intermediate logic between the Controllers,
    /// and the Connector implementations. And provides abstractions 
    /// </summary>
    public interface IPlayerRepository : IDisposable
    {
        /// <summary>
        /// Retrieves All player with an active session at specified position
        /// </summary>
        /// <param name="position">Cave position in format ([0-9],[0-9],[0-9])</param>
        /// <returns>IEnumerable containing Player Records</returns>
        public  Task<IEnumerable<PlayerRecordModel>> GetActivePlayersAt(Position position);
        /// <summary>
        /// Retrieves a Player record based on the player id.
        /// </summary>
        /// <param name="playerId">The id of the player that should be retrieved</param>
        /// <returns>Returns an instance of PlayerRecordModel if a player with the id is found in the storage, else null is returned</returns>
        public Task<PlayerRecordModel> GetPlayerRecordBy(PlayerId playerId);
        /// <summary>
        /// Add a new player to player storage
        /// </summary>
        /// <param name="player">The player record with the values</param>
        /// <returns>Nothing, but throws an exception if updating the player failed (E.g. the player already exists)</returns>
        /// TODO: Implement Unit test to ensure an exception is thrown if player already exist
        public Task AddPlayer(PlayerRecordModel player);
        /// <summary>
        /// Update player if player does not already exist.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>Nothing, but throws an Exception if the player update failed</returns>
        /// TODO: Implement Unit test to ensure an exception is thrown if player does not exist
        public Task UpdatePlayer(PlayerRecordModel player);

    }
}
