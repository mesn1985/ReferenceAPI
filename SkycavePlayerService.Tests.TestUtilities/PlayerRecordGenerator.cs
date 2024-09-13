using SkycavePlayerService.api.DTOs;
using SkycavePlayerService.Shared.Models.PlayerRecord;

namespace SkycavePlayerService.Tests.TestUtilities
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