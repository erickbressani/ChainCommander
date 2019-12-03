using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander.Sample.Implementation.Async
{
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Eat)]
    public class EatHandler : IAsynchronousCommandHandler<HumanCommand, Human>
    {
        public Task HandleAsync(Human subject, CancellationToken cancellationToken)
        {
            Task.Delay(TimeSpan.FromSeconds(13)).Wait();
            subject.IsEating = true;
            Console.WriteLine($"{subject.Name} is Eating");
            return Task.CompletedTask;
        }

        public Task UndoAsync(Human subject, CancellationToken cancellationToken)
        {
            subject.IsEating = false;
            Console.WriteLine($"{subject.Name} is not Eating");
            return Task.CompletedTask;
        }
    }
}
