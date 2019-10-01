using System;

namespace ChainCommander.Sample.Implementation
{
    [Handles(HumanCommand.Run)]
    public class RunHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
        {
            subject.IsRunning = true;
            Console.WriteLine($"{subject.Name} is Running");
        }
    }
}
