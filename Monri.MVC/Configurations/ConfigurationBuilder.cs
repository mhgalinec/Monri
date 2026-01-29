using Monri.Core.Services;
using Monri.MVC.Models;
using Monri.MVC.Services;
using Serilog;

namespace Monri.MVC.Configurations
{
    public static class ConfigurationBuilder
    {
        public static void ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Configuration
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

            builder.Services.Configure<AppSettings>(
                builder.Configuration.GetSection("AppSettings"));

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();

            ConfigureLogger(builder);
            ConfigureServices(builder.Services);
        }

        public static void ConfigurePipeline(this WebApplication app)
        {
            app.UseRouting();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
        }

        #region Private
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IHttpClientProviderService, HttpClientProviderService>();
            services.AddScoped<IUserService, UserService>();
        }

        private static void ConfigureLogger(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.WithProperty("Application", "MVC")
                .CreateLogger();
            builder.Host.UseSerilog();
        }
        #endregion
    }
}
