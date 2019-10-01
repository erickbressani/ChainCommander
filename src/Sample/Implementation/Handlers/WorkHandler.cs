using System;

namespace ChainCommander.Sample.Implementation
{
    [Handles(HumanCommand.Work)]
    public class WorkHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
        {
            subject.IsWorking = true;
            Console.WriteLine($"{subject.Name} is Working");
        }
    }
}