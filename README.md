**CQRSimple**


# How to Use CQRSimple

## Introduction
CQRSimple is a simple and lightweight NuGet package that implements CQRS (Command Query Responsibility Segregation) in C#. This guide will help you get started with CQRSimple in your .NET project.

## Prerequisites
- .NET 6.0 or later

## Installation
- .NET CLI
```properties
dotnet add package CQRSimple
````
- Package Manager
```PS
Install-Package CQRSimple
````
- PackageReference
```XML
<PackageReference Include="CQRSimple" Version="1.0.0" />
````


## Setting Up CQRSimple

### 1. Define Commands and Queries
Create classes for your commands and queries. Commands are used to change the state, while queries are used to retrieve data.

```csharp
// Command
public class CreateUserCommand : ICommand
{
    public string Name { get; }
    public string Email { get; }

    public CreateUserCommand(string name, string email)
    {
        Name = name;
        Email = email;
    }
}

// Query
public class GetUserQuery : IQuery<User>
{
    public int UserId { get; }

    public GetUserQuery(int userId)
    {
        UserId = userId;
    }
}
````

### 2. Implement Command and Query Handlers

Create handlers for your commands and queries. Handlers contain the business logic for processing commands and queries.

```csharp
// Command Handler
public class CreateUserCommandHandler : : ICommandHandler<CreateUserCommand>
{
    public Task HandleAsync(CreateUserCommand command)
    {
        // Business logic to create a user
        return Task.CompletedTask;
    }
}

// Query Handler
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
{
    public Task<User> HandleAsync(GetUserQuery query)
    {
        // Business logic to get a user
        return Task.FromResult(new User(query.UserId, "John Doe"));
    }
}
````

### 3. Register Handlers in Dependency Injection Container

Register your command and query handlers in the dependency injection container by calling one the extension method AddCQRSimple.

```csharp
// Scan AppDomain assemblies to register Queries, Commands and Handlers
services.AddCQRSimple()

// Scan the passed assembly to register Queries, Commands and Handlers
services.AddCQRSimple(Assembly.GetExecutingAssembly())

// Register Queries, Commands and Handlers from an array of assemblies
Assembly[] assemblies = new[] {assembly1, assembly2, assembly3};
services.AddCQRSimple(assemblies)
````

### 4. Dispatching commands and queries


Inject the IDispatcher interface into your services or controllers and use it to dispatch commands and queries.

```csharp
public class UserController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public UserController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        await _dispatcher.DispatchAsync(command);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _dispatcher.DispatchAsync<GetUserQuery, User>(new GetUserQuery(id));
        return Ok(user);
    }
}
````