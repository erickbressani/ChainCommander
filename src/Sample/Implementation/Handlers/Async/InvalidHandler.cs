using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander.Sample.Implementation.Async
{
    /// <summary>
    /// This is a example of an invalid handler, without the "Handles" attribute.
    /// It will never be called by the CommandChain.
    /// Only used here for the testing purpose.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InvalidHandler : IAsynchronousCommandHandler<HumanCommand, Human>
    {
        public Task HandleAsync(Human subject, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }
}