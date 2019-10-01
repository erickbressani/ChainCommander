using System;
using System.Collections.Generic;

namespace ChainCommander.Structure
{
    public interface ISubjectBuilder<TCommandType> where TCommandType : Enum
    {
        ICommandBuilder<TCommandType, TSubject> Using<TSubject>(IEnumerable<TSubject> subjects);

        ICommandBuilder<TCommandType, TSubject> Using<TSubject>(params TSubject[] subjects);
    }
}