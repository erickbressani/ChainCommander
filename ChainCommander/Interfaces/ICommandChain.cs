using ChainCommander.Structure;
using System;

namespace ChainCommander
{
    public interface ICommandChain
    {
        IContractBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum;
    }
}