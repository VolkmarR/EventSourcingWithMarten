namespace Tests.UseCases.SimpleEventsWithNoise.Employee;

public class EmployeesCommands
{
    private IDocumentStore _Store;

    public EmployeesCommands(IDocumentStore store)
        => _Store = store;

    public async Task<Guid> HireAsync(string name, int level = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        using var session = _Store.OpenSession();

        var id = Guid.NewGuid();
        session.Events.StartStream<EmployeeAgregate>(id, new EmployeeHiredEvent { Id = id, Name = name, Level = level });
        await session.SaveChangesAsync();

        return id;
    }

    public async Task TerminateAsync(Guid id)
    {
        using var session = _Store.OpenSession();

        var aggregate = await session.Events.AggregateStreamAsync<EmployeeAgregate>(id);
        if (aggregate?.Employeed != true)
            throw new InvalidOperationException("Employee is not employeed");

        session.Events.Append(id, new EmployeeTerminatedEvent { Id = id });
        await session.SaveChangesAsync();
    }

    public async Task PromoteAsync(Guid id, int levelIncrease)
    {
        if (levelIncrease < 1)
            throw new ArgumentException("Level increase must be at least 1");

        using var session = _Store.OpenSession();
        var aggregate = await session.Events.AggregateStreamAsync<EmployeeAgregate>(id);
        if (aggregate?.Employeed != true)
            throw new InvalidOperationException("Employee is not employeed");
        if (aggregate.Level + levelIncrease > 10)
            throw new InvalidOperationException("Maximum Employee level is 10");

        session.Events.Append(id, new EmployeePromotedEvent { Id = id, LevelIncrease = levelIncrease });
        await session.SaveChangesAsync();
    }

}