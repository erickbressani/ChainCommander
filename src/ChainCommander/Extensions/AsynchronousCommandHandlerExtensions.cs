using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander
{
    internal static class AsynchronousCommandHandlerExtensions
    {
        internal static IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> GetBy<TCommandType, TSubject>(
            this IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> handlers,
            TCommandType type)
            where TCommandType : Enum
        {
            foreach (var handler in handlers)
            {
                string handlerChainType = handler.GetCommandName();

                if (handlerChainType.Equals(type.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    yield return handler;
            }
        }

        internal static Task DoAsync<TCommandType, TSubject>(
            this IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> handlers,
            IEnumerable<TSubject> subjects,
            CancellationToken cancellationToken)
            where TCommandType : Enum
        {
            var tasks = new List<Task>();

            foreach (var subject in subjects)
                tasks.Add(handlers.DoAsync(subject, cancellationToken));

            return Task.WhenAll(tasks);
        }

        internal static Task DoAsync<TCommandType, TSubject>(
           this IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> handlers,
           TSubject subject,
           CancellationToken cancellationToken)
           where TCommandType : Enum
        {
            var tasks = new List<Task>();

            foreach (var handler in handlers)
                tasks.Add(handler.HandleAsync(subject, cancellationToken));

            return Task.WhenAll(tasks);
        }

        internal static Task DoAsync<TCommandType, TSubject>(
            this IAsynchronousCommandHandler<TCommandType, TSubject> handler,
            IEnumerable<TSubject> subjects,
           CancellationToken cancellationToken)
            where TCommandType : Enum
        {
            var tasks = new List<Task>();

            foreach (var subject in subjects)
                tasks.Add(handler.HandleAsync(subject, cancellationToken));

            return Task.WhenAll(tasks);
        }

        internal static Task UndoAsync<TCommandType, TSubject>(
            this IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> handlers,
            IEnumerable<TSubject> subjects,
            CancellationToken cancellationToken)
            where TCommandType : Enum
        {
            var tasks = new List<Task>();

            foreach (var subject in subjects)
            {
                foreach (var handler in handlers)
                    tasks.Add(handler.UndoAsync(subject, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }

        internal static Task UndoAsync<TCommandType, TSubject>(
            this IAsynchronousCommandHandler<TCommandType, TSubject> handler,
            IEnumerable<TSubject> subjects,
            CancellationToken cancellationToken)
            where TCommandType : Enum
        {
            var tasks = new List<Task>();

            foreach (var subject in subjects)
                tasks.Add(handler.UndoAsync(subject, cancellationToken));

            return Task.WhenAll(tasks);
        }

        private static string GetCommandName<TCommandType, TSubject>(
            this IAsynchronousCommandHandler<TCommandType, TSubject> handler)
            where TCommandType : Enum
        {
            var handles = handler
                .GetType()
                .GetCustomAttributes(typeof(HandlesAttribute), true)
                .FirstOrDefault() as HandlesAttribute;

            return handles?.CommandName ?? string.Empty;
        }
    }
}