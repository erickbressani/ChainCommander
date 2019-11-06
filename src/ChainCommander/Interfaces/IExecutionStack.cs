using System;
using System.Collections.Generic;

namespace ChainCommander
{
    public interface IExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        IReadOnlyList<TCommandType> Commands { get; }

        void UndoAll();

        void UndoLast(int howMany = 1);

        void Undo(TCommandType command);

        void RedoAll();

        void RedoLast(int howMany = 1);

        void Redo(TCommandType command);
    }
}
