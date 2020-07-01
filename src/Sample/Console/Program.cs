using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ChainCommander.Sample.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Async = ChainCommander.Sample.Implementation.Async;
using Sync = ChainCommander.Sample.Implementation.Sync;

namespace ChainCommander.Sample.ConsoleApp
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public async static Task Main()
        {
            var chainCommander = SetupDependency()
                .BuildServiceProvider()
                .GetService<IChainCommander>();

            var human1 = new Human() { Name = "John" };
            var human2 = new Human() { Name = "Logan" };

            await chainCommander
                .CreateBasedOn<HumanCommand>()
                .Using(human1, human2)
                .Do(HumanCommand.Eat)
                .Do(HumanCommand.Run)
                .Do(HumanCommand.Sleep)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }

        private static IServiceCollection SetupDependency()
        {
            return new ServiceCollection()
                .AddTransient<ICommandHandler<HumanCommand, Human>, Sync.EatHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, Sync.SleepHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, Sync.WalkHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, Sync.RunHandler>()
                .AddTransient<ICommandHandler<HumanCommand, Human>, Sync.WorkHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, Async.EatHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, Async.SleepHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, Async.WalkHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, Async.RunHandler>()
                .AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, Async.WorkHandler>()
                .AddChainCommander();
        }
    }
}
