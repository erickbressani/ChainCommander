using System;
using System.Collections.Generic;

namespace ChainCommander.Structure
{
    public interface INextCommandBuilder<TCommandType, TSubject> where TCommandType : Enum
    {
        INextCommandBuilder<TCommandType, TSubject> ThenDo(TCommandType step);
        ICommandBuilder<TCommandType, TSubject> ThenUsing(IEnumerable<TSubject> newSubjects);
        ICommandBuilder<TCommandType, TSubject> ThenUsing(params TSubject[] newSubjects);
    }
}