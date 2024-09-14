using Microsoft.Extensions.Configuration;
using PlayerRecordService.Implementations.Storage.Extentions;
using PlayerRecordService.Implementations.Storage.Models;
using PlayerRecordService.Shared.Contracts;
using PlayerRecordService.Shared.Models.PlayerRecord;
using PlayerRecordService.Shared.Models.PlayerRecord.Primitives;
using StackExchange.Redis;

namespace PlayerRecordService.Implementations.Storage
{
    public class PlayerServiceRedisStorage : IPlayerStorage
    {
        private  ConnectionMultiplexer connectionMultiplexer;
        private readonly RedisDatabaseProvider databaseProvider;

        public PlayerServiceRedisStorage(IConfiguration configuration)
        {

            string redisUrl = configuration["ConnectionStrings:Redis"];

            //connectionMultiplexer = ConnectionMultiplexer.Connect(redisUrl);
            databaseProvider = new RedisDatabaseProvider(redisUrl);

        }

        public async Task AddPlayer(PlayerRecordModel player)
        {
            IDatabase db = databaseProvider.GetDatabase();
            //IDatabase db = connectionMultiplexer.GetDatabase();
            ITransaction dbTransaction = db.CreateTransaction();
            
            PlayerRecordRedisModel redisPlayerRecord = player.asRedisModel();

            string playerRedisKey = $"player({redisPlayerRecord.PlayerId})";

            // Update position and record fields separately
            dbTransaction.HashSetAsync(playerRedisKey, "position", redisPlayerRecord.Position);
            dbTransaction.HashSetAsync(playerRedisKey, "record", redisPlayerRecord.AsJsonString());

            // Add player to the room
            dbTransaction.SetAddAsync($"room{redisPlayerRecord.Position}players", redisPlayerRecord.AsJsonString());

            await dbTransaction.ExecuteAsync();
        }

        public void Dispose()
        {
           //Left blank, because Multiplexer should not actively be closed
        }

        public async Task<bool> DoesPlayerExist(PlayerId playerId)
        {
            IDatabase db = databaseProvider.GetDatabase();

            return await db.KeyExistsAsync($"player({playerId.Value})");
        }

        public async Task<PlayerRecordModel> GetPlayerRecordBy(PlayerId playerId)
        {
            IDatabase db = databaseProvider.GetDatabase();


            string playerRedisId = $"player({playerId.Value})";

            var recordAsJRedisValue 
                = await db.HashGetAsync(playerRedisId, "record");

            string recordAsJson = recordAsJRedisValue.ToString();


            if (string.IsNullOrEmpty(recordAsJson))
            {
                return null;
            }

            PlayerRecordRedisModel retrievedPlayerRecord = recordAsJson.FromJson();

            return retrievedPlayerRecord.AsPlayerRecordModel();

        }

        public async Task<IEnumerable<PlayerRecordModel>> GetPlayersWithNonNullAccessTokenAt(Position position)
        {
            string playerInRoomKey = $"room{position.Value}players";
            IDatabase db = databaseProvider.GetDatabase();

            var playersInRoomRedisValue = await db.SetMembersAsync(playerInRoomKey);

            var playersInRoom = playersInRoomRedisValue.ToStringArray();

            List<PlayerRecordModel> listOfPlayerRecordsInRoom = new List<PlayerRecordModel>();

            foreach (string record in playersInRoom)
            {
                PlayerRecordModel playerRecordModel = record.FromJson().AsPlayerRecordModel();

                if (playerRecordModel.AccessToken.Value != null && !playerRecordModel.AccessToken.Value.Equals("void"))
                {
                    listOfPlayerRecordsInRoom.Add(playerRecordModel);;
                }

            }

            return listOfPlayerRecordsInRoom;
        }

        public async Task UpdatePlayer(PlayerRecordModel player)
        {
            string playerRedisKey = $"player({player.PlayerId.Value})";

            //IDatabase db = connectionMultiplexer.GetDatabase();
            IDatabase db = databaseProvider.GetDatabase();


            PlayerRecordRedisModel newRedisPlayerRecord = player.asRedisModel();
            var oldRedisPlayerRecordRedisValue = await db.HashGetAsync(playerRedisKey, "record");
            PlayerRecordRedisModel oldRedisPlayerRecord = oldRedisPlayerRecordRedisValue.ToString().FromJson();

            ITransaction transaction = db.CreateTransaction();

            
            transaction.HashSetAsync(playerRedisKey, "position", newRedisPlayerRecord.Position);
            transaction.HashSetAsync(playerRedisKey, "record", newRedisPlayerRecord.AsJsonString());

            // Add player to the room
            transaction.SetRemoveAsync($"room{oldRedisPlayerRecord.Position}players", oldRedisPlayerRecord.AsJsonString());
            transaction.SetAddAsync($"room{newRedisPlayerRecord.Position}players", newRedisPlayerRecord.AsJsonString());

            await transaction.ExecuteAsync();
        }
    }
}
