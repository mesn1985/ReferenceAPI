# Player record service
  
Player record service is a service that functionalities for persisting, retrieving or
changing existing player records. 


## Api specifications
  
The service provides Open api specifications through swagger. Which means that a `swagger.json` file is
generated upon build.

Currently the api is set to provide the specifications at the path `/swagger/index.html` in development
environments.

For more information on swagger configuration in .net, see (https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-7.0)[https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-7.0]

## General structure of the code base

The code base is divided into several projects, meaning that there is several assemblies. This is to keep a high level of
modularity in the in the project, and a logical seperations of the code.
  
The executable API application is found with the `PlayerRecordService.api` project, and the remainder of projects
are either dependencies, or tests.

## Assembly and namespace convention
The  convention of all projects, are that the name of the namespace, is the same as the name of the assembly

## Dependencies and injection
All dependency injection, is handled by  `PlayerRecordService.api`.
The used implementations for each dependency is specified by a provided configuration
file that must be located in the `/ConfigurationFiles`. An example of such a file is shown below  
  
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Implementations": {
    "PlayerStorage": "PlayerRecordService.Implementations.TestDoubles.FakePlayerStorageInMemory",
    "PlayerRepository": "PlayerRecordService.Implementations.Repositories.PlayerRepository"
  }
}

```
The section `Implementations` defines which implementation there should be used for each dependency.
E.g. `PlayerStorage` will use the implementation found in the fully qualified name `PlayerRecordService.Implementations.TestDoubles.FakePlayerStorageInMemory`.
This allows the dependency injection to dynamically map interfaces to implementations. 
```
services.AddScoped<IPlayerRepository>(provider =>
            {
                Assembly assembly = Assembly.Load(ExtractAssemblyName(implementations["PlayerRepository"]));
                Type type = assembly.GetType(implementations["PlayerRepository"]);
                return ActivatorUtilities.CreateInstance(provider, type) as IPlayerRepository;
            });
```
A `Scoped` lifetime of objects means that the object only lives for a single request. This presents a problem when using test doubles
fakes for persistance that not within the application domain. In the case of PlayerRecordService, an in-memory fake storage is used for in-process integration tests. So there is a 
need to keep the fake storage objects that uses in-memory persistance alive for the entire life time of the application. **Therefor all storage implementations  that ends with `InMemory` will by convention be loaded as a singleton**.
```
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
```

## Starting the service
To start application execution the command `dotnet run Configuration:File=<Configuration file name>` from
the solution root, or from the project root of the `PlayerRecordService.api`.
There are no default configuration file. If the service is started without a proper configuration file defined,
it will throw an exception. This is intended behavior, so a default configurations file are not accidentally used.

### Starting the service using in memory player storage
An example of starting the application with fake storage is `dotnet run Configuration:File=FakePlayerStorage.json`

### Starting the service using local redis instance as storage
An example of starting the application with local hosted redis storage `dotnet run Configuration:File=RedisStorageLocalExecution.json`.

A redis instance should be hosted locally, as  the assumption is that the host ip is 127.0.0.1, and that the used redis port is
6379. You can start a Redis container on docker with the command:  ```docker run -p 6379:6379  redis:7.2.1-alpine```

### Starting the service with docker swarm
An example of starting the application with local hosted redis storage `dotnet run Configuration:File=RedisStorageDockerSwarm.json`.

See the configuration file for redis service dns and port.

## Testing
The code base provides in-process integration tests of the application, unit tests and out-process integration
tests (Connector test).
To Execute All tests, execute `dotnet test` in the solution root folder.

To only execute unit tests or Integration test. Execute `dotnet test` within the respectively project directory. 

