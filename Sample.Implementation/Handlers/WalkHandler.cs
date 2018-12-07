using Commander;
using System;

namespace Sample.Implementation
{
    [Executes(HumanCommand.Walk)]
    public class WalkHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            Console.WriteLine($"{contract.Name} is Walking");
        }
    }
}
