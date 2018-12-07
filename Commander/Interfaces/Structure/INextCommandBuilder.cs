using System;

namespace Commander.Structure
{
    public interface INextCommandBuilder<TCommandType, TContract> where TCommandType : Enum
    {
        INextCommandBuilder<TCommandType, TContract> ThenDo(TCommandType step);
        ICommandBuilder<TCommandType, TContract> ThenUsing(TContract step);
    }
}