using System;

namespace Commander
{
    public interface ICommandHandler<TChain, TContract> where TChain : Enum
    {
        void Handle(TContract contract);
    }
}
