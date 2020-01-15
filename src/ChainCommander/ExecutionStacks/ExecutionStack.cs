using System;
using System.Collections.Generic;

namespace ChainCommander
{
    internal class ExecutionStack<TCommandType, TSubject> : IExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        public IReadOnlyList<TCommandType> Commands
            => _commands;

        private readonly IEnumerable<TSubject> _subjects;
        private readonly List<ICommandHandler<TCommandType, TSubject>> _handlers;
        private readonly List<TCommandType> _commands;

        internal ExecutionStack(IEnumerable<TSubject> subjects)
        {
            _subjects = subjects;
            _handlers = new List<ICommandHandler<TCommandType, TSubject>>();
            _commands = new List<TCommandType>();
        }

        public void Add(IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers, TCommandType command)
        {
            _handlers.AddRange(handlers);
            _commands.Add(command);
        }

        public void Execute()
            => _handlers.Do(_subjects);

        public void UndoAll()
            => _handlers.Undo(_subjects);

        public void UndoLast(int howMany = 1)
            => OperateLast(OperationType.Undo, howMany);

        public void Undo(TCommandType command)
        {
            _handlers
                .GetBy(command)
                .Undo(_subjects);
        }

        public void RedoAll()
            => Execute();

        public void RedoLast(int howMany = 1)
            => OperateLast(OperationType.Redo, howMany);

        public void Redo(TCommandType command)
        {
            _handlers
                .GetBy(command)
                .Do(_subjects);
        }

        private void OperateLast(OperationType operationType, int howMany)
        {
            int handlersCount = _handlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (int  iteration = 0;  iteration < howMany;  iteration++)
            {
                var handler = _handlers[handlersCount - iteration - 1];

                if (operationType == OperationType.Undo)
                    handler.Undo(_subjects);
                else
                    handler.Do(_subjects);
            }
        }
    }
}