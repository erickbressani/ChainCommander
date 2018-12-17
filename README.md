# ChainCommander
Generic Chain of Command Structure

## Nuget
> Install-Package ChainCommander -Version 1.1.1

## Usage 

First you have to create an enum that represents all the Command Types:

```
public enum HumanCommand
{
    Eat,
    Run,
    Sleep,
    Walk,
    Work
}
```

Then the concrete Command Handler classes need to implement this interface: ICommandHandler<TCommandType, TContract>
 - TCommandType: The Enum that represents the command types;
 - TContract: The type of the class that will be manipulated by the handlers.

Add the Custom Attribute *Handles* above the Command Handler class, passing the Enum Value as a parameter.

```
[Handles(HumanCommand.Work)]
public class WorkHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human contract)
    {
        ...
    }
}
```

Don't forget to inject the Handlers and the CommandChain class:

```
...
.AddTransient<ICommandHandler<HumanCommand, Human>, EatHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, SleepHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, WalkHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, RunHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, WorkHandler>()
.AddTransient<ICommandChain, CommandChain>()
...
```

## Sample

### Concrete Handlers:
```
[Handles(HumanCommand.Eat)]
public class EatHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human contract)
        => Console.WriteLine($"{contract.Name} is Eating");
}
[Handles(HumanCommand.Run)]
public class RunHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human contract)
        => Console.WriteLine($"{contract.Name} is Running");
}
[Handles(HumanCommand.Sleep)]
public class SleepHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human contract)
        => Console.WriteLine($"{contract.Name} is Sleeping");
}
[Handles(HumanCommand.Work)]
public class WorkHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human contract)
        => Console.WriteLine($"{contract.Name} is Working");
}
```

### Building the Command Chain:
```
var commandChain = serviceProvider.GetService<ICommandChain>();

var human = new Human() { Name = "John" };

commandChain.CreateBasedOn<HumanCommand>()
            .Using(human)
            .Do(HumanCommand.Eat)
            .ThenDo(HumanCommand.Run)
            .ThenDo(HumanCommand.Sleep);
```

#### Console:
> John is Eating

> John is Running

> John is Sleeping


You can also create your chain using more than one contract and command queue:

```
var commandChain = serviceProvider.GetService<ICommandChain>();

var human1 = new Human() { Name = "John" };
var human2 = new Human() { Name = "Logan" };
var human3 = new Human() { Name = "Roger" };

commandChain.CreateBasedOn<HumanCommand>()
            .Using(human1, human2)
                .Do(HumanCommand.Eat)
                .ThenDo(HumanCommand.Run)
                .ThenDo(HumanCommand.Sleep)
            .ThenUsing(human3)
                .Do(HumanCommand.Work)
                .ThenDo(HumanCommand.Eat);
```
#### Console:
> John is Eating

> Logan is Eating

> John is Running

> Logan is Running

> John is Sleeping

> Logan is Sleeping

> Roger is Working

> Roger is Walking

> Roger is Eating
