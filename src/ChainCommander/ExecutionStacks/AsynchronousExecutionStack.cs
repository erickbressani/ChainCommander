using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander
{
    internal class AsynchronousExecutionStack<TCommandType, TSubject> : IAsynchronousExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        public IReadOnlyList<TCommandType> Commands
            => _commands;

        private readonly IEnumerable<TSubject> _subjects;
        private readonly AsynchronousCommandHandlersWrapper<TCommandType, TSubject> _handlers;
        private readonly List<TCommandType> _commands;

        internal AsynchronousExecutionStack(IEnumerable<TSubject> subjects, AsynchronousCommandHandlersWrapper<TCommandType, TSubject> handlers)
        {
            _subjects = subjects;
            _handlers = handlers;
            _commands = new List<TCommandType>();
        }

        internal void Add(TCommandType command)
            => _commands.Add(command);

        public Task ExecuteAsync(CancellationToken cancellationToken)
             => _handlers.HandleAsync(_commands, _subjects, cancellationToken);

        public Task UndoAllAsync(CancellationToken cancellationToken = default)
            => _handlers.UndoAsync(_commands, _subjects, cancellationToken);

        public Task UndoLastAsync(int howMany = 1, CancellationToken cancellationToken = default)
            => OperateLastAsync(howMany, OperationType.Undo, cancellationToken);

        public Task UndoAsync(TCommandType command, CancellationToken cancellationToken = default)
            => _handlers.UndoAsync(command, _subjects, cancellationToken);

        public Task RedoAllAsync(CancellationToken cancellationToken = default)
            => ExecuteAsync(cancellationToken);

        public Task RedoLastAsync(int howMany = 1, CancellationToken cancellationToken = default)
            => OperateLastAsync(howMany, OperationType.Redo, cancellationToken);

        public Task RedoAsync(TCommandType command, CancellationToken cancellationToken = default)
            => _handlers.HandleAsync(command, _subjects, cancellationToken);

        private Task OperateLastAsync(int howMany, OperationType operationType, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var command in _commands.TakeLast(howMany))
            {
                if (operationType == OperationType.Undo)
                    tasks.Add(_handlers.UndoAsync(command, _subjects, cancellationToken));
                else
                    tasks.Add(_handlers.HandleAsync(command, _subjects, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }
    }
}