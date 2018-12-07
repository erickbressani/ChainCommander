using Commander;
using System;

namespace Sample.Implementation
{
    [Handles(HumanCommand.Work)]
    public class WorkHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            Console.WriteLine($"{contract.Name} is Working");
        }
    }
}