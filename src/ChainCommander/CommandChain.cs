using ChainCommander.Structure;
using ChainCommander.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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
            private readonly IEnumerable<TSubject> _subjects;
            private readonly IEnumerable<ICommandHandler<TCommandType, TSubject>> _handlers;

            internal CommandBuilder(IEnumerable<TSubject> subjects, IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers)
            {
                _subjects = subjects;
                _handlers = handlers;
            }

            public INextCommandBuilder<TCommandType, TSubject> Do(TCommandType step)
            {
                _handlers.GetBy(step)
                         .Execute(_subjects);

                return new NextCommandBuilder<TCommandType, TSubject>(_subjects, _handlers);
            }
        }

        internal class NextCommandBuilder<TCommandType, TSubjects> : INextCommandBuilder<TCommandType, TSubjects> where TCommandType : Enum
        {
            private readonly IEnumerable<TSubjects> _subjects;
            private readonly IEnumerable<ICommandHandler<TCommandType, TSubjects>> _handlers;

            internal NextCommandBuilder(IEnumerable<TSubjects> subjects, IEnumerable<ICommandHandler<TCommandType, TSubjects>> handlers)
            {
                _subjects = subjects;
                _handlers = handlers;
            }

            public INextCommandBuilder<TCommandType, TSubjects> ThenDo(TCommandType step)
            {
                _handlers.GetBy(step)
                         .Execute(_subjects);

                return this;
            }

            public ICommandBuilder<TCommandType, TSubjects> ThenUsing(IEnumerable<TSubjects> newSubjects)
                => ThenUsing(newSubjects.ToArray());

            public ICommandBuilder<TCommandType, TSubjects> ThenUsing(params TSubjects[] newSubjects)
                => new CommandBuilder<TCommandType, TSubjects>(newSubjects, _handlers);
        }
    }
}
