﻿namespace DeeDee.Builders
{
    internal static class IocExtensionsBuilder
    {
        public static string Build(string ns)
        {
            var usings = $@"
            using System;
            using System.Linq;
            using System.Reflection;
            using DeeDee.Models;
            using {ns}DeeDee.Generated.Models;
            using Microsoft.Extensions.DependencyInjection;
            using ServiceProvider = DeeDee.Models.ServiceProvider;
            #nullable enable
            namespace {ns}DeeDee.Generated
";


            return usings + @"
            {
                internal static class IocExtensions
                {
                    public static IServiceCollection AddDispatcher(this IServiceCollection services, Lifetime lifetime = Lifetime.Singleton)
                    {
                        switch(lifetime)
                        {
                            case Lifetime.Singleton:
                                services.AddSingleton<IDispatcher, Dispatcher>();
                                services.AddSingleton<ServiceProvider>(ctx => ctx.GetRequiredService);
                                break;

                            case Lifetime.Scoped:
                                services.AddScoped<IDispatcher, Dispatcher>();
                                services.AddScoped<ServiceProvider>(ctx => ctx.GetRequiredService);
                                break;

                            case Lifetime.Transient:
                                services.AddTransient<IDispatcher, Dispatcher>();
                                services.AddTransient<ServiceProvider>(ctx => ctx.GetRequiredService);
                                break;
                        }
                        
                        RegisterPipelineActions(services);
                        
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
                                var bindAs = type.GetCustomAttribute<BindAsAttribute>();
                                switch (bindAs?.Lifetime)
                                {
                                    case Lifetime.Singleton:
                                        services.AddSingleton(implementedInterface, type);
                                        break;
                                    case Lifetime.Scoped:
                                        services.AddScoped(implementedInterface, type);
                                        break;  
                                    case Lifetime.Transient:
                                        services.AddTransient(implementedInterface, type);
                                        break;
                                    default:
                                        services.AddSingleton(implementedInterface, type);
                                        break;
                                }
                                
                            }
                        } 
                    }
                }
            }";
        }
    }
}
