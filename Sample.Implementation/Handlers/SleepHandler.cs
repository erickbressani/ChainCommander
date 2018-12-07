using Commander;
using System;

namespace Sample.Implementation
{
    [Handles(HumanCommand.Sleep)]
    public class SleepHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            Console.WriteLine($"{contract.Name} is Sleeping");
        }
    }
}
