namespace PlayerRecordService.Shared.Models.PlayerRecord.Primitives
{
    public class GroupName : ValueObjectBase<string>
    {
        public GroupName(string groupName) : base(groupName)
        { }

        protected override bool IsValid(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return false;
            }

            return true;
        }
    }
}
