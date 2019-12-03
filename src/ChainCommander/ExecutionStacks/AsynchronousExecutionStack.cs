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
        private readonly List<IAsynchronousCommandHandler<TCommandType, TSubject>> _commandHandlers;
        private readonly List<TCommandType> _commands;

        internal AsynchronousExecutionStack(IEnumerable<TSubject> subjects)
        {
            _subjects = subjects;
            _commandHandlers = new List<IAsynchronousCommandHandler<TCommandType, TSubject>>();
            _commands = new List<TCommandType>();
        }

        public void Add(IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> commandHandlers, TCommandType command)
        {
            _commandHandlers.AddRange(commandHandlers);
            _commands.Add(command);
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
            => _commandHandlers.DoAsync(_subjects, cancellationToken);

        public Task ExecuteInOrderAsync(CancellationToken cancellationToken)
            => _commandHandlers.DoInOrderAsync(_subjects, cancellationToken);

        public Task UndoAllAsync(CancellationToken cancellationToken = default)
            => _commandHandlers.UndoAsync(_subjects, cancellationToken);

        public Task UndoAllInOrderAsync(CancellationToken cancellationToken = default)
            => _commandHandlers.UndoInOrderAsync(_subjects, cancellationToken);

        public Task UndoLastAsync(int howMany = 1, CancellationToken cancellationToken = default)
            => OperateLastAsync(OperationType.Undo, howMany, cancellationToken);

        public Task UndoLastInOrderAsync(int howMany, CancellationToken cancellationToken = default)
            => OperateLastInOrderAsync(OperationType.Undo, howMany, cancellationToken);

        public Task UndoAsync(TCommandType command, CancellationToken cancellationToken = default)
        {
            return _commandHandlers
                .GetBy(command)
                .UndoAsync(_subjects, cancellationToken);
        }

        public Task RedoAllAsync(CancellationToken cancellationToken = default)
            => _commandHandlers.DoAsync(_subjects, cancellationToken);

        public Task RedoAllInOrderAsync(CancellationToken cancellationToken = default)
            => _commandHandlers.DoInOrderAsync(_subjects, cancellationToken);

        public Task RedoLastAsync(int howMany = 1, CancellationToken cancellationToken = default)
            => OperateLastAsync(OperationType.Redo, howMany, cancellationToken);

        public Task RedoLastInOrderAsync(int howMany, CancellationToken cancellationToken = default)
            => OperateLastInOrderAsync(OperationType.Redo, howMany, cancellationToken);

        public Task RedoAsync(TCommandType command, CancellationToken cancellationToken = default)
        {
            return _commandHandlers
                .GetBy(command)
                .DoAsync(_subjects, cancellationToken);
        }

        private Task OperateLastAsync(OperationType operationType, int howMany, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            int handlersCount = _commandHandlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (int iteration = 0; iteration < howMany; iteration++)
            {
                var handler = _commandHandlers[handlersCount - iteration - 1];

                if (operationType == OperationType.Undo)
                    tasks.Add(handler.UndoAsync(_subjects, cancellationToken));
                else
                    tasks.Add(handler.DoAsync(_subjects, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }

        private async Task OperateLastInOrderAsync(OperationType operationType, int howMany, CancellationToken cancellationToken)
        {
            int handlersCount = _commandHandlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (int iteration = 0; iteration < howMany; iteration++)
            {
                var handler = _commandHandlers[handlersCount - iteration - 1];

                if (operationType == OperationType.Undo)
                    await handler.UndoAsync(_subjects, cancellationToken).ConfigureAwait(false);
                else
                    await handler.DoAsync(_subjects, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
