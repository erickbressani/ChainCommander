using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander
{
    internal class AsynchronousCommandHandlersWrapper<TCommandType, TSubject> where TCommandType : Enum
    {
        private readonly Dictionary<string, List<IAsynchronousCommandHandler<TCommandType, TSubject>>> _handlersDictionary;

        internal AsynchronousCommandHandlersWrapper(IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> handlers)
        {
            _handlersDictionary = new Dictionary<string, List<IAsynchronousCommandHandler<TCommandType, TSubject>>>();

            foreach (var handler in handlers)
            {
                string name = GetCommandName(handler);

                if (_handlersDictionary.TryGetValue(name, out var addedHandlers))
                    addedHandlers.Add(handler);
                else
                    _handlersDictionary.Add(name, new List<IAsynchronousCommandHandler<TCommandType, TSubject>> { handler });
            }
        }

        internal Task HandleAsync(IEnumerable<TCommandType> commands, IEnumerable<TSubject> subjects, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var subject in subjects)
            {
                foreach (var command in commands)
                    tasks.AddRange(Handle(command, subject, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }

        internal Task HandleAsync(TCommandType command, IEnumerable<TSubject> subjects, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var subject in subjects)
                tasks.AddRange(Handle(command, subject, cancellationToken));

            return Task.WhenAll(tasks);
        }

        internal Task UndoAsync(IEnumerable<TCommandType> commands, IEnumerable<TSubject> subjects, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var subject in subjects)
            {
                foreach (var command in commands)
                    tasks.AddRange(Undo(command, subject, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }

        internal Task UndoAsync(TCommandType command, IEnumerable<TSubject> subjects, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var subject in subjects)
                tasks.AddRange(Undo(command, subject, cancellationToken));

            return Task.WhenAll(tasks);
        }

        private IEnumerable<Task> Handle(TCommandType command, TSubject subject, CancellationToken cancellationToken)
        {
            foreach (var handler in GetHandlersBy(command))
                yield return handler.HandleAsync(subject, cancellationToken);
        }

        private IEnumerable<Task> Undo(TCommandType command, TSubject subject, CancellationToken cancellationToken)
        {
            foreach (var handler in GetHandlersBy(command))
                yield return handler.UndoAsync(subject, cancellationToken);
        }

        private IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> GetHandlersBy(TCommandType command)
           => _handlersDictionary.GetValueOrDefault(command.ToString());

        private string GetCommandName(IAsynchronousCommandHandler<TCommandType, TSubject> handler)
        {
            var handles = handler
                .GetType()
                .GetCustomAttributes(typeof(HandlesAttribute), true)
                .FirstOrDefault() as HandlesAttribute;

            return handles?.CommandName ?? string.Empty;
        }
    }
}
