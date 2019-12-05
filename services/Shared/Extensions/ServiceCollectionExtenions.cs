using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromEntryAssembly()
                .AddClasses(cls => cls
                    .AssignableTo(typeof(AbstractValidator<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            return services;
        }
    }
}