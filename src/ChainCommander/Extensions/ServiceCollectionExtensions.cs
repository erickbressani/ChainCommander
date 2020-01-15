using Microsoft.Extensions.DependencyInjection;

namespace ChainCommander
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChainCommander(this IServiceCollection services)
            => services.AddSingleton<IChainCommander, ChainCommander>();
    }
}