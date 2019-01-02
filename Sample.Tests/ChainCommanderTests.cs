using ChainCommander;
using Microsoft.Extensions.DependencyInjection;
using Sample.Implementation;
using System;
using System.Collections.Generic;
using Xunit;

namespace Sample.Tests
{
    public class ChainCommanderTests
    {
        private ServiceProvider _serviceProvider;

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

            commandChain.CreateBasedOn<HumanCommand>()
                        .Using(human)
                            .Do(HumanCommand.Eat)
                            .ThenDo(HumanCommand.Run)
                            .ThenDo(HumanCommand.Sleep);

            Assert.True(human.IsEating && human.IsRunning && human.IsRunning);
            Assert.False(human.IsWalking || human.IsWorking);
        }

        [Fact]
        public void ThreeSubjects()
        {
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();
            var human3 = new Human();

            var humans = new List<Human>() { human1, human2, human3 };

            commandChain.CreateBasedOn<HumanCommand>()
                        .Using(human1, human2, human3)
                            .Do(HumanCommand.Eat)
                            .ThenDo(HumanCommand.Run)
                            .ThenDo(HumanCommand.Sleep);


            Action<Human> assert = delegate (Human human)
            {
                Assert.True(human.IsEating && human.IsRunning && human.IsSleeping);
                Assert.False(human.IsWalking || human.IsWorking);
            };

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

            commandChain.CreateBasedOn<HumanCommand>()
                        .Using<Human>(humans)
                            .Do(HumanCommand.Work)
                            .ThenDo(HumanCommand.Walk);

            Action<Human> assert = delegate (Human human)
            {
                Assert.True(human.IsWorking && human.IsWalking);
                Assert.False(human.IsSleeping || human.IsEating || human.IsRunning);
            };

            humans.ForEach(assert);
        }

        [Fact]
        public void MoreThanOneChain()
        {
            var commandChain = _serviceProvider.GetService<ICommandChain>();

            var human1 = new Human();
            var human2 = new Human();

            var humans = new List<Human>() { human1, human2};

            commandChain.CreateBasedOn<HumanCommand>()
                        .Using<Human>(humans)
                            .Do(HumanCommand.Work)
                            .ThenDo(HumanCommand.Walk)
                        .ThenUsing(human2)
                            .Do(HumanCommand.Run)
                            .ThenDo(HumanCommand.Sleep);

            Action<Human> commonAssert = delegate (Human human)
            {
                Assert.True(human.IsWorking && human.IsWalking);
                Assert.False(human.IsEating);
            };

            humans.ForEach(commonAssert);

            Assert.False(human1.IsRunning || human1.IsSleeping);
            Assert.True(human2.IsRunning && human2.IsSleeping);
        }
    }
}
