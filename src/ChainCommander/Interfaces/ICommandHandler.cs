using System;

namespace ChainCommander
{
    public interface ICommandHandler<TCommandType, in TSubject> where TCommandType : Enum
    {
        void Handle(TSubject subject);
    }
}
