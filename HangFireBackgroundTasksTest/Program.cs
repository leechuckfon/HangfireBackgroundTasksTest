using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace HangFireBackgroundTasksTest {
    class Program {
        static void Main(string[] args) {
            // Getting configuration from appsettings.json and building it
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot root = builder.Build();

            // Making a webhost so that the dashboard is hosted
            var host = new WebHostBuilder()
                        .UseConfiguration(root)
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseEnvironment("Development")
                        .UseIISIntegration()
                        .ConfigureServices(services => {
                            services.AddHangfire(configuration => {
                                configuration
                                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                .UseSimpleAssemblyNameTypeSerializer()
                                .UseRecommendedSerializerSettings()
                                .UseMemoryStorage();
                            });
                            services.AddMvc();
                            services.AddHangfireServer();
                        })
                        .UseKestrel()
                        .UseStartup<Startup>()
                        .Build();

            host.Run();

        }

       
    }
}
