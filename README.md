# ChainCommander
Generic Command Structure with Undo and Redo features

## Nuget
> Install-Package ChainCommander -Version 2.0.0

## Usage 

First you have to create an enum that represents all the Command:

```csharp
public enum HumanCommand
{
    Eat,
    Run,
    Sleep,
    Walk,
    Work
}
```

Then the concrete Command Handler classes need to implement this interface: ICommandHandler<TCommandType, TSubject>
 - TCommandType: The Enum that represents the command types;
 - TSubject: The type of the class/interface that will be manipulated by the handlers.

Add the Custom Attribute *Handles* above the Command Handler class, passing the Enum Value as a parameter.

```csharp
[Handles(HumanCommand.Work)]
public class WorkHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human subject)
    {
        ...
    }
    
    //Note: This is an optional method. Contains empty default behavior on interface.
    public void Undo(Human subject)
    {
        ...
    }
}
```

Don't forget to inject the Handlers and the CommandChain class:

```csharp
...
.AddTransient<ICommandHandler<HumanCommand, Human>, EatHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, SleepHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, WalkHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, RunHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, WorkHandler>()
.AddChainCommander() //Injects IChainCommander
...
```

## Sample

### Concrete Handlers:
```csharp
[Handles(HumanCommand.Eat)]
public class EatHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human subject)
        => Console.WriteLine($"{subject.Name} is Eating");
        
    public void Undo(Human subject)
        => Console.WriteLine($"{subject.Name} is not Eating");
}

[Handles(HumanCommand.Run)]
public class RunHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human subject)
        => Console.WriteLine($"{subject.Name} is Running");
        
    public void Undo(Human subject)
        => Console.WriteLine($"{subject.Name} is not Running");
}

[Handles(HumanCommand.Sleep)]
public class SleepHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human subject)
        => Console.WriteLine($"{subject.Name} is Sleeping");
        
    public void Undo(Human subject)
        => Console.WriteLine($"{subject.Name} is not Sleeping");
}

[Handles(HumanCommand.Work)]
public class WorkHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human subject)
        => Console.WriteLine($"{subject.Name} is Working");
        
    public void Undo(Human subject)
        => Console.WriteLine($"{subject.Name} is not Working");
}
```

### Building the Command Chain:
```csharp
var chainCommander = serviceProvider.GetService<IChainCommander>();

var human = new Human() { Name = "John" };

chainCommander
    .CreateBasedOn<HumanCommand>()
    .Using(human)
    .Do(HumanCommand.Eat)
    .Do(HumanCommand.Run)
    .Do(HumanCommand.Sleep)
    .Execute();
```

#### Console:
> John is Eating

> John is Running

> John is Sleeping


You can also create your chain using more than one subject:

```csharp
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
```
#### Console:
> John is Eating

> Logan is Eating

> John is Running

> Logan is Running

> John is Sleeping

> Logan is Sleeping

### Undo and Redo Commands
You can easily Undo and Redo executed Commands.

After the Chain executes it will return an IExecutionStack, this interface contains the following property and methods:

```csharp
...

var executionStack = chainCommander
    .CreateBasedOn<HumanCommand>()
    .Using(human1, human2)
    .Do(HumanCommand.Eat)
    .Do(HumanCommand.Run)
    .Do(HumanCommand.Sleep)
    .Execute();
    
... = executionStack.Commands; //A read only list with all the Commands executed in order.

executionStack.UndoAll(); //Calls the Undo method from all the Handlers on the stack.
executionStack.UndoLast(); //Calls the Undo method from the last executed Handler on the stack.
executionStack.UndoLast(3); //Calls the Undo method from all the last executed Handlers on the stack based on the parameter.
executionStack.Undo(HumanCommand.Eat); //Calls the Undo method from all handlers that handles the command passed by parameter.

executionStack.RedoAll(); //Calls the Handle method again from all the Handlers on the stack.
executionStack.RedoLast(); //Calls the Handle method again from the last executed Handler on the stack.
executionStack.RedoLast(3); //Calls the Handle method again from all the last executed Handlers on the stack based on the parameter.
executionStack.Redo(HumanCommand.Eat); //Calls the Handle method again from all handlers that handles the command passed by parameter.
```
