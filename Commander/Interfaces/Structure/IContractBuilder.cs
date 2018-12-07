using System;

namespace Commander.Structure
{
    public interface IContractBuilder<TCommandType> where TCommandType : Enum
    {
        ICommandBuilder<TCommandType, TContract> Using<TContract>(params TContract[] contracts);
    }
}