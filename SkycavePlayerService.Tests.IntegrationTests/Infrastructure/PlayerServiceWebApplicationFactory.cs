using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SkycavePlayerService.Tests.IntegrationTests.Infrastructure
{
    public class PlayerServiceWebApplicationFactory : WebApplicationFactory<SkycavePlayerService.api.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("Configuration:File", "FakePlayerStorage.json");
        }
    }
}
