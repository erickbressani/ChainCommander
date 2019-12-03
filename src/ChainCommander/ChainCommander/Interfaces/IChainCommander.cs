using System;

namespace ChainCommander
{
    public interface IChainCommander
    {
        ISubjectBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum;
    }
}