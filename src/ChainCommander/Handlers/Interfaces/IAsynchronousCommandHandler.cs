using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander
{
    public interface IAsynchronousCommandHandler<TCommandType, in TSubject> where TCommandType : Enum
    {
        Task HandleAsync(TSubject subject, CancellationToken cancellationToken);

        Task UndoAsync(TSubject subject, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
