using SkycavePlayerService.Implementations.Storage.Models;
using SkycavePlayerService.Shared.Models.PlayerRecord;

namespace SkycavePlayerService.Implementations.Storage.Extentions
{
    internal static class PlayerRecordModelExtentions
    {
        public static PlayerRecordRedisModel asRedisModel(this PlayerRecordModel playerRecordModel)
        {
            return new PlayerRecordRedisModel()
            {
                PlayerId = playerRecordModel.PlayerId.Value,
                PlayerName = playerRecordModel.PlayerName.Value,
                GroupName = playerRecordModel.GroupName.Value,
                Region = playerRecordModel.Region,
                Position = playerRecordModel.Position.Value,
                AccessToken = playerRecordModel.AccessToken.Value
            };
        }

    }
}
