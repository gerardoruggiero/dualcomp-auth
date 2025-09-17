using Dualcomp.Auth.Application.Abstractions.Messaging;
using System.Reflection;

namespace Dualcomp.Auth.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var applicationAssembly = typeof(ICommandHandler<,>).Assembly;

            // Auto-register command handlers (exclude generic types)
            var commandHandlerTypes = applicationAssembly.GetTypes()
                .Where(t => !t.IsGenericTypeDefinition && // Exclude generic type definitions
                           t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .ToList();

            foreach (var handlerType in commandHandlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));
                
                services.AddScoped(interfaceType, handlerType);
            }

            // Auto-register query handlers (exclude generic types)
            var queryHandlerTypes = applicationAssembly.GetTypes()
                .Where(t => !t.IsGenericTypeDefinition && // Exclude generic type definitions
                           t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .ToList();

            foreach (var handlerType in queryHandlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));
                
                services.AddScoped(interfaceType, handlerType);
            }

            return services;
        }
    }
}

