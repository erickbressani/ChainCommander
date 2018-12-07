using Commander;
using Microsoft.Extensions.DependencyInjection;
using Sample.Implementation;
using System;

namespace Sample.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();
            var commandChain = serviceProvider.GetService<ICommandChain>();

            var human1 = new Human() { Name = "John" };
            var human2 = new Human() { Name = "Erick" };

            commandChain.CreateBasedOn<HumanCommand>()
                        .Using(human1)
                        .Do(HumanCommand.Eat)
                        .ThenDo(HumanCommand.Run)
                        .ThenDo(HumanCommand.Sleep)
                        .ThenUsing(human2)
                        .Do(HumanCommand.Work)
                        .ThenDo(HumanCommand.Walk)
                        .ThenDo(HumanCommand.Eat);

            Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine();

            commandChain.CreateBasedOn<HumanCommand>()
                        .Using(human1)
                        .Do(HumanCommand.Walk)
                        .ThenDo(HumanCommand.Run)
                        .ThenDo(HumanCommand.Work)
                        .ThenDo(HumanCommand.Sleep);

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
