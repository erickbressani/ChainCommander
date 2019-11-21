using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChainCommander.Sample.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ChainCommander.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    public class ChainCommanderTests
    {
        private readonly ServiceProvider _serviceProvider;

        public ChainCommanderTests()
            => _serviceProvider = BuildServiceProvider();

        private static ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddTransient<ICommandHandler<HumanCommand, Human>, EatHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, SleepHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, WalkHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, RunHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, WorkHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, InvalidHandler>()
                .AddChainCommander()
                .BuildServiceProvider();
        }

        [Fact]
        public void OneSubject()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human = new Human();

            chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using(human)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            Assert.True(human.IsEating);
            Assert.True(human.IsRunning);
            Assert.True(human.IsSleeping);
            Assert.False(human.IsWalking);
            Assert.False(human.IsWorking);
        }

        [Fact]
        public void ThreeSubjects()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();
            var human3 = new Human();

            var humans = new List<Human>() { human1, human2, human3 };

            chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using(human1, human2, human3)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
                Assert.False(human.IsWalking);
                Assert.False(human.IsWorking);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void ListSubject()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();
            var human3 = new Human();

            var humans = new List<Human>() { human1, human2, human3 };

            chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Work)
                .Do(HumanCommand.Walk)
                .Execute();

            static void assert(Human human)
            {
                Assert.True(human.IsWorking);
                Assert.True(human.IsWalking);
                Assert.False(human.IsSleeping);
                Assert.False(human.IsEating);
                Assert.False(human.IsRunning);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void MoreThanOneOfTheSameCommand()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human = new Human();

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using(human)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Run)
                .Execute();

            Assert.Equal(HumanCommand.Run, executionStack.Commands[0]);
            Assert.Equal(HumanCommand.Eat, executionStack.Commands[1]);
            Assert.Equal(HumanCommand.Run, executionStack.Commands[2]);
            Assert.Equal(HumanCommand.Sleep, executionStack.Commands[3]);
            Assert.Equal(HumanCommand.Run, executionStack.Commands[4]);
            Assert.True(human.IsEating);
            Assert.True(human.IsRunning);
            Assert.True(human.IsSleeping);
            Assert.False(human.IsWalking);
            Assert.False(human.IsWorking);
        }

        [Fact]
        public void UndoLast()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Walk)
                .Do(HumanCommand.Work)
                .Do(HumanCommand.Eat)
                .Execute();

            executionStack.UndoLast();

            static void assert(Human human)
            {
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
                Assert.True(human.IsWalking);
                Assert.True(human.IsWorking);
                Assert.False(human.IsEating);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void UndoLastTwo()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            executionStack.UndoLast(2);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.False(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void UndoLastMoreThanLenght()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            executionStack.UndoLast(10);

            static void assert(Human human)
            {
                Assert.False(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.False(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void UndoAll()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            executionStack.UndoAll();

            static void assert(Human human)
            {
                Assert.False(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.False(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void UndoWithNoUndoImplementation()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Work)
                .Execute();

            executionStack.UndoAll();

            static void assert(Human human)
            {
                Assert.False(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.False(human.IsSleeping);
                Assert.True(human.IsWorking);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void Undo()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            executionStack.Undo(HumanCommand.Run);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void RedoLast()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Work)
                .Do(HumanCommand.Walk)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Eat)
                .Execute();

            executionStack.UndoLast(2);
            executionStack.RedoLast();

            static void assert(Human human)
            {
                Assert.True(human.IsRunning);
                Assert.True(human.IsWorking);
                Assert.True(human.IsWalking);
                Assert.False(human.IsSleeping);
                Assert.True(human.IsEating);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void RedoLastTwo()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            executionStack.UndoLast(2);
            executionStack.RedoLast(2);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void RedoLastMoreThanLenght()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            executionStack.UndoLast(10);
            executionStack.RedoLast(10);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void RedoAll()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            executionStack.UndoAll();
            executionStack.RedoAll();

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void RedoWithNoUndoImplementation()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Work)
                .Execute();

            executionStack.UndoAll();
            executionStack.RedoAll();

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
                Assert.True(human.IsWorking);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void Redo()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            executionStack.Undo(HumanCommand.Run);
            executionStack.Redo(HumanCommand.Run);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }
    }
}
