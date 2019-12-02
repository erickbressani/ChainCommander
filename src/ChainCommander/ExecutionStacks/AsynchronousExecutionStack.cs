using System;
using System.Collections.Generic;
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

        public Task ExecuteAsync()
            => _commandHandlers.DoAsync(_subjects);

        public Task ExecuteInOrderAsync()
            => _commandHandlers.DoInOrderAsync(_subjects);

        public Task UndoAllAsync()
            => _commandHandlers.UndoAsync(_subjects);

        public Task UndoAllInOrderAsync()
            => _commandHandlers.UndoInOrderAsync(_subjects);

        public Task UndoLastAsync(int howMany = 1)
            => OperateLastAsync(OperationType.Undo, howMany);

        public Task UndoLastInOrderAsync(int howMany)
            => OperateLastInOrderAsync(OperationType.Undo, howMany);

        public Task UndoAsync(TCommandType command)
        {
            return _commandHandlers
                .GetBy(command)
                .UndoAsync(_subjects);
        }

        public Task RedoAllAsync()
            => _commandHandlers.DoAsync(_subjects);

        public Task RedoAllInOrderAsync()
            => _commandHandlers.DoInOrderAsync(_subjects);

        public Task RedoLastAsync(int howMany = 1)
            => OperateLastAsync(OperationType.Redo, howMany);

        public Task RedoLastInOrderAsync(int howMany)
            => OperateLastInOrderAsync(OperationType.Redo, howMany);

        public Task RedoAsync(TCommandType command)
        {
            return _commandHandlers
                .GetBy(command)
                .DoAsync(_subjects);
        }

        private Task OperateLastAsync(OperationType operationType, int howMany)
        {
            var tasks = new List<Task>();
            int handlersCount = _commandHandlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (int iteration = 0; iteration < howMany; iteration++)
            {
                var handler = _commandHandlers[handlersCount - iteration - 1];

                if (operationType == OperationType.Undo)
                    tasks.Add(handler.UndoAsync(_subjects));
                else
                    tasks.Add(handler.DoAsync(_subjects));
            }

            return Task.WhenAll(tasks);
        }

        private async Task OperateLastInOrderAsync(OperationType operationType, int howMany)
        {
            int handlersCount = _commandHandlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (int iteration = 0; iteration < howMany; iteration++)
            {
                var handler = _commandHandlers[handlersCount - iteration - 1];

                if (operationType == OperationType.Undo)
                    await handler.UndoAsync(_subjects).ConfigureAwait(false);
                else
                    await handler.DoAsync(_subjects).ConfigureAwait(false);
            }
        }
    }
}
