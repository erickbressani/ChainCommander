using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander
{
    public interface ICommandBuilder<TCommandType, TSubject> where TCommandType : Enum
    {
        ICommandBuilder<TCommandType, TSubject> Do(TCommandType command);

        IExecutionStack<TCommandType, TSubject> Execute();

        Task ExecuteAsync(CancellationToken cancellationToken = default);

        Task ExecuteAsync(out IAsynchronousExecutionStack<TCommandType, TSubject> executionStack, CancellationToken cancellationToken = default);
    }
}