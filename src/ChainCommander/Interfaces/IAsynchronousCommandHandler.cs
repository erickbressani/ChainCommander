using System;
using System.Threading.Tasks;

namespace ChainCommander
{
    public interface IAsynchronousCommandHandler<TCommandType, in TSubject> where TCommandType : Enum
    {
        Task HandleAsync(TSubject subject);

        Task UndoAsync(TSubject subject)
        {
            return Task.CompletedTask;
        }
    }
}
