using System;
using System.Threading.Tasks;

namespace ChainCommander
{
    public interface IAsynchronousCommandBuilder<TCommandType, TSubject> where TCommandType : Enum
    {
        IAsynchronousCommandBuilder<TCommandType, TSubject> Do(TCommandType command);

        Task<IAsynchronousExecutionStack<TCommandType, TSubject>> ExecuteAsync();

        Task<IAsynchronousExecutionStack<TCommandType, TSubject>> ExecuteInOrderAsync();
    }
}