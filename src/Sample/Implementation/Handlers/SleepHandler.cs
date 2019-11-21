using System;
using System.Diagnostics.CodeAnalysis;

namespace ChainCommander.Sample.Implementation
{
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Sleep)]
    public class SleepHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
        {
            subject.IsSleeping = true;
            Console.WriteLine($"{subject.Name} is Sleeping");
        }

        public void Undo(Human subject)
        {
            subject.IsSleeping = false;
            Console.WriteLine($"{subject.Name} is not Sleeping");
        }
    }
}
