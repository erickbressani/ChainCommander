using System;
using System.Collections.Generic;

namespace ChainCommander
{
    public interface ICommandExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        IReadOnlyList<TCommandType> CommandStack { get; }

        void UndoAll();

        void UndoLast(int howMany = 1);

        void Undo(TCommandType command);

        void RedoAll();

        void RedoLast(int howMany = 1);

        void Redo(TCommandType command);
    }
}
