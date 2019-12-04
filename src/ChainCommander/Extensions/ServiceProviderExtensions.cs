using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ChainCommander
{
    internal static class ServiceProviderExtensions
    {
        internal static IEnumerable<ICommandHandler<TCommandType, TSubject>> GetAllHandlers<TCommandType, TSubject>(this IServiceProvider serviceProvider) where TCommandType : Enum
            => serviceProvider.GetServices<ICommandHandler<TCommandType, TSubject>>();

        internal static IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> GetAllAsynchronousHandlers<TCommandType, TSubject>(this IServiceProvider serviceProvider) where TCommandType : Enum
            => serviceProvider.GetServices<IAsynchronousCommandHandler<TCommandType, TSubject>>();
    }
}