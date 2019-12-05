using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;

namespace Shared.Extensions
{
    public static class HostExtensions
    {
        public static IHost InitializeDatabase<TContext>(this IHost host) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetService<TContext>();
            var logger = host.Services.GetService<ILogger<IHost>>();

            Policy
                .Handle<NpgsqlException>()
                .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(attempt * 10),
                    (ex, ts) => logger.LogCritical(ex, $"Unable to migrate database, retrying in {ts}"))
                .Execute(() => db.Database.Migrate());

            return host;
        }
    }
}
