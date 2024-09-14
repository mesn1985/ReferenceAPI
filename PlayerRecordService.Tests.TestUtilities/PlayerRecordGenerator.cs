using PlayerRecordService.api.DTOs;
using PlayerRecordService.Shared.Models.PlayerRecord;

namespace PlayerRecordService.Tests.TestUtilities
{
    public class PlayerRecordGenerator
    {
        public PlayerRecord CreatePlayerRecordObjectWithSeuquentielGUIDIdAtPosition(string position = "(1,2,3)")
        {

            return new PlayerRecord
            {
                AccessToken = "token",
                GroupName = "groupName",
                PlayerName = "playerName",
                Position = position,
                PlayerId = Guid.NewGuid().ToString(),
                Region = Region.AARHUS
            };
        }
    }
}