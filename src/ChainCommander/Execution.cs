using System;
using System.Collections.Generic;
using ChainCommander.Extensions;

namespace ChainCommander
{
    internal class Execution<TCommandType, TSubject> : ICommandExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        public IReadOnlyList<TCommandType> CommandStack { get; }

        private readonly IEnumerable<TSubject> _subjects;
        private readonly List<ICommandHandler<TCommandType, TSubject>> _commandHandlers;

        internal Execution(IEnumerable<TSubject> subjects)
        {
            _subjects = subjects;
            _commandHandlers = new List<ICommandHandler<TCommandType, TSubject>>();
            CommandStack = new List<TCommandType>();
        }

        public void Add(IEnumerable<ICommandHandler<TCommandType, TSubject>> commandHandlers, TCommandType command)
        {
            _commandHandlers.AddRange(commandHandlers);
            (CommandStack as List<TCommandType>)?.Add(command);
        }

        public void Execute()
            => _commandHandlers.Do(_subjects);

        public void UndoAll()
            => _commandHandlers.Undo(_subjects);

        public void UndoLast(int howMany = 1)
        {
            int handlersCount = _commandHandlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (; howMany > 0; howMany--)
            {
                var handler = _commandHandlers.ToArray()[howMany - 1];
                handler.Undo(_subjects);
            }
        }

        public void Undo(TCommandType command)
        {
            _commandHandlers
                .GetBy(command)
                .Undo(_subjects);
        }

        public void RedoAll()
            => Execute();

        public void RedoLast(int howMany = 1)
        {
            int handlersCount = _commandHandlers.Count;

            if (howMany > handlersCount)
                howMany = handlersCount;

            for (; howMany > 0; howMany--)
            {
                var handler = _commandHandlers.ToArray()[howMany - 1];
                handler.Do(_subjects);
            }
        }

        public void Redo(TCommandType command)
        {
            _commandHandlers
                .GetBy(command)
                .Do(_subjects);
        }
    }
}
