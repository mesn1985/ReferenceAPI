using SkycavePlayerService.Shared.Models.PlayerRecord.Primitives;
using SkycavePlayerService.Shared.Models.PlayerRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkycavePlayerService.Shared.Contracts
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
