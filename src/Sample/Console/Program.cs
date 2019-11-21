using System;
using System.Diagnostics.CodeAnalysis;
using ChainCommander.Sample.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace ChainCommander.Sample.ConsoleApp
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main()
        {
            var serviceProvider = BuildServiceProvider();
            var chainCommander = serviceProvider.GetService<IChainCommander>();

            var human1 = new Human() { Name = "John" };
            var human2 = new Human() { Name = "Logan" };

            chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using(human1, human2)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .Execute();

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
                .AddChainCommander()
                .BuildServiceProvider();
        }
    }
}
