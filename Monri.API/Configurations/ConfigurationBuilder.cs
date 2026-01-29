using Monri.API.Middleware;
using Monri.API.Models;
using Monri.API.Services;
using Monri.Core.Services;
using Monri.Data.Repositories;
using Monri.Data.Settings;
using Serilog;
using System.Text.Json.Serialization;

namespace Monri.API.Configurations
{
    public static class ConfigurationBuilder
    {
        public static void ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            builder.Configuration
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            builder.Services.Configure<AppSettings>(
                builder.Configuration.GetSection("AppSettings"));
            builder.Services.AddHttpClient();

            ConfigureLogger(builder);
            ConfigureServices(builder.Services);
            ConfigureRepositories(builder.Services, builder.Configuration);

        }

        public static void ConfigurePipeline(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            app.UseMiddleware<ErrorMiddleware>();
            app.UseAuthorization();
            app.MapControllers();
        }


        #region Private
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IHttpClientProviderService, HttpClientProviderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        private static void ConfigureLogger(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.WithProperty("Application", "API")
                .CreateLogger();
            builder.Host.UseSerilog();
        }
        private static void ConfigureRepositories(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionSettings>(configuration.GetSection("ConnectionSettings"));
            services.AddScoped<IUserRepository, UserRepository>();
        }
        #endregion
    }

}
