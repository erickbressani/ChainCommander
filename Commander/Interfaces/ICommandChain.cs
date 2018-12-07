using Commander.Structure;
using System;

namespace Commander
{
    public interface ICommandChain
    {
        IContractBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum;
    }
}