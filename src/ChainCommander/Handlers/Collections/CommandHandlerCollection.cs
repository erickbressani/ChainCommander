using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainCommander
{
    internal class CommandHandlerCollection<TCommandType, TSubject> where TCommandType : Enum
    {
        private readonly Dictionary<string, List<ICommandHandler<TCommandType, TSubject>>> _handlersDictionary;

        internal CommandHandlerCollection(IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers)
        {
            _handlersDictionary = new Dictionary<string, List<ICommandHandler<TCommandType, TSubject>>>();

            foreach (var handler in handlers)
            {
                string name = GetCommandName(handler);

                if (_handlersDictionary.TryGetValue(name, out var addedHandlers))
                    addedHandlers.Add(handler);
                else
                    _handlersDictionary.Add(name, new List<ICommandHandler<TCommandType, TSubject>> { handler });
            }
        }

        internal List<ICommandHandler<TCommandType, TSubject>> GetBy(TCommandType command)
            => _handlersDictionary.GetValueOrDefault(command.ToString());

        internal void Do(TCommandType command, IEnumerable<TSubject> subjects)
        {
            foreach (var subject in subjects)
                Do(command, subject);
        }

        internal void Do(TCommandType command, TSubject subject)
        {
            var handlers = GetBy(command);

            foreach (var handler in handlers)
                handler.Handle(subject);
        }

        internal void Undo(TCommandType command, IEnumerable<TSubject> subjects)
        {
            var handlers = GetBy(command);

            foreach (var subject in subjects)
            {
                foreach (var handler in handlers)
                    handler.Undo(subject);
            }
        }

        private string GetCommandName(ICommandHandler<TCommandType, TSubject> handler)
        {
            var handles = handler
                .GetType()
                .GetCustomAttributes(typeof(HandlesAttribute), true)
                .FirstOrDefault() as HandlesAttribute;

            return handles?.CommandName ?? string.Empty;
        }
    }
}
