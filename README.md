# ChainCommander
Generic Command Structure with Undo and Redo features

## Nuget
> Install-Package ChainCommander -Version 3.1.0

## Usage 

First you have to create an enum that represents all the Command:

```csharp
public enum HumanCommand
{
    Eat,
    Sleep,
    Work
}
```

Then the concrete Command Handler classes need to implement this interface: ICommandHandler<TCommandType, TSubject>
 - TCommandType: The Enum that represents the command types;
 - TSubject: The type of the class/interface that will be manipulated by the handlers.

Add the Custom Attribute *Handles* above the Command Handler class, passing the Enum Value as a parameter.

Sync Implemantation:

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

Async Implemantation:

```csharp
[Handles(HumanCommand.Work)]
public class WorkHandler : IAsynchronousCommandHandler<HumanCommand, Human>
{
    public Task HandleAsync(Human subject, CancellationToken cancellationToken)
    {
        ...
    }

    public Task UndoAsync(Human subject, CancellationToken cancellationToken)
    {
        ...
    }
}
```

Don't forget to inject the Handlers and the CommandChain class:

Sync Implemantation:

```csharp
...
.AddTransient<ICommandHandler<HumanCommand, Human>, EatHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, SleepHandler>()
.AddTransient<ICommandHandler<HumanCommand, Human>, WorkHandler>()
.AddChainCommander() //Extension method that injects IChainCommander
...
```

Async Implemantation:

```csharp
...
.AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, EatHandler>()
.AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, SleepHandler>()
.AddTransient<IAsynchronousCommandHandler<HumanCommand, Human>, WorkHandler>()
.AddChainCommander() //Extension method that injects IChainCommander
...
```

## Sample

### Concrete Handlers:

Sync Implemantation:

```csharp
[Handles(HumanCommand.Eat)]
public class EatHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human subject)
        => Console.WriteLine($"{subject.Name} is Eating");
        
    public void Undo(Human subject)
        => Console.WriteLine($"{subject.Name} is not Eating");
}

[Handles(HumanCommand.Work)]
public class WorkHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human subject)
        => Console.WriteLine($"{subject.Name} is Working");
        
    public void Undo(Human subject)
        => Console.WriteLine($"{subject.Name} is not Working");
}

[Handles(HumanCommand.Sleep)]
public class SleepHandler : ICommandHandler<HumanCommand, Human>
{
    public void Handle(Human subject)
        => Console.WriteLine($"{subject.Name} is Sleeping");
        
    public void Undo(Human subject)
        => Console.WriteLine($"{subject.Name} is not Sleeping");
}
```

Async Implemantation:

```csharp
[Handles(HumanCommand.Eat)]
public class EatHandler : IAsynchronousCommandHandler<HumanCommand, Human>
{
    public Task HandleAsync(Human subject, CancellationToken cancellationToken)
    {
        ...
    }
    
    public Task UndoAsync(Human subject, CancellationToken cancellationToken)
    {
        ...
    }
}

[Handles(HumanCommand.Work)]
public class WorkHandler : IAsynchronousCommandHandler<HumanCommand, Human>
{
    public Task HandleAsync(Human subject, CancellationToken cancellationToken)
    {
        ...
    }
        
    public Task UndoAsync(Human subject, CancellationToken cancellationToken)
    {
        ...
    }
}

[Handles(HumanCommand.Sleep)]
public class SleepHandler : IAsynchronousCommandHandler<HumanCommand, Human>
{
    public Task HandleAsync(Human subject, CancellationToken cancellationToken)
    {
        ...
    }
    
    public Task UndoAsync(Human subject, CancellationToken cancellationToken)
    {
        ...
    }
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
    .Do(HumanCommand.Work)
    .Do(HumanCommand.Sleep)
    .Execute(); //Calls Sync handlers
    
await chainCommander
    .CreateBasedOn<HumanCommand>()
    .Using(human)
    .Do(HumanCommand.Eat)
    .Do(HumanCommand.Work)
    .Do(HumanCommand.Sleep)
    .ExecuteAsync() //Calls Async handlers
    .ConfigureAwait(false); 
```

#### Console:
> John is Eating

> John is Working

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
    .Do(HumanCommand.Work)
    .Do(HumanCommand.Sleep)
    .Execute(); //Or ExecuteAsync
```
#### Console:
> John is Eating

> Logan is Eating

> John is Working

> Logan is Working

> John is Sleeping

> Logan is Sleeping

### Undo and Redo Commands
You can easily Undo and Redo executed Commands.

After the Chain executes it will return an IExecutionStack, this interface contains the following property and methods:

```csharp
...

//sync
var executionStack = chainCommander
    .CreateBasedOn<HumanCommand>()
    .Using(human1, human2)
    .Do(HumanCommand.Eat)
    .Do(HumanCommand.Work)
    .Do(HumanCommand.Sleep)
    .Execute();
    
//async
await chainCommander
    .CreateBasedOn<HumanCommand>()
    .Using(human1, human2)
    .Do(HumanCommand.Eat)
    .Do(HumanCommand.Work)
    .Do(HumanCommand.Sleep)
    .ExecuteAsync(out var executionStack)
    .ConfigureAwait(false);
    
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
