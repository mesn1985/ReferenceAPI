namespace SkycavePlayerService.api.Configuration.Extensions
{
    /// <summary>
    /// An extention method class container for all Extension methods related to  IConfigurationBuilder
    /// @author: team india
    /// </summary>
    internal static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Loads the configuration file defined by the cli argument Configuration:File=<Configuration file name>
        /// </summary>
        /// <param name="configurationBuilder">Return an instances of original ConfigurationBuilder</param>
        /// <param name="builder">The web application builder, used to build the api</param>
        /// <param name="contentRootPath">The Root path of the application</param>
        /// <returns></returns>
        internal static IConfigurationBuilder AddConfigurationFileByNameProvidedFromCommandLineArguments(this IConfigurationBuilder configurationBuilder, WebApplicationBuilder builder, string contentRootPath)
        {
            string configurationFileName = builder.Configuration["Configuration:File"];

            string filePath = $"{contentRootPath}/ConfigurationFiles/{configurationFileName}";

            configurationBuilder
                .AddJsonFile(
                                filePath ,
                        false,
                                reloadOnChange: false
                        );
            return configurationBuilder;
        }
    }
}
