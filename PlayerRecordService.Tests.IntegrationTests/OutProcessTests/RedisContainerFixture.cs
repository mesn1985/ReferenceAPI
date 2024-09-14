using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Configuration;
using Testcontainers.Redis;

namespace PlayerRecordService.Tests.IntegrationTests.OutProcessTests
{
    public class RedisContainerFixture : IAsyncLifetime
    {
        private static int publicPortExposedByRedisContainer = 6379;
        private IContainer redisContainer = new RedisBuilder()
            .WithImage("redis:7.2.1-alpine")
            .WithExposedPort(publicPortExposedByRedisContainer)
            .Build();

        public Task InitializeAsync()
        {
            return redisContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return redisContainer.DisposeAsync().AsTask();
        }

        public IConfiguration GetConnectionConfigurationForRedisContainer()
        {
            var configurations = new Dictionary<string, string>()
            {
                {"ConnectionStrings:Redis", $"{redisContainer.Hostname}:{redisContainer.GetMappedPublicPort(publicPortExposedByRedisContainer)}"}
            };

            return new ConfigurationBuilder().AddInMemoryCollection(configurations).Build();
        }
    }
}
