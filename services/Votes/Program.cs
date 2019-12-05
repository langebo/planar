using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shared.Extensions;
using Votes.Data;

namespace Votes
{
    public class Program
    {
        public static async Task Main(string[] args) =>
            await CreateHostBuilder(args).Build()
                .InitializeDatabase<VotesContext>()
                .RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => config
                    .AddEnvironmentVariables())
                .ConfigureWebHostDefaults(builder => builder
                    .UseStartup<Startup>());
    }
}
