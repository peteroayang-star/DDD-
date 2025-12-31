# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Domain-Driven Design (DDD) template project** built with .NET 9.0, implementing core DDD tactical patterns and providing a reusable foundation for business applications. The project uses Clean Architecture with strict dependency rules and includes modern error handling patterns.

## Build and Run Commands

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API (from solution root)
dotnet run --project src/DddTemplate.Api/DddTemplate.Api.csproj

# Clean build artifacts
dotnet clean
```

**API Endpoints**: http://localhost:5002
**Swagger UI**: http://localhost:5002/swagger

## Architecture Overview

### Layer Dependencies (Strict One-Way)

```
Api (Presentation)
  ↓ depends on
Application
  ↓ depends on
Domain (Core - NO dependencies)
  ↑ implemented by
Infrastructure
```

**Critical Rule**: Domain layer has ZERO external dependencies. All other layers depend on Domain.

### Project Structure

- **DddTemplate.Domain**: Core business logic, DDD abstractions (Entity, AggregateRoot, ValueObject, DomainEvent, Result pattern)
- **DddTemplate.Application**: Use cases, application services, DTOs
- **DddTemplate.Infrastructure.InMemory**: Repository implementations (currently in-memory, designed for EF Core replacement)
- **DddTemplate.Api**: REST API, global exception handling, Swagger

## Core DDD Patterns Implementation

### 1. Aggregate Roots

All aggregate roots inherit from `AggregateRoot<TId>` which provides:
- Identity management via `Entity<TId>`
- Domain event collection and management
- Encapsulation of business rules

**Example**:
```csharp
public sealed class TodoItem : AggregateRoot<Guid>
{
    public string Title { get; private set; }  // Private setters enforce encapsulation

    public static TodoItem Create(string title)  // Factory method
    {
        var item = new TodoItem(Guid.NewGuid(), title);
        item.AddDomainEvent(new TodoItemCreatedEvent(item.Id, title));  // Emit event
        return item;
    }

    public void MarkCompleted()  // Business behavior
    {
        if (IsCompleted) return;  // Idempotent
        IsCompleted = true;
        AddDomainEvent(new TodoItemCompletedEvent(Id));
    }
}
```

### 2. Domain Events

Domain events use `record` types for immutability and inherit from `DomainEvent`:

```csharp
public sealed record TodoItemCreatedEvent : DomainEvent
{
    public Guid TodoItemId { get; init; }
    public string Title { get; init; }
}
```

**Event Flow**:
1. Business method executes → adds event to aggregate's `_domainEvents` list
2. Repository saves aggregate
3. Application layer publishes events to handlers
4. Aggregate's events are cleared after publishing

### 3. Result Pattern

Replaces exception-based error handling for expected failures:

```csharp
// Success case
return Result.Success(todoItem);

// Failure case
return Result.Failure<TodoItem>(Error.NotFound("TodoItem.NotFound", "Item not found"));

// Null check
return Result.Create(nullableValue);  // Returns failure if null
```

**When to use**:
- Expected business rule violations
- Validation failures
- Not-found scenarios
- Any predictable failure path

**When NOT to use**:
- Unexpected system errors (let exceptions bubble to global handler)
- Programming errors (null reference, etc.)

### 4. Exception Handling Strategy

**Three-tier approach**:

1. **Domain Layer**: Throw `DomainException` subclasses for business rule violations
   - `ValidationException` → 400 Bad Request
   - `NotFoundException` → 404 Not Found
   - `ConflictException` → 409 Conflict
   - `BusinessRuleException` → 422 Unprocessable Entity

2. **Application Layer**: Can catch and convert to Result, or let bubble up

3. **API Layer**: `GlobalExceptionHandlerMiddleware` catches all exceptions and returns standardized `ApiResponse<T>` format

**Exception to HTTP mapping** is automatic via the middleware.

### 5. Repository Pattern

Generic repository interface in Domain, concrete implementations in Infrastructure:

```csharp
public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task RemoveAsync(TEntity entity, CancellationToken ct = default);
}
```

**Current Implementation**: `InMemoryRepository<TEntity, TId>` using `ConcurrentDictionary`
**Future**: Replace with EF Core implementation while keeping same interface

### 6. Dependency Injection

Each layer has a `DependencyInjection.cs` with extension methods:

```csharp
// Application layer
services.AddApplication();

