using Commander;
using System;

namespace Sample.Implementation
{
    [Executes(HumanCommand.Run)]
    public class RunHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            Console.WriteLine($"{contract.Name} is Running");
        }
    }
}
