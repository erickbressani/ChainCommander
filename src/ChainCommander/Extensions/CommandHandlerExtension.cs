using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainCommander.Extensions
{
    internal static class CommandHandlerExtension
    {
        internal static IEnumerable<ICommandHandler<TCommandType, TSubject>> GetBy<TCommandType, TSubject>(
            this IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers,
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

        internal static void Do<TCommandType, TSubject>(
            this IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers,
            IEnumerable<TSubject> subjects)
            where TCommandType : Enum
        {
            foreach (var subject in subjects)
                handlers.Do(subject);
        }

        internal static void Do<TCommandType, TSubject>(
            this IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers,
            TSubject subject)
            where TCommandType : Enum
        {
            foreach (var handler in handlers)
                handler.Handle(subject);
        }

        internal static void Do<TCommandType, TSubject>(
            this ICommandHandler<TCommandType, TSubject> handler,
            IEnumerable<TSubject> subjects)
            where TCommandType : Enum
        {
            foreach (var subject in subjects)
                handler.Handle(subject);
        }

        internal static void Undo<TCommandType, TSubject>(
            this IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers,
            IEnumerable<TSubject> subjects)
            where TCommandType : Enum
        {
            foreach (var handler in handlers)
            {
                foreach (var subject in subjects)
                    handler.Undo(subject);
            }
        }

        internal static void Undo<TCommandType, TSubject>(
            this ICommandHandler<TCommandType, TSubject> handler,
            IEnumerable<TSubject> subjects)
            where TCommandType : Enum
        {
            foreach (var subject in subjects)
                handler.Undo(subject);
        }

        private static string GetCommandName<TCommandType, TSubject>(
            this ICommandHandler<TCommandType, TSubject> handler)
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
