using System;

namespace Commander
{
    public interface ICommandHandler<TCommandType, TContract> where TCommandType : Enum
    {
        void Handle(TContract contract);
    }
}
