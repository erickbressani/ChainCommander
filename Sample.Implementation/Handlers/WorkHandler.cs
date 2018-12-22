using ChainCommander;
using System;

namespace Sample.Implementation
{
    [Handles(HumanCommand.Work)]
    public class WorkHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            contract.IsWorking = true;
            Console.WriteLine($"{contract.Name} is Working");
        }
    }
}