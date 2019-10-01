using System;

namespace ChainCommander.Sample.Implementation
{
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
