using Tests.UseCases.EventSourcingTakeTwo.Employee.Events;

namespace Tests.UseCases.EventSourcingTakeTwo.Employee;

public class EmployeeCommands
{
    private IEventSourcingTakeTwoStore _Store;

    public EmployeeCommands(IEventSourcingTakeTwoStore store)
        => _Store = store;

    public async Task<Guid> HireAsync(string name, int level = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        using var session = _Store.OpenSession();

        var e = new EmployeeHiredEvent { Id = Guid.NewGuid(), Name = name, Level = level };
        session.Events.StartStream<Employee>(e.Id, e);

        var aggregate = new Employee();
        aggregate.Apply(e);
        session.Store(EmployeeView.CreateWith(aggregate));
        await session.SaveChangesAsync();

        return e.Id;
    }

    public async Task TerminateAsync(Guid id)
    {
        using var session = _Store.OpenSession();

        var aggregate = await session.Events.AggregateStreamAsync<Employee>(id);
        if (aggregate?.Employeed != true)
            throw new InvalidOperationException("Employee is not employeed");
        var e = new EmployeeTerminatedEvent { Id = id };

        session.Events.Append(e.Id, e);
        session.Delete<EmployeeView>(e.Id);

        await session.SaveChangesAsync();
    }

    public async Task PromoteAsync(Guid id, int levelIncrease)
    {
        if (levelIncrease < 1)
            throw new ArgumentException("Level increase must be at least 1");

        using var session = _Store.OpenSession();
        var aggregate = await session.Events.AggregateStreamAsync<Employee>(id);
        if (aggregate?.Employeed != true)
            throw new InvalidOperationException("Employee is not employeed");
        if (aggregate.Level + levelIncrease > 10)
            throw new InvalidOperationException("Maximum Employee level is 10");

        var e = new EmployeePromotedEvent { Id = id, LevelIncrease = levelIncrease };

        session.Events.Append(id, e);

        aggregate.Apply(e);
        session.Store(EmployeeView.CreateWith(aggregate));

        await session.SaveChangesAsync();
    }

}