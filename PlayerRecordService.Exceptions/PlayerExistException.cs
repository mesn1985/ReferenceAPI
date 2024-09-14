namespace PlayerRecordService.Exceptions
{
    public class PlayerExistException : ArgumentException
    {
        public PlayerExistException()
        { }
        public PlayerExistException(string message) : base(message)
        { }
    }
}
