//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ChainCommander.Extensions;
//using ChainCommander.Structure;

//namespace ChainCommander
//{
//    public class CommandChain : ICommandChain
//    {
//        private readonly IServiceProvider _serviceProvider;

//        public CommandChain(IServiceProvider serviceProvider)
//            => _serviceProvider = serviceProvider;

//        public ISubjectBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum
//            => new SubjectBuilder<TCommandType>(_serviceProvider);

//        internal class SubjectBuilder<TCommandType> : ISubjectBuilder<TCommandType> where TCommandType : Enum
//        {
//            private readonly IServiceProvider _serviceProvider;

//            internal SubjectBuilder(IServiceProvider serviceProvider)
//                => _serviceProvider = serviceProvider;

//            public ICommandBuilder<TCommandType, TSubject> Using<TSubject>(IEnumerable<TSubject> subjects)
//                => Using(subjects.ToArray());

//            public ICommandBuilder<TCommandType, TSubject> Using<TSubject>(params TSubject[] subjects)
//            {
//                var handlers = _serviceProvider.GetAllHandlers<TCommandType, TSubject>();
//                return new CommandBuilder<TCommandType, TSubject>(subjects, handlers);
//            }
//        }

//        internal class CommandBuilder<TCommandType, TSubject> : ICommandBuilder<TCommandType, TSubject> where TCommandType : Enum
//        {
//            private readonly IEnumerable<TSubject> _subjects;
//            private readonly IEnumerable<ICommandHandler<TCommandType, TSubject>> _handlers;
//            private readonly List<ICommandHandler<TCommandType, TSubject>> _handlersExecutionStack;

//            internal CommandBuilder(
//                IEnumerable<TSubject> subjects,
//                IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers)
//            {
//                _subjects = subjects;
//                _handlers = handlers;
//                _handlersExecutionStack = new List<ICommandHandler<TCommandType, TSubject>>();
//            }

//            public INextCommandBuilder<TCommandType, TSubject> Do(TCommandType command)
//            {
//                IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers = _handlers.GetBy(command);
//                _handlersExecutionStack.AddRange(handlers);
//                handlers.Execute(_subjects);

//                return new NextCommandBuilder<TCommandType, TSubject>(_subjects, _handlers, _handlersExecutionStack);
//            }
//        }

//        internal class NextCommandBuilder<TCommandType, TSubject> : INextCommandBuilder<TCommandType, TSubject> where TCommandType : Enum
//        {
//            private readonly IEnumerable<TSubject> _subjects;
//            private readonly IEnumerable<ICommandHandler<TCommandType, TSubject>> _handlers;
//            private readonly Stack<ICommandHandler<TCommandType, TSubject>> _handlersExecutionStack;

//            internal NextCommandBuilder(
//                IEnumerable<TSubject> subjects,
//                IEnumerable<ICommandHandler<TCommandType, TSubject>> handlers,
//                Stack<ICommandHandler<TCommandType, TSubject>> handlersExecutionStack)
//            {
//                _subjects = subjects;
//                _handlers = handlers;
//                _handlersExecutionStack = handlersExecutionStack;
//            }

//            public INextCommandBuilder<TCommandType, TSubject> ThenDo(TCommandType command)
//            {
//                _handlers
//                    .GetBy(command)
//                    .Execute(_subjects);

//                return this;
//            }

//            public INextCommandBuilder<TCommandType, TSubject> UndoAll()
//                => UndoLast(_handlers.Count());

//            public INextCommandBuilder<TCommandType, TSubject> Undo(TCommandType command)
//            {
//                _handlers
//                    .GetBy(command)
//                    .Undo(_subjects);

//                return this;
//            }

//            public INextCommandBuilder<TCommandType, TSubject> Undo(params TCommandType[] commands)
//            {
//                foreach (var command in commands)
//                    Undo(command);

//                return this;
//            }

//            public INextCommandBuilder<TCommandType, TSubject> UndoLast(int howMany = 1)
//            {
//                int handlersCount = _handlers.Count();

//                if (howMany > handlersCount)
//                    howMany = handlersCount;

//                for (; howMany > 0; howMany--)
//                {
//                    var handler = _handlers.ToArray()[howMany-1];
//                    handler.Undo(_subjects);
//                }

//                return this;
//            }
//        }
//    }
//}
