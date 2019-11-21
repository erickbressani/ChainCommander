using System;
using System.Diagnostics.CodeAnalysis;

namespace ChainCommander.Sample.Implementation
{
    /// <summary>
    /// This is a example of an invalid handler, without the "Handles" attribute.
    /// It will never be called by the CommandChain.
    /// Only used here for the testing purpose.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InvalidHandler : ICommandHandler<HumanCommand, Human>
    {
        public void Handle(Human subject)
            => throw new NotImplementedException();
    }
}