namespace PlayerRecordService.Exceptions
{
    public class PlayerDoesNotExistException : ArgumentException
    {
        public PlayerDoesNotExistException(string message) :base(message)
        { }
    }
}
