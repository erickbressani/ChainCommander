using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainCommander
{
    internal class CommandHandlersWrapper<TCommandType, TSubject> where TCommandType : Enum
    {
        private readonly Dictionary<string, List<ICommandHandler<TCommandType, TSubject>>> _handlersDictionary;

        private IEnumerable<ICommandHandler<TCommandType, TSubject>> _handlers
        {
            get
            {
                foreach (var handlers in _handlersDictionary.Values)
                {
                    foreach (var handler in handlers)
                        yield return handler;
                }
            }
        }

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

        internal void Do(IEnumerable<TSubject> subjects)
        {
            foreach (var subject in subjects)
                Do(subject);
        }

        internal void Do(TSubject subject)
        {
            foreach (var handler in _handlers)
                handler.Handle(subject);
        }

        internal void Do(TCommandType command, IEnumerable<TSubject> subjects)
        {
            var handlers = GetBy(command);

            foreach (var subject in subjects)
            {
                foreach (var handler in handlers)
                    handler.Handle(subject);
            }
        }

        internal void Undo(IEnumerable<TSubject> subjects)
        {
            foreach (var subject in subjects)
                Undo(subject);
        }

        internal void Undo(TSubject subject)
        {
            foreach (var handler in _handlers)
                handler.Undo(subject);
        }

        internal void Undo(TCommandType command, TSubject subject)
        {
            var handlers = GetBy(command);

            foreach (var handler in handlers)
                handler.Undo(subject);
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

        private List<ICommandHandler<TCommandType, TSubject>> GetBy(TCommandType command)
           => _handlersDictionary.GetValueOrDefault(command.ToString());

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
