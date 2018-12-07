using Commander;
using System;

namespace Sample.Implementation
{
    [Handles(HumanCommand.Eat)]
    public class EatHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human contract)
        {
            Console.WriteLine($"{contract.Name} is Eating");
        }
    }
}
