using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ChainCommander.Extensions
{
    internal static class ServiceProviderExtension
    {
        internal static IEnumerable<ICommandHandler<TCommandType, TContract>> GetAllHandlers<TCommandType, TContract>(this IServiceProvider serviceProvider) where TCommandType : Enum
            => serviceProvider.GetServices<ICommandHandler<TCommandType, TContract>>();
    }
}
