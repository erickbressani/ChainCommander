using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChainCommander
{
    public interface IAsynchronousExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        IReadOnlyList<TCommandType> Commands { get; }

        Task UndoAllAsync();

        Task UndoAllInOrderAsync();

        Task UndoLastAsync(int howMany = 1);

        Task UndoLastInOrderAsync(int howMany);

        Task UndoAsync(TCommandType command);

        Task RedoAllAsync();

        Task RedoAllInOrderAsync();

        Task RedoLastAsync(int howMany = 1);

        Task RedoLastInOrderAsync(int howMany);

        Task RedoAsync(TCommandType command);
    }
}
