using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander
{
    public interface ICommandBuilder<TCommandType, TSubject> where TCommandType : Enum
    {
        ICommandBuilder<TCommandType, TSubject> Do(TCommandType command);

        IExecutionStack<TCommandType, TSubject> Execute();

        Task<IAsynchronousExecutionStack<TCommandType, TSubject>> ExecuteAsync(CancellationToken cancellationToken = default);

        Task<IAsynchronousExecutionStack<TCommandType, TSubject>> ExecuteInOrderAsync(CancellationToken cancellationToken = default);
    }
}