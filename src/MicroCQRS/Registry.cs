using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MicroCQRS
{
    public static class Registry
    {
        public static IServiceCollection AddMicroCQRS(this IServiceCollection services)
        {
            return services.AddMicroCQRS(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static IServiceCollection AddMicroCQRS(this IServiceCollection services, Assembly assembly)
        {
            return services.AddMicroCQRS(new[] { assembly });
        }

        public static IServiceCollection AddMicroCQRS(this IServiceCollection services, Assembly[] assemblies)
        {
            services.AddGenericTypes(assemblies, typeof(ICommandHandler<>));
            services.AddGenericTypes(assemblies, typeof(IQueryHandler<,>));

            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }

        private static void AddGenericTypes(this IServiceCollection services, Assembly[] assemblies, Type genericType)
        {
            var types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType)
                    .Select(i => new { Interface = i, Implementation = t }))
                .ToList();

            foreach (var type in types)
            {
                services.AddTransient(type.Interface, type.Implementation);
            }
        }
    }
}
