using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SkycavePlayerService.api.Configuration.Extensions;
using SkycavePlayerService.api.Infrastructure;

namespace SkycavePlayerService.api
{
    /// <summary>
    /// The Program class is the composition root of the api service.
    /// All application configuration is handle within this class.
    /// @author: team india
    /// </summary>
    public partial class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            // Obtain configurations from configuration file defined by cli arguments
            var configuration =
                new ConfigurationBuilder()
                    .AddConfigurationFileByNameProvidedFromCommandLineArguments(builder, builder.Environment.ContentRootPath)
                    .Build();
            builder.Configuration.AddConfiguration(configuration);
            // Dynamically configuring Dependency injection based on implementations defined in the Configuration file
            builder.Services
                .InitializeDependencyInjectionImplmentationsBasedOnConfigurationFileImplementations(builder.Configuration);

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // The suppress options that is suppose to disable client errors is not working
                    //options.SuppressMapClientErrors = true;
                    // See:
                    //Problemdetails HTTP standardization RFC 7807, see: https://datatracker.ietf.org/doc/html/rfc7807
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        //This is a tempoary fix. An object is still returned, but without the problem details
                        // See: https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-7.0#problem-details-service
                        return new BadRequestObjectResult(new { });
                    };


                }).AddJsonOptions(options =>
                {
                    //This configurations ensures that Swagger can interpret ENUM into plain text, rather the arrays of integers.
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Skycave player service API",
                    Version = "v1",
                    Description = "Skycave player service API <br />Team India<br />Martin Edwin Schjødt Nielsen<br />Jakob Varring<br /> ",
                    Contact = new OpenApiContact
                    {
                        Name = "Martin Nielsen",
                        Email = "202104924@post.au.dk"
                    }

                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
            builder.Services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.ReportApiVersions = true;
            }).AddApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                }
            );
            builder.Services.AddRouting(
                options => options.LowercaseUrls = true
                );

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                    {
                        var descriptions = app.DescribeApiVersions();

                        // Build a swagger endpoint for each discovered API version
                        foreach (var description in descriptions)
                        {
                            var url = $"/swagger/{description.GroupName}/swagger.json";
                            var name = description.GroupName.ToUpperInvariant();
                            options.SwaggerEndpoint(url, name);
                        }

                    });
            }
            app.MapControllers();

            app.Run();
        }
     
    }
}