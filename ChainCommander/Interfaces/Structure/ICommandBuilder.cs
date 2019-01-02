using System;

namespace ChainCommander.Structure
{
    public interface ICommandBuilder<TCommandType, TSubject> where TCommandType : Enum
    {
        INextCommandBuilder<TCommandType, TSubject> Do(TCommandType step);
    }
}