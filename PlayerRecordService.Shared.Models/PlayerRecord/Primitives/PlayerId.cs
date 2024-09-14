namespace PlayerRecordService.Shared.Models.PlayerRecord.Primitives
{
    public class PlayerId : ValueObjectBase<string>
    {
        public PlayerId(string playerId) : base(playerId)
        { }

        protected override bool IsValid(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
            {
                return false;
            }

            return true;
        }
    }
}
