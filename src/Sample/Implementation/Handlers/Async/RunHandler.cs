using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander.Sample.Implementation.Async
{
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Run)]
    public class RunHandler : IAsynchronousCommandHandler<HumanCommand, Human>
    {
        public Task HandleAsync(Human subject, CancellationToken cancellationToken)
        {
            subject.IsRunning = true;
            Console.WriteLine($"{subject.Name} is Running");
            return Task.CompletedTask;
        }

        public Task UndoAsync(Human subject, CancellationToken cancellationToken)
        {
            subject.IsRunning = false;
            Console.WriteLine($"{subject.Name} is not Running");
            return Task.CompletedTask;
        }
    }
}
