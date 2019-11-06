﻿using System;

namespace ChainCommander
{
    public interface ICommandChain
    {
        ISubjectBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum;
    }
}