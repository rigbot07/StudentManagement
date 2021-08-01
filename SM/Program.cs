using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Find the service layer within our scope.
            using (var scope = host.Services.CreateScope())
            {
                // Get the instance of DataContext in our services layer
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<Data.DataContext>();

                // Call the DataGenerator to create sample data (student & grades & user)
                DataGenerator.Initialize(services);
            }

            // Continue to run the application
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        
    }
}
