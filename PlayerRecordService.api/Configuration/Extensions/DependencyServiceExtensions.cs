using System.Reflection;
using PlayerRecordService.Shared.Contracts;

namespace PlayerRecordService.api.Configuration.Extensions
{
    /// <summary>
    ///  provides extension methods for initializing dependency injection implementations based on configuration settings.
    ///  This class allows for dynamic loading of assemblies and instantiation of types specified in a configuration file.
    /// 
    /// @author: Team india
    /// </summary>
    internal static class DependencyServiceExtensions
    {
        /// <summary>
        /// Configures Dependency injection for  the service base on the "implementations" section of the configuration file.
        /// Be aware the all implementations are loaded with a scoped lifetime, unless the implementation name  ends with the word InMemory,
        ///  Then they will be loaded as a singleton
        /// </summary>
        /// <param name="services">the Applications main .Net Service collection </param>
        /// <param name="configuration">the configurations of the application, containing the  name of the implementations </param>
        /// <returns>The original instance of the IServiceCollection, with DI configuration set</returns>
        internal static IServiceCollection InitializeDependencyInjectionImplmentationsBasedOnConfigurationFileImplementations
            (this IServiceCollection services, IConfiguration configuration)
        {
            LoadReferencedAssemblies();

            Dictionary<string,string> implementations 
                = getAllEntriesFromImplementationSection(configuration);

            AddStorage(services,configuration,implementations);

            services.AddScoped<IPlayerRepository>(provider =>
            {
                Assembly assembly = Assembly.Load(ExtractAssemblyName(implementations["PlayerRepository"]));
                Type type = assembly.GetType(implementations["PlayerRepository"]);
                return ActivatorUtilities.CreateInstance(provider, type) as IPlayerRepository;
            });

            return services;
        }

        private static void AddStorage(IServiceCollection services, IConfiguration configuration, Dictionary<string,string> implementations)
        {
            Assembly assembly = Assembly.Load(ExtractAssemblyName(implementations["PlayerStorage"]));
            Type type = assembly.GetType(implementations["PlayerStorage"]);

            if (type.Name.EndsWith("InMemory"))
            {
                services.AddSingleton<IPlayerStorage>(provider =>
                {
                    return ActivatorUtilities.CreateInstance(provider, type) as IPlayerStorage;
                });
            }
            else
            {
                services.AddScoped<IPlayerStorage>(provider =>
                {
                    return ActivatorUtilities.CreateInstance(provider, type) as IPlayerStorage;
                });
            }
        }

        private static Dictionary<string, string> getAllEntriesFromImplementationSection(IConfiguration configuration)
        {
            return configuration.GetSection("Implementations")
                .GetChildren().ToDictionary(x => x.Key, x => x.Value);
        }

        private static void LoadReferencedAssemblies()
        {
            // Get the directory containing the DLLs of referenced projects
            string dllDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Get all DLL files in the directory
            string[] dllFiles = Directory.GetFiles(dllDirectory, "*.dll");

            // Load each DLL file as an assembly
            foreach (string dllFile in dllFiles)
            {
                Assembly referencedAssembly = Assembly.LoadFrom(dllFile);
            }
        }

        private static string ExtractAssemblyName(string value)
        {
            // Split the value by dots and take all parts except the last one
            string[] parts = value.Split('.');
            return string.Join(".", parts.Take(parts.Length - 1));
        }
    }
}
