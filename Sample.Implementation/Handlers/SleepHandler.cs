using ChainCommander;
using System;

namespace Sample.Implementation
{
    [Handles(HumanCommand.Sleep)]
    public class SleepHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            contract.IsSleeping = true;
            Console.WriteLine($"{contract.Name} is Sleeping");
        }
    }
}
