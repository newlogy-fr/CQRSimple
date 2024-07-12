using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MicroCQRS
{
    public static class Registry
    {
        public static IServiceCollection AddMicroCQRS(this IServiceCollection services, Assembly assembly)
        {
            services.AddGenericTypes(assembly, typeof(ICommandHandler<>));
            services.AddGenericTypes(assembly, typeof(IQueryHandler<,>));

            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }

        private static void AddGenericTypes(this IServiceCollection services, Assembly assembly, Type genericType)
        {
            var types = assembly.GetTypes()
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
