using System;
using System.Collections.Generic;
using System.Linq;
using ChainCommander.Extensions;

namespace ChainCommander
{
    public class CommandChain : ICommandChain
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandChain(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public ISubjectBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum
            => new SubjectBuilder<TCommandType>(_serviceProvider);

        internal class SubjectBuilder<TCommandType> : ISubjectBuilder<TCommandType> where TCommandType : Enum
        {
            private readonly IServiceProvider _serviceProvider;

            internal SubjectBuilder(IServiceProvider serviceProvider)
                => _serviceProvider = serviceProvider;

            public ICommandBuilder<TCommandType, TSubject> Using<TSubject>(IEnumerable<TSubject> subjects)
                => Using(subjects.ToArray());

            public ICommandBuilder<TCommandType, TSubject> Using<TSubject>(params TSubject[] subjects)
            {
                var handlers = _serviceProvider.GetAllHandlers<TCommandType, TSubject>();
                return new CommandBuilder<TCommandType, TSubject>(subjects, handlers);
            }
        }

        internal class CommandBuilder<TCommandType, TSubject> : ICommandBuilder<TCommandType, TSubject> where TCommandType : Enum
        {
            private readonly IEnumerable<ICommandHandler<TCommandType, TSubject>> _handlers;
            private readonly Execution<TCommandType, TSubject> _commandExecutionStack;

            internal CommandBuilder(
                IEnumerable<TSubject> subjects,
                IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers)
            {
                _handlers = handlers;
                _commandExecutionStack = new Execution<TCommandType, TSubject>(subjects);
            }

            public ICommandBuilder<TCommandType, TSubject> Do(TCommandType command)
            {
                var handlers = _handlers.GetBy(command);
                _commandExecutionStack.Add(handlers, command);
                return this;
            }

            public ICommandExecutionStack<TCommandType, TSubject> Execute()
            {
                _commandExecutionStack.Execute();
                return _commandExecutionStack;
            }
        }
    }
}
