using Commander;
using System;

namespace Sample.Implementation
{
    [Handles(HumanCommand.Walk)]
    public class WalkHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            Console.WriteLine($"{contract.Name} is Walking");
        }
    }
}
