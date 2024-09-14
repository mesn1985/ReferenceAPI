using PlayerRecordService.Shared.Models.PlayerRecord;
using PlayerRecordService.Shared.Models.PlayerRecord.Primitives;

namespace PlayerRecordService.Shared.Contracts
{
    public interface IPlayerStorage : IDisposable
    {
        public Task<IEnumerable<PlayerRecordModel>> GetPlayersWithNonNullAccessTokenAt(Position position);
        public Task<PlayerRecordModel> GetPlayerRecordBy(PlayerId playerId);
        public Task AddPlayer(PlayerRecordModel player);
        public Task UpdatePlayer(PlayerRecordModel player);
        public Task<Boolean> DoesPlayerExist(PlayerId playerId);

    }
}
