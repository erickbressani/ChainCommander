using System;
using System.Collections.Generic;

namespace ChainCommander.Structure
{
    public interface INextCommandBuilder<TCommandType, TContract> where TCommandType : Enum
    {
        INextCommandBuilder<TCommandType, TContract> ThenDo(TCommandType step);
        ICommandBuilder<TCommandType, TContract> ThenUsing(IEnumerable<TContract> newContracts);
        ICommandBuilder<TCommandType, TContract> ThenUsing(params TContract[] newContracts);
    }
}