using Moq;
using Microsoft.Extensions.Logging;
using SkycavePlayerService.Tests.TestUtilities;

namespace SkycavePlayerService.Tests.UnitTests
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
