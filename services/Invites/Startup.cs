using System;
using Invites.Data;
using Invites.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Grpc.Meetings;
using Shared.Grpc.Users;

namespace Invites
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // enabling non-tls grpc via http/2
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.AddDbContext<InvitesContext>(options =>
                options.UseNpgsql(config.GetValue<string>("Database")));

            services.AddGrpcClient<UsersService.UsersServiceClient>(options =>
                options.Address = new Uri("http://localhost:7100"));
            services.AddGrpcClient<MeetingsService.MeetingsServiceClient>(options =>
                options.Address = new Uri("http://localhost:7200"));

            services.AddGrpc(options => options.EnableDetailedErrors = true);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
                endpoints.MapGrpcService<InvitesEndpoint>());
        }
    }
}
