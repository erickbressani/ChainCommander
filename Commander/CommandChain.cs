using Commander.Structure;
using Commander.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commander
{
    public class CommandChain : ICommandChain
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandChain(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public IContractBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum
            => new ContractBuilder<TCommandType>(_serviceProvider);

        internal class ContractBuilder<TCommandType> : IContractBuilder<TCommandType> where TCommandType : Enum
        {
            private IServiceProvider _serviceProvider;

            internal ContractBuilder(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public ICommandBuilder<TCommandType, TContract> Using<TContract>(IEnumerable<TContract> contracts)
                => Using(contracts.ToArray());

            public ICommandBuilder<TCommandType, TContract> Using<TContract>(params TContract[] contracts)
            {
                var handlers = _serviceProvider.GetAllHandlers<TCommandType, TContract>();
                return new CommandBuilder<TCommandType, TContract>(contracts, handlers);
            }
        }

        internal class CommandBuilder<TCommandType, TContract> : ICommandBuilder<TCommandType, TContract> where TCommandType : Enum
        {
            private IEnumerable<TContract> _contracts;
            private IEnumerable<ICommandHandler<TCommandType, TContract>> _handlers;

            internal CommandBuilder(IEnumerable<TContract> contracts, IEnumerable<ICommandHandler<TCommandType, TContract>> handlers)
            {
                _contracts = contracts;
                _handlers = handlers;
            }

            public INextCommandBuilder<TCommandType, TContract> Do(TCommandType step)
            {
                _handlers.GetBy(step)
                         .Execute(_contracts);

                return new NextCommandBuilder<TCommandType, TContract>(_contracts, _handlers);
            }
        }

        internal class NextCommandBuilder<TCommandType, TContract> : INextCommandBuilder<TCommandType, TContract> where TCommandType : Enum
        {
            private IEnumerable<TContract> _contracts;
            private IEnumerable<ICommandHandler<TCommandType, TContract>> _handlers;

            internal NextCommandBuilder(IEnumerable<TContract> contracts, IEnumerable<ICommandHandler<TCommandType, TContract>> handlers)
            {
                _contracts = contracts;
                _handlers = handlers;
            }

            public INextCommandBuilder<TCommandType, TContract> ThenDo(TCommandType step)
            {
                _handlers.GetBy(step)
                         .Execute(_contracts);

                return new NextCommandBuilder<TCommandType, TContract>(_contracts, _handlers);
            }

            public ICommandBuilder<TCommandType, TContract> ThenUsing(IEnumerable<TContract> newContracts)
                => ThenUsing(newContracts.ToArray());

            public ICommandBuilder<TCommandType, TContract> ThenUsing(params TContract[] newContracts)
                => new CommandBuilder<TCommandType, TContract>(newContracts, _handlers);
        }
    }
}
