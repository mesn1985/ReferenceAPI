using StackExchange.Redis;

namespace SkycavePlayerService.Implementations.Storage
{
    internal class RedisDatabaseProvider
    {
        private static IConnectionMultiplexer connectionMultiplexer;

        public RedisDatabaseProvider(string redisDatabaseConnectionString)
        {
            if (connectionMultiplexer == null)
            {
                ConfigurationOptions configuration = new ConfigurationOptions
                {
                    EndPoints = {redisDatabaseConnectionString},
                    ConnectTimeout = 10000,
                    AbortOnConnectFail = false,
                };
                connectionMultiplexer = ConnectionMultiplexer.Connect(configuration);
            }
            
        }
        public IDatabase GetDatabase()
        {
            return connectionMultiplexer.GetDatabase();
        }


    }
}
