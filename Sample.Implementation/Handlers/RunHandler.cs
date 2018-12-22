using ChainCommander;
using System;

namespace Sample.Implementation
{
    [Handles(HumanCommand.Run)]
    public class RunHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            contract.IsRunning = true;
            Console.WriteLine($"{contract.Name} is Running");
        }
    }
}
