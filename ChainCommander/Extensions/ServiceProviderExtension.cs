using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ChainCommander.Extensions
{
    internal static class ServiceProviderExtension
    {
        internal static IEnumerable<ICommandHandler<TCommandType, TSubject>> GetAllHandlers<TCommandType, TSubject>(this IServiceProvider serviceProvider) where TCommandType : Enum
            => serviceProvider.GetServices<ICommandHandler<TCommandType, TSubject>>();
    }
}
