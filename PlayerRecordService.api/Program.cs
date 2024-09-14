using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using PlayerRecordService.api.Configuration.Extensions;
using PlayerRecordService.api.Infrastructure;
using Serilog;
using Serilog.Context;


namespace PlayerRecordService.api
{
    /// <summary>
    /// The Program class is the composition root of the api service.
    /// @author: Martin Edwin Schjødt Nielsen
    /// </summary>
    /// Is very bloated, and should be refactored
    public partial class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole();

            // Obtain configurations from configuration file defined by cli arguments
            var configuration =
                new ConfigurationBuilder()
                    .AddConfigurationFileByNameProvidedFromCommandLineArguments(builder, builder.Environment.ContentRootPath)
                    .Build();
            builder.Configuration.AddConfiguration(configuration);

            // Log configuration
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext()
            );

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
                    Title = "Player record service API",
                    Version = "v1",
                    Description = "Player record service API <br />Martin Edwin Schj�dt Nielsen<br /> ",
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

            // Configure request pipeline
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

            // Middleware that adds correlation id to request.

            // Add Correlation id to header, if non are existing.
            app.Use(async (context, next) =>
            {
                const string _correlationIdHeader = "X-Correlation-Id";
                const string correlationIdLogPropertyname = "CorrelationId";

                context.Request.Headers.TryGetValue(_correlationIdHeader, out StringValues correlationIds);
                var correlationId = correlationIds.FirstOrDefault() ?? Guid.NewGuid().ToString();

                using (LogContext.PushProperty(correlationIdLogPropertyname, correlationId))
                {
                    context.Items["Correlation-Id"] = correlationId;
                    await next(context);
                }
            });

            app.MapControllers();

            app.Run();
        }
     
    }
}