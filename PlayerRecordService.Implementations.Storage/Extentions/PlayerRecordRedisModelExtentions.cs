using System.Text.Json;
using SkycavePlayerService.Implementations.Storage.Models;
using SkycavePlayerService.Shared.Models.PlayerRecord;

namespace SkycavePlayerService.Implementations.Storage.Extentions
{
    internal static class PlayerRecordRedisModelExtentions
    {
        public static string AsJsonString(this PlayerRecordRedisModel redisPlayerRecord)
        {
           return JsonSerializer.Serialize(redisPlayerRecord);
        }

        public static PlayerRecordRedisModel FromJson(this string jsonstring)
        {
            return JsonSerializer.Deserialize<PlayerRecordRedisModel>(jsonstring);
        }

        public static PlayerRecordModel AsPlayerRecordModel(this PlayerRecordRedisModel redisPlayerRecord)
        {

            return new PlayerRecordModel(
                redisPlayerRecord.PlayerId,
                redisPlayerRecord.PlayerName,
                redisPlayerRecord.GroupName,
                redisPlayerRecord.Region,
                redisPlayerRecord.Position,
                redisPlayerRecord.AccessToken
                );
        }

    }
}
