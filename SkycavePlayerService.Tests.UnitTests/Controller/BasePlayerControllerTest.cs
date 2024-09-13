using Microsoft.Extensions.Logging;
using Moq;
using SkycavePlayerService.api.Controllers;
using SkycavePlayerService.Shared.Contracts;
using SkycavePlayerService.Tests.TestUtilities;

namespace SkycavePlayerService.Tests.UnitTests.Controller
{
    public abstract class BasePlayerControllerTest : BaseUnitTest<PlayersController>
    {
        protected PlayersController Controller;
        protected Mock<IPlayerRepository> MockPlayerRepository;
        protected PlayerRecordGenerator PlayerRecordGenerator;
        protected Mock<ILogger<PlayersController>> MockLogger;

        public BasePlayerControllerTest()
        {
            MockPlayerRepository = new Mock<IPlayerRepository>();
            PlayerRecordGenerator = new PlayerRecordGenerator();
            MockLogger = new Mock<ILogger<PlayersController>>();
            Controller = new PlayersController(MockPlayerRepository.Object, MockLogger.Object);
        }
    }
}
