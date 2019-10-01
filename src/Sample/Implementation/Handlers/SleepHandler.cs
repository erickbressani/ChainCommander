using System;

namespace ChainCommander.Sample.Implementation
{
    [Handles(HumanCommand.Sleep)]
    public class SleepHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
        {
            subject.IsSleeping = true;
            Console.WriteLine($"{subject.Name} is Sleeping");
        }
    }
}
