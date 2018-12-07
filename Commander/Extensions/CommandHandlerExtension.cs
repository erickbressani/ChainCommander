using System;
using System.Collections.Generic;
using System.Linq;

namespace Commander.Extensions
{
    internal static class CommandHandlerExtension
    {
        internal static IEnumerable<ICommandHandler<TCommandType, TContract>> GetBy<TCommandType, TContract>(this IEnumerable<ICommandHandler<TCommandType, TContract>> handlers, TCommandType type) where TCommandType : Enum
        {
            foreach (ICommandHandler<TCommandType, TContract> handler in handlers)
            {
                string handlerChainType = handler.GetCommandName();

                if (!string.IsNullOrEmpty(handlerChainType) &&
                    handlerChainType.Equals(type.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return handler;
                }
            }
        }

        internal static void Execute<TCommandType, TContract>(this IEnumerable<ICommandHandler<TCommandType, TContract>> handlers, IEnumerable<TContract> contracts) where TCommandType : Enum
        {
            foreach (TContract contract in contracts)
                handlers.Execute(contract);
        }

        internal static void Execute<TCommandType, TContract>(this IEnumerable<ICommandHandler<TCommandType, TContract>> handlers, TContract contract) where TCommandType : Enum
        {
            foreach (ICommandHandler<TCommandType, TContract> handler in handlers)
                handler.Handle(contract);
        }

        private static string GetCommandName<TCommandType, TContract>(this ICommandHandler<TCommandType, TContract> handler) where TCommandType : Enum
        {
            var fromChain = handler.GetType()
                                   .GetCustomAttributes(typeof(Handles), true)
                                   .FirstOrDefault() as Handles;

            return fromChain?.CommandName;
        }
    }
}
