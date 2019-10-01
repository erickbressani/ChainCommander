using ChainCommander;
using ChainCommander.Sample.Implementation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChainCommander.Sample.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();
            var commandChain = serviceProvider.GetService<ICommandChain>();

            var human1 = new Human() { Name = "John" };
            var human2 = new Human() { Name = "Logan" };
            var human3 = new Human() { Name = "Roger" };

            commandChain
                .CreateBasedOn<HumanCommand>()
                .Using(human1, human2)
                    .Do(HumanCommand.Eat)
                    .ThenDo(HumanCommand.Run)
                    .ThenDo(HumanCommand.Sleep)
                .ThenUsing(human3)
                    .Do(HumanCommand.Work)
                    .ThenDo(HumanCommand.Walk)
                    .ThenDo(HumanCommand.Eat);

            Console.ReadLine();
        }

        private static ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddTransient<ICommandHandler<HumanCommand, Human>, EatHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, SleepHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, WalkHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, RunHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, WorkHandler>()
                .AddTransient<ICommandChain, CommandChain>()
                .BuildServiceProvider();
        }
    }
}
