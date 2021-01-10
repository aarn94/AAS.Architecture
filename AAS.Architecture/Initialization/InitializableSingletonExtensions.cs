using Convey;
using Microsoft.Extensions.DependencyInjection;

namespace AAS.Architecture.Initialization
{
    public static class InitializableSingletonExtensions
    {
        public static IConveyBuilder AddInitializableSingleton<TInitializer, TInterfaceInitializer>(this IConveyBuilder builder)
            where TInitializer : class, TInterfaceInitializer, ISingletonInitializer
            where TInterfaceInitializer: class 
        {
            builder.Services.AddSingleton<TInitializer>();
            builder.Services.AddSingleton<TInterfaceInitializer>(sp => sp.GetRequiredService<TInitializer>());
            builder.Services.AddSingleton<ISingletonInitializer>(sp => sp.GetRequiredService<TInitializer>());

            return builder;
        }
        
        public static IConveyBuilder AddAsyncInitializableSingleton<TInitializer, TInterfaceInitializer>(this IConveyBuilder builder)
            where TInitializer : class, TInterfaceInitializer, ISingletonInitializerAsync
            where TInterfaceInitializer: class 
        {
            builder.Services.AddSingleton<TInitializer>();
            builder.Services.AddSingleton<TInterfaceInitializer>(sp => sp.GetRequiredService<TInitializer>());
            builder.Services.AddSingleton<ISingletonInitializerAsync>(sp => sp.GetRequiredService<TInitializer>());

            return builder;
        }
    }
}