using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander
{
    public interface IAsynchronousExecutionStack<TCommandType, TSubject> where TCommandType : Enum
    {
        IReadOnlyList<TCommandType> Commands { get; }

        Task UndoAllAsync(CancellationToken cancellationToken = default);

        Task UndoAllInOrderAsync(CancellationToken cancellationToken = default);

        Task UndoLastAsync(int howMany = 1, CancellationToken cancellationToken = default);

        Task UndoLastInOrderAsync(int howMany, CancellationToken cancellationToken = default);

        Task UndoAsync(TCommandType command, CancellationToken cancellationToken = default);

        Task RedoAllAsync(CancellationToken cancellationToken = default);

        Task RedoAllInOrderAsync(CancellationToken cancellationToken = default);

        Task RedoLastAsync(int howMany = 1, CancellationToken cancellationToken = default);

        Task RedoLastInOrderAsync(int howMany, CancellationToken cancellationToken = default);

        Task RedoAsync(TCommandType command, CancellationToken cancellationToken = default);
    }
}
