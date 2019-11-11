using System;
using System.Collections.Generic;

namespace ChainCommander
{
    internal class ExecutionStack<TCommandType, TSubject> : IExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        public IReadOnlyList<TCommandType> Commands { get; }

        private readonly IEnumerable<TSubject> _subjects;
        private readonly List<ICommandHandler<TCommandType, TSubject>> _commandHandlers;

        internal ExecutionStack(IEnumerable<TSubject> subjects)
        {
            Commands = new List<TCommandType>();
            _subjects = subjects;
            _commandHandlers = new List<ICommandHandler<TCommandType, TSubject>>();
        }

        public void Add(IEnumerable<ICommandHandler<TCommandType, TSubject>> commandHandlers, TCommandType command)
        {
            _commandHandlers.AddRange(commandHandlers);
            (Commands as List<TCommandType>)?.Add(command);
        }

        public void Execute()
            => _commandHandlers.Do(_subjects);

        public void UndoAll()
            => _commandHandlers.Undo(_subjects);

        public void UndoLast(int howMany = 1)
            => OperateLast(OperationType.Undo, howMany);

        public void Undo(TCommandType command)
        {
            _commandHandlers
                .GetBy(command)
                .Undo(_subjects);
        }

        public void RedoAll()
            => Execute();

        public void RedoLast(int howMany = 1)
            => OperateLast(OperationType.Redo, howMany);

        public void Redo(TCommandType command)
        {
            _commandHandlers
                .GetBy(command)
                .Do(_subjects);
        }

        private void OperateLast(OperationType operationType, int howMany)
        {
            int handlersCount = _commandHandlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (int  iteration = 0;  iteration < howMany;  iteration++)
            {
                var handler = _commandHandlers[handlersCount - iteration - 1];

                if (operationType == OperationType.Undo)
                    handler.Undo(_subjects);
                else
                    handler.Do(_subjects);
            }
        }
    }
}
