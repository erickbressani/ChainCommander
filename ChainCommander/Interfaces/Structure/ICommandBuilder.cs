using System;

namespace ChainCommander.Structure
{
    public interface ICommandBuilder<TCommandType, TContract> where TCommandType : Enum
    {
        INextCommandBuilder<TCommandType, TContract> Do(TCommandType step);
    }
}