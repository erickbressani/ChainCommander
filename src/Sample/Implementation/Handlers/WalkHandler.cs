using System;
using System.Diagnostics.CodeAnalysis;

namespace ChainCommander.Sample.Implementation
{
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Walk)]
    public class WalkHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
        {
            subject.IsWalking = true;
            Console.WriteLine($"{subject.Name} is Walking");
        }
    }
}
