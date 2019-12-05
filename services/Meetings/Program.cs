using System.Threading.Tasks;
using Meetings.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shared.Extensions;

namespace Meetings
{
    public class Program
    {
        public static async Task Main(string[] args) =>
            await CreateHostBuilder(args).Build()
                .InitializeDatabase<MeetingsContext>()
                .RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => config
                    .AddEnvironmentVariables())
                .ConfigureWebHostDefaults(builder => builder
                    .UseStartup<Startup>());
    }
}
