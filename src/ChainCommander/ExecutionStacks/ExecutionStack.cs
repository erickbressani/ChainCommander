using System;
using System.Collections.Generic;

namespace ChainCommander
{
    internal class ExecutionStack<TCommandType, TSubject> : IExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        public IReadOnlyList<TCommandType> Commands
            => _commands;

        private readonly IEnumerable<TSubject> _subjects;
        private readonly CommandHandlersWrapper<TCommandType, TSubject> _handlers;
        private readonly List<TCommandType> _commands;

        internal ExecutionStack(IEnumerable<TSubject> subjects, CommandHandlersWrapper<TCommandType, TSubject> handlers)
        {
            _subjects = subjects;
            _handlers = handlers;
            _commands = new List<TCommandType>();
        }

        public void Add(TCommandType command)
            => _commands.Add(command);

        public void Execute()
            => _handlers.Handle(_commands, _subjects);

        public void UndoAll()
            => _handlers.Undo(_commands, _subjects);

        public void UndoLast(int howMany = 1)
            => OperateLast(OperationType.Undo, howMany);

        public void Undo(TCommandType command)
            => _handlers.Undo(command, _subjects);

        public void RedoAll()
            => Execute();

        public void RedoLast(int howMany = 1)
            => OperateLast(OperationType.Redo, howMany);

        public void Redo(TCommandType command)
            => _handlers.Handle(command, _subjects);

        private void OperateLast(OperationType operationType, int howMany)
        {
            int commandsCount = _commands.Count;

            if (howMany > commandsCount)
                howMany = commandsCount;

            for (int iteration = 0; iteration < howMany; iteration++)
            {
                var command = _commands[commandsCount - iteration - 1];

                if (operationType == OperationType.Undo)
                    _handlers.Undo(command, _subjects);
                else
                    _handlers.Handle(command, _subjects);
            }
        }
    }
}