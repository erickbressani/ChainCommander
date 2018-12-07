using Commander.Structure;
using Commander.Extensions;
using System;
using System.Collections.Generic;

namespace Commander
{
    public class CommandChain : ICommandChain
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandChain(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public IContractBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum
            => new ContractBuilder<TCommandType>(_serviceProvider);

        public class ContractBuilder<TCommandType> : IContractBuilder<TCommandType> where TCommandType : Enum
        {
            private IServiceProvider _serviceProvider;

            internal ContractBuilder(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public ICommandBuilder<TCommandType, TContract> Using<TContract>(TContract contract)
            {
                var handlers = _serviceProvider.GetAllHandlers<TCommandType, TContract>();
                return new CommandBuilder<TCommandType, TContract>(contract, handlers);
            }
        }

        public class CommandBuilder<TCommandType, TContract> : ICommandBuilder<TCommandType, TContract> where TCommandType : Enum
        {
            private TContract _contract;
            private IEnumerable<ICommandHandler<TCommandType, TContract>> _handlers;

            internal CommandBuilder(TContract contract, IEnumerable<ICommandHandler<TCommandType, TContract>> handlers)
            {
                _contract = contract;
                _handlers = handlers;
            }

            public INextCommandBuilder<TCommandType, TContract> Do(TCommandType step)
            {
                _handlers.GetBy(step)
                         .ExecuteAll(_contract);

                return new NextCommandBuilder<TCommandType, TContract>(_contract, _handlers);
            }
        }

        public class NextCommandBuilder<TCommandType, TContract> : INextCommandBuilder<TCommandType, TContract> where TCommandType : Enum
        {
            private TContract _contract;
            private IEnumerable<ICommandHandler<TCommandType, TContract>> _handlers;

            internal NextCommandBuilder(TContract contract, IEnumerable<ICommandHandler<TCommandType, TContract>> handlers)
            {
                _contract = contract;
                _handlers = handlers;
            }

            public INextCommandBuilder<TCommandType, TContract> ThenDo(TCommandType step)
            {
                _handlers.GetBy(step)
                         .ExecuteAll(_contract);

                return new NextCommandBuilder<TCommandType, TContract>(_contract, _handlers);
            }

            public ICommandBuilder<TCommandType, TContract> ThenUsing(TContract newContract)
                => new CommandBuilder<TCommandType, TContract>(newContract, _handlers);
        }
    }
}
