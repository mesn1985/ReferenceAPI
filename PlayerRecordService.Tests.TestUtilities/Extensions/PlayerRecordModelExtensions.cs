using PlayerRecordService.Shared.Models.PlayerRecord;

namespace PlayerRecordService.Tests.TestUtilities.Extensions
{
    public static class PlayerRecordModelExtensions
    {
        public static bool IsEqualTo(this PlayerRecordModel player1, PlayerRecordModel player2)
        {
            if (player1 == null || player2 == null)
                return false;

            return player1.PlayerId.Value.Equals(player2.PlayerId.Value) &&
                   player1.PlayerName.Value.Equals(player2.PlayerName.Value) &&
                   player1.GroupName.Value.Equals(player2.GroupName.Value) &&
                   player1.Region == player2.Region &&
                   player1.Position.Value.Equals(player2.Position.Value) &&
                   player1.AccessToken.Value.Equals(player2.AccessToken.Value);
        }

        public static PlayerRecordModel CreateDeepCloneWithNewPosition(this PlayerRecordModel playerRecordModel, string position)
        {
            return new PlayerRecordModel(
                    playerRecordModel.PlayerId.Value,
                    playerRecordModel.PlayerName.Value,
                    playerRecordModel.GroupName.Value,
                    playerRecordModel.Region,
                    position,
                    playerRecordModel.AccessToken.Value
                );
        }
    }
}
