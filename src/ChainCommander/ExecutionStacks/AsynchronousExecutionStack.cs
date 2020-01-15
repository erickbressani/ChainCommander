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
        private readonly List<IAsynchronousCommandHandler<TCommandType, TSubject>> _handlers;
        private readonly List<TCommandType> _commands;

        internal AsynchronousExecutionStack(IEnumerable<TSubject> subjects)
        {
            _subjects = subjects;
            _handlers = new List<IAsynchronousCommandHandler<TCommandType, TSubject>>();
            _commands = new List<TCommandType>();
        }

        public void Add(IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> commandHandlers, TCommandType command)
        {
            _handlers.AddRange(commandHandlers);
            _commands.Add(command);
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
            => _handlers.DoAsync(_subjects, cancellationToken);

        public Task UndoAllAsync(CancellationToken cancellationToken = default)
            => _handlers.UndoAsync(_subjects, cancellationToken);

        public Task UndoLastAsync(int howMany = 1, CancellationToken cancellationToken = default)
            => OperateLastAsync(OperationType.Undo, howMany, cancellationToken);

        public Task UndoAsync(TCommandType command, CancellationToken cancellationToken = default)
        {
            return _handlers
                .GetBy(command)
                .UndoAsync(_subjects, cancellationToken);
        }

        public Task RedoAllAsync(CancellationToken cancellationToken = default)
            => _handlers.DoAsync(_subjects, cancellationToken);

        public Task RedoLastAsync(int howMany = 1, CancellationToken cancellationToken = default)
            => OperateLastAsync(OperationType.Redo, howMany, cancellationToken);

        public Task RedoAsync(TCommandType command, CancellationToken cancellationToken = default)
        {
            return _handlers
                .GetBy(command)
                .DoAsync(_subjects, cancellationToken);
        }

        private Task OperateLastAsync(OperationType operationType, int howMany, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            int handlersCount = _handlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (int iteration = 0; iteration < howMany; iteration++)
            {
                var handler = _handlers[handlersCount - iteration - 1];

                if (operationType == OperationType.Undo)
                    tasks.Add(handler.UndoAsync(_subjects, cancellationToken));
                else
                    tasks.Add(handler.DoAsync(_subjects, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }
    }
}