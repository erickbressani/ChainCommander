using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainCommander.Extensions
{
    internal static class CommandHandlerExtension
    {
        internal static IEnumerable<ICommandHandler<TCommandType, TSubject>> GetBy<TCommandType, TSubject>(this IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers, TCommandType type) where TCommandType : Enum
        {
            foreach (ICommandHandler<TCommandType, TSubject> handler in handlers)
            {
                string handlerChainType = handler.GetCommandName();

                if (handlerChainType.Equals(type.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    yield return handler;
            }
        }

        internal static void Execute<TCommandType, TSubject>(this IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers, IEnumerable<TSubject> subjects) where TCommandType : Enum
        {
            foreach (TSubject subject in subjects)
                handlers.Execute(subject);
        }

        internal static void Execute<TCommandType, TSubject>(this IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers, TSubject subject) where TCommandType : Enum
        {
            foreach (ICommandHandler<TCommandType, TSubject> handler in handlers)
                handler.Handle(subject);
        }

        private static string GetCommandName<TCommandType, TSubject>(this ICommandHandler<TCommandType, TSubject> handler) where TCommandType : Enum
        {
            var handles = handler.GetType()
                                 .GetCustomAttributes(typeof(HandlesAttribute), true)
                                 .FirstOrDefault() as HandlesAttribute;

            return handles?.CommandName ?? string.Empty;
        }
    }
}
