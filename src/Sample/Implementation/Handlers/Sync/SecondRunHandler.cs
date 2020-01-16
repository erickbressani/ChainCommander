using System;
using System.Diagnostics.CodeAnalysis;

namespace ChainCommander.Sample.Implementation.Sync
{
    /// <summary>
    /// This "Second" RunHandler was created to test how ChainCommander behaves with two handlers with the same type.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Run)]
    public class SecondRunHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
        {
            subject.IsRunning = true;
            Console.WriteLine($"{subject.Name} is Running");
        }

        public void Undo(Human subject)
        {
            subject.IsRunning = false;
            Console.WriteLine($"{subject.Name} is not Running");
        }
    }
}
