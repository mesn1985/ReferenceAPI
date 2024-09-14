using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using PlayerRecordService.api;

namespace PlayerRecordService.Tests.IntegrationTests.Infrastructure
{
    public class PlayerServiceWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("Configuration:File", "FakePlayerStorage.json");
        }
    }
}
