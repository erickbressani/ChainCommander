using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ChainCommander.Extensions
{
    internal static class ServiceProviderExtension
    {
        internal static IEnumerable<ICommandHandler<TCommandType, TSubject>> GetAllHandlers<TCommandType, TSubject>(this IServiceProvider serviceProvider) where TCommandType : Enum
            => serviceProvider.GetServices<ICommandHandler<TCommandType, TSubject>>();

        internal static IEnumerable<ICommandHandler<TCommandType, TSubject>> GetHandlers<TCommandType, TSubject>(this IServiceProvider serviceProvider) where TCommandType : Enum
            => serviceProvider.GetServices<ICommandHandler<TCommandType, TSubject>>();
    }
}
