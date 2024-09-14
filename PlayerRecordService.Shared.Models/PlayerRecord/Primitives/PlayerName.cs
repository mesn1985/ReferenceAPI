namespace PlayerRecordService.Shared.Models.PlayerRecord.Primitives
{
    public class PlayerName : ValueObjectBase<string>
    {
        public PlayerName(string value) : base(value)
        {
        }

        protected override bool IsValid(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                return false;
            }

            return true;
        }
    }
}
