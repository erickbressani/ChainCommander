using ChainCommander;
using System;

namespace Sample.Implementation
{
    [Handles(HumanCommand.Eat)]
    public class EatHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
        {
            subject.IsEating = true;
            Console.WriteLine($"{subject.Name} is Eating");
        }
    }
}
