using Dualcomp.Auth.Application.Abstractions.Messaging;
using System.Reflection;

namespace Dualcomp.Auth.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ICommandHandler<>).Assembly;

            // Auto-register command handlers with result (exclude generic types)
            var commandWithResultHandlerTypes = applicationAssembly.GetTypes()
                .Where(t => !t.IsGenericTypeDefinition && !t.IsAbstract &&
                           t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .ToList();

            foreach (var handlerType in commandWithResultHandlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));
                
                services.AddScoped(interfaceType, handlerType);
            }

            // Auto-register command handlers without result (exclude generic types)
            var commandWithoutResultHandlerTypes = applicationAssembly.GetTypes()
                .Where(t => !t.IsGenericTypeDefinition && !t.IsAbstract &&
                           t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                .ToList();

            foreach (var handlerType in commandWithoutResultHandlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
                
                services.AddScoped(interfaceType, handlerType);
            }

            // Auto-register query handlers (exclude generic types)
            var queryHandlerTypes = applicationAssembly.GetTypes()
                .Where(t => !t.IsGenericTypeDefinition && !t.IsAbstract &&
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

