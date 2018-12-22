using System;

namespace ChainCommander
{
    public interface ICommandHandler<TCommandType, TContract> where TCommandType : Enum
    {
        void Handle(TContract contract);
    }
}