// Infrastructure layer
services.AddInMemoryInfrastructure();
```

**Lifetimes**:
- Application services: `Scoped`
- Repositories: `Singleton` (InMemory) or `Scoped` (EF Core)

## Adding New Aggregates

1. **Create aggregate root** in `Domain/{AggregateName}/`
   - Inherit from `AggregateRoot<TId>`
   - Use private setters and factory methods
   - Emit domain events for state changes

2. **Define domain events** in `Domain/{AggregateName}/Events/`
   - Use `record` types
   - Inherit from `DomainEvent`

3. **Create repository interface** in Domain
   - Inherit from `IRepository<TEntity, TId>`
   - Add aggregate-specific queries if needed

4. **Implement repository** in Infrastructure
   - Inherit from `InMemoryRepository<TEntity, TId>`
   - Register in `DependencyInjection.cs`

5. **Create application service** in Application
   - Inject repository
   - Orchestrate use cases
   - Map to DTOs

6. **Add API endpoints** in `Program.cs`
   - Use Minimal API style
   - Add `.WithName()` and `.WithTags()` for Swagger

## Key Design Decisions

### Why `record` for Domain Events?
- Immutability by default
- Value-based equality
- Concise syntax
- Clear intent

### Why Result Pattern?
- Makes failure paths explicit in type system
- Avoids exception overhead for expected failures
- Better composability than try-catch
- Forces callers to handle errors

### Why Minimal API?
- Less ceremony than Controllers
- Direct dependency injection in endpoints
- Easier to see request/response flow
- Sufficient for most REST APIs

### Why InMemory Repository?
- Fast development and testing
- Easy to swap for EF Core later
- Same interface, different implementation
- Demonstrates repository pattern clearly

## Extending the Framework

### Adding Value Objects

```csharp
public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string value)
    {
        if (!IsValid(value))
            return Result.Failure<Email>(Error.Validation("Email.Invalid", "Invalid email"));
        return Result.Success(new Email(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

### Adding Domain Services

```csharp
public interface ITodoItemDomainService : IDomainService
{
    bool CanComplete(TodoItem item);
}
```

Domain services are for logic that:
- Doesn't belong to a single aggregate
- Requires multiple aggregates
- Needs external dependencies in domain

### Replacing InMemory with EF Core

1. Create new project: `DddTemplate.Infrastructure.EntityFramework`
2. Add `DbContext` with `DbSet<T>` for each aggregate
3. Implement `IRepository<T, TId>` using EF Core
4. Configure entity mappings (Fluent API)
5. Update `Program.cs` to use new infrastructure

## Common Patterns

### Handling Domain Events

```csharp
// After saving aggregate
var events = aggregate.DomainEvents.ToList();
aggregate.ClearDomainEvents();

foreach (var domainEvent in events)
{
    await _eventPublisher.PublishAsync(domainEvent);
}
```

### API Response Format

All responses use `ApiResponse<T>`:

```json
{
  "success": true,
  "data": { ... },
  "error": null,
  "timestamp": "2025-12-08T07:20:00Z"
}
```

Error responses:

```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "TodoItem.NotFound",
    "message": "TodoItem not found",
    "type": "NotFound",
    "details": "Stack trace (dev only)"
  },
  "timestamp": "2025-12-08T07:20:00Z"
}
```

## Testing Strategy

### Unit Tests (Domain)
- Test aggregate behavior
- Test domain event emission
- Test business rule validation
- No mocking needed (pure domain logic)

### Integration Tests (Application)
- Test use cases end-to-end
- Mock repositories
- Verify event publishing

### API Tests
- Test HTTP endpoints
- Verify response formats
- Test exception handling

## Future Enhancements (Not Yet Implemented)

- CQRS pattern (Command/Query separation)
- MediatR for request/response pipeline
- FluentValidation for input validation
- Unit of Work pattern for transactions
- EF Core persistence
- Specification pattern for complex queries
- Event sourcing (optional)

## Notes

- **Nullable Reference Types**: Enabled project-wide. Some warnings exist but are acceptable.
- **Minimal API**: Preferred over Controllers for simplicity.
- **Swagger**: Auto-configured, available at `/swagger`.
- **Global Exception Handler**: Must be first middleware in pipeline.
- **Domain Events**: Currently collected but not published (awaiting MediatR integration).
