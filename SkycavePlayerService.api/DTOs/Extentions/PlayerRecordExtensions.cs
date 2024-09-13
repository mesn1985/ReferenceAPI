using SkycavePlayerService.Shared.Models.PlayerRecord;
using SkycavePlayerService.Shared.Models.PlayerRecord.Primitives;

namespace SkycavePlayerService.api.DTOs.Extentions
{
    public static class PlayerRecordExtensions
    {
        public static PlayerRecord AsDto(this PlayerRecordModel playerRecord)
        {
            return new PlayerRecord()
            {
                PlayerId = playerRecord.PlayerId.Value,
                PlayerName = playerRecord.PlayerName.Value,
                GroupName = playerRecord.GroupName.Value,
                Region = playerRecord.Region,
                Position = playerRecord.Position.Value,
                AccessToken = playerRecord.AccessToken.Value
            };
        }

        public static PlayerRecordModel AsModel(this PlayerRecord record)
        {
            return new PlayerRecordModel(
                record.PlayerId, 
                record.PlayerName, 
                record.GroupName, 
                record.Region,
                record.Position, 
                record.AccessToken
                );
        }

    }
}
