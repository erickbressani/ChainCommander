using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ChainCommander.Sample.Implementation.Async
{
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Work)]
    public class WorkHandler : IAsynchronousCommandHandler<HumanCommand, Human>
    {
        public Task HandleAsync(Human subject)
        {
            subject.IsWorking = true;
            Console.WriteLine($"{subject.Name} is Working");
            return Task.CompletedTask;
        }
    }
}
