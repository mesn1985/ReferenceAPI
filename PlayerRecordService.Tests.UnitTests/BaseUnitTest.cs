using PlayerRecordService.Tests.TestUtilities;

namespace PlayerRecordService.Tests.UnitTests
{
    public abstract class BaseUnitTest<T>
    {
        protected PlayerRecordGenerator RecordGenerator;

        public BaseUnitTest()
        {
            
            RecordGenerator = new PlayerRecordGenerator();
        }
    }
}
