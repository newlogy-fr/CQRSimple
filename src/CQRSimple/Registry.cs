using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CQRSimple
{
    public static class Registry
    {
        public static IServiceCollection AddCQRSimple(this IServiceCollection services)
        {
            return services.AddCQRSimple(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static IServiceCollection AddCQRSimple(this IServiceCollection services, Assembly assembly)
        {
            return services.AddCQRSimple(new[] { assembly });
        }

        public static IServiceCollection AddCQRSimple(this IServiceCollection services, Assembly[] assemblies)
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
