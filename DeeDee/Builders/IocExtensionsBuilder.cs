namespace DeeDee.Builders
{
    internal static class IocExtensionsBuilder
    {
        public static string Build()
        {
            return @"
            using System;
            using System.Linq;
            using System.Reflection;
            using DeeDee.Models;
            using Microsoft.Extensions.DependencyInjection;

            namespace DeeDee
            {
                public static class IocExtensions
                {
                    public static IServiceCollection AddDispatcher(this IServiceCollection services)
                    {
                        services.AddSingleton<IDispatcher, Dispatcher>();
                        RegisterPipelineActions(services);
                        services.AddSingleton<DeeDee.Models.ServiceProvider>(ctx => ctx.GetRequiredService);
                        return services;
                    }

                    private static void RegisterPipelineActions(IServiceCollection services)
                    {
                        var pipelineTypes = AppDomain
                            .CurrentDomain
                            .GetAssemblies()
                            .SelectMany
                            (
                                a => a.GetTypes().Where
                                (
                                   x => !x.IsInterface &&
                                        !x.IsAbstract &&
                                        x.GetInterfaces()
                                            .Any
                                            (
                                                y => y.Name.Equals(typeof(IPipelineActionAsync<,>).Name,
                                                         StringComparison.InvariantCulture) ||
                                                     y.Name.Equals(typeof(IPipelineActionAsync<>).Name,
                                                         StringComparison.InvariantCulture) ||
                                                     y.Name.Equals(typeof(IPipelineAction<>).Name,
                                                         StringComparison.InvariantCulture) ||
                                                     y.Name.Equals(typeof(IPipelineAction<,>).Name, StringComparison.InvariantCulture)
                                            )
                                ).GroupBy(type => type.GetInterfaces()[0]).SelectMany(g => g.OrderByDescending(s => s.GetCustomAttribute<StepAttribute>()?.Order))
                            );

                        foreach (var type in pipelineTypes)
                        {
                            foreach (var implementedInterface in type.GetInterfaces())
                            {
                                services.AddSingleton(implementedInterface, type);
                            }
                        }
                    }
                }
            }";
        }
    }
}
