using System;

namespace ChainCommander
{
    public interface ICommandBuilder<TCommandType, TSubject> where TCommandType : Enum
    {
        ICommandBuilder<TCommandType, TSubject> Do(TCommandType command);

        ICommandExecutionStack<TCommandType, TSubject> Execute();
    }
}