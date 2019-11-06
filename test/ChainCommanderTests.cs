using System.Collections.Generic;
using ChainCommander;
using ChainCommander.Sample.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Sample.Tests
{
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
                .AddTransient<ICommandChain, CommandChain>()
                .BuildServiceProvider();
        }

        [Fact]
        public void OneSubject()
        {
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human = new Human();

            commandChain
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
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();
            var human3 = new Human();

            var humans = new List<Human>() { human1, human2, human3 };

            commandChain
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
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();
            var human3 = new Human();

            var humans = new List<Human>() { human1, human2, human3 };

            commandChain
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
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human = new Human();

            var executionStack = commandChain
                .CreateBasedOn<HumanCommand>()
                .Using(human)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Run)
                .Execute();

            Assert.Equal(HumanCommand.Run, executionStack.CommandStack[0]);
            Assert.Equal(HumanCommand.Eat, executionStack.CommandStack[1]);
            Assert.Equal(HumanCommand.Run, executionStack.CommandStack[2]);
            Assert.Equal(HumanCommand.Sleep, executionStack.CommandStack[3]);
            Assert.Equal(HumanCommand.Run, executionStack.CommandStack[4]);
            Assert.True(human.IsEating);
            Assert.True(human.IsRunning);
            Assert.True(human.IsSleeping);
            Assert.False(human.IsWalking);
            Assert.False(human.IsWorking);
        }

        [Fact]
        public void UndoLast()
        {
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            var executionStack = commandChain
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Execute();

            executionStack.UndoLast();

            static void assert(Human human)
            {
                Assert.False(human.IsEating);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public void UndoLastTwo()
        {
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            commandChain
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

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
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            commandChain
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

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
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            commandChain
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

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
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            commandChain
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Work)
                .Execute();

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
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            commandChain
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }
    }
}
