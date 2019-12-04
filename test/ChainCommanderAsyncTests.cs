using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ChainCommander.Sample.Implementation;
using ChainCommander.Sample.Implementation.Async;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ChainCommander.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    public class ChainCommanderAsyncTests
    {
        private readonly ServiceProvider _serviceProvider;

        public ChainCommanderAsyncTests()
            => _serviceProvider = BuildServiceProvider();

        private static ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, EatHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, SleepHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, WalkHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, RunHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, WorkHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, InvalidHandler>()
                .AddChainCommander()
                .BuildServiceProvider();
        }

        [Fact]
        public async Task OneSubjectAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human = new Human();

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using(human)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync()
                .ConfigureAwait(false);

            Assert.True(human.IsEating);
            Assert.True(human.IsRunning);
            Assert.True(human.IsSleeping);
            Assert.False(human.IsWalking);
            Assert.False(human.IsWorking);
        }

        [Fact]
        public async Task ThreeSubjectsAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();
            var human3 = new Human();

            var humans = new List<Human> { human1, human2, human3 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using(human1, human2, human3)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync()
                .ConfigureAwait(false);

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
        public async Task ListOfSubjectAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();
            var human3 = new Human();

            var humans = new List<Human> { human1, human2, human3 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Work)
                .Do(HumanCommand.Walk)
                .ExecuteAsync()
                .ConfigureAwait(false);

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
        public async Task MoreThanOneOfTheSameCommandAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human = new Human();

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using(human)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Run)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

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
        public async Task UndoLastAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Walk)
                .Do(HumanCommand.Work)
                .Do(HumanCommand.Eat)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoLastAsync().ConfigureAwait(false);

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
        public async Task UndoLastTwoAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoLastAsync(2).ConfigureAwait(false);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.False(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public async Task UndoLastMoreThanLenghtAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoLastAsync(10).ConfigureAwait(false);

            static void assert(Human human)
            {
                Assert.False(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.False(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public async Task UndoAllAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoAllAsync().ConfigureAwait(false);

            static void assert(Human human)
            {
                Assert.False(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.False(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public async Task UndoWithNoUndoImplementationAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Work)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoAllAsync().ConfigureAwait(false);

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
        public async Task UndoAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoAsync(HumanCommand.Run).ConfigureAwait(false);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.False(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public async Task RedoLastAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Work)
                .Do(HumanCommand.Walk)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Eat)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoLastAsync(2).ConfigureAwait(false);
            await executionStack.RedoLastAsync().ConfigureAwait(false);

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
        public async Task RedoLastTwoAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoLastAsync(2).ConfigureAwait(false);
            await executionStack.RedoLastAsync(2).ConfigureAwait(false);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public async Task RedoLastMoreThanLenghtAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoLastAsync(10).ConfigureAwait(false);
            await executionStack.RedoLastAsync(10).ConfigureAwait(false);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public async Task RedoAllAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoAllAsync().ConfigureAwait(false);
            await executionStack.RedoAllAsync().ConfigureAwait(false);

            static void assert(Human human)
            {
                Assert.True(human.IsEating);
                Assert.True(human.IsRunning);
                Assert.True(human.IsSleeping);
            }

            humans.ForEach(assert);
        }

        [Fact]
        public async Task RedoWithNoUndoImplementationAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Do(HumanCommand.Work)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoAllAsync().ConfigureAwait(false);
            await executionStack.RedoAllAsync().ConfigureAwait(false);

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
        public async Task RedoAsync()
        {
            var chainCommander = _serviceProvider.GetService<IChainCommander>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2 };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using<Human>(humans)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync(out var executionStack)
                .ConfigureAwait(false);

            await executionStack.UndoAsync(HumanCommand.Run).ConfigureAwait(false);
            await executionStack.RedoAsync(HumanCommand.Run).ConfigureAwait(false);

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
