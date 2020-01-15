using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainCommander
{
    internal static class CommandHandlerExtensions
    {
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
            foreach (var subject in subjects)
            {
                foreach (var handler in handlers)
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
    }
}