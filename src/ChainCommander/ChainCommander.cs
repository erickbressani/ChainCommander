using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChainCommander
{
    internal class ChainCommander : IChainCommander
    {
        private readonly IServiceProvider _serviceProvider;

        public ChainCommander(IServiceProvider serviceProvider)
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
            private readonly ExecutionStack<TCommandType, TSubject> _commandExecutionStack;

            internal CommandBuilder(
                IEnumerable<TSubject> subjects,
                IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers)
            {
                _handlers = handlers;
                _commandExecutionStack = new ExecutionStack<TCommandType, TSubject>(subjects);
            }

            public ICommandBuilder<TCommandType, TSubject> Do(TCommandType command)
            {
                var handlers = _handlers.GetBy(command);
                _commandExecutionStack.Add(handlers, command);
                return this;
            }

            public IExecutionStack<TCommandType, TSubject> Execute()
            {
                _commandExecutionStack.Execute();
                return _commandExecutionStack;
            }
        }

        internal class AsynchronousCommandBuilder<TCommandType, TSubject> : IAsynchronousCommandBuilder<TCommandType, TSubject> where TCommandType : Enum
        {
            private readonly IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> _handlers;
            private readonly AsynchronousExecutionStack<TCommandType, TSubject> _commandExecutionStack;

            internal AsynchronousCommandBuilder(
                IEnumerable<TSubject> subjects,
                IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> handlers)
            {
                _handlers = handlers;
                _commandExecutionStack = new AsynchronousExecutionStack<TCommandType, TSubject>(subjects);
            }

            public IAsynchronousCommandBuilder<TCommandType, TSubject> Do(TCommandType command)
            {
                var handlers = _handlers.GetBy(command);
                _commandExecutionStack.Add(handlers, command);
                return this;
            }

            public async Task<IAsynchronousExecutionStack<TCommandType, TSubject>> ExecuteAsync()
            {
                await _commandExecutionStack.ExecuteAsync().ConfigureAwait(false);
                return _commandExecutionStack;
            }

            public async Task<IAsynchronousExecutionStack<TCommandType, TSubject>> ExecuteInOrderAsync()
            {
                await _commandExecutionStack.ExecuteInOrderAsync().ConfigureAwait(false);
                return _commandExecutionStack;
            }
        }
    }
}
