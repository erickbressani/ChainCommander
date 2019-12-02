using System;
using System.Diagnostics.CodeAnalysis;

namespace ChainCommander.Sample.Implementation.Sync
{
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Eat)]
    public class EatHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
        {
            subject.IsEating = true;
            Console.WriteLine($"{subject.Name} is Eating");
        }

        public void Undo(Human subject)
        {
            subject.IsEating = false;
            Console.WriteLine($"{subject.Name} is not Eating");
        }
    }
}
