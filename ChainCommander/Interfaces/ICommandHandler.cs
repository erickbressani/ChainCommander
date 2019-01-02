using System;

namespace ChainCommander
{
    public interface ICommandHandler<TCommandType, TSubject> where TCommandType : Enum
    {
        void Handle(TSubject subject);
    }
}
