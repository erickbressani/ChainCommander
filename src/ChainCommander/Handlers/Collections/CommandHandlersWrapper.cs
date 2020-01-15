using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainCommander
{
    internal class CommandHandlersWrapper<TCommandType, TSubject> where TCommandType : Enum
    {
        private readonly Dictionary<string, List<ICommandHandler<TCommandType, TSubject>>> _handlersDictionary;

        internal CommandHandlersWrapper()
            => _handlersDictionary = new Dictionary<string, List<ICommandHandler<TCommandType, TSubject>>>();

        internal CommandHandlersWrapper(IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers): this()
            => Add(handlers);

        internal void Add(IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers)
        {
            foreach (var handler in handlers)
            {
                string name = GetCommandName(handler);

                if (_handlersDictionary.TryGetValue(name, out var addedHandlers))
                    addedHandlers.Add(handler);
                else
                    _handlersDictionary.Add(name, new List<ICommandHandler<TCommandType, TSubject>> { handler });
            }
        }

        internal void Handle(IEnumerable<TCommandType> commands, IEnumerable<TSubject> subjects)
        {
            foreach (var subject in subjects)
            {
                foreach (var command in commands)
                    Handle(command, subject);
            }
        }

        internal void Handle(TCommandType command, IEnumerable<TSubject> subjects)
        {
            foreach (var subject in subjects)
                Handle(command, subject);
        }

        internal void Undo(IEnumerable<TCommandType> commands, IEnumerable<TSubject> subjects)
        {
            foreach (var subject in subjects)
            {
                foreach (var command in commands)
                    Undo(command, subject);
            }
        }

        internal void Undo(TCommandType command, IEnumerable<TSubject> subjects)
        {
            foreach (var subject in subjects)
                Undo(command, subject);
        }

        private IEnumerable<ICommandHandler<TCommandType, TSubject>> GetBy(TCommandType command)
           => _handlersDictionary.GetValueOrDefault(command.ToString());

        private string GetCommandName(ICommandHandler<TCommandType, TSubject> handler)
        {
            var handles = handler
                .GetType()
                .GetCustomAttributes(typeof(HandlesAttribute), true)
                .FirstOrDefault() as HandlesAttribute;

            return handles?.CommandName ?? string.Empty;
        }

        private void Handle(TCommandType command, TSubject subject)
        {
            var handlers = GetBy(command);

            foreach (var handler in handlers)
                handler.Handle(subject);
        }

        private void Undo(TCommandType command, TSubject subject)
        {
            var handlers = GetBy(command);

            foreach (var handler in handlers)
                handler.Undo(subject);
        }
    }
}
