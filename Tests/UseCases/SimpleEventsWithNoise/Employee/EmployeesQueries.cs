using Marten.Events;

namespace Tests.UseCases.SimpleEventsWithNoise.Employee;

public class EmployeesQueries
{
    private IDocumentStore _Store;

    public EmployeesQueries(IDocumentStore store)
        => _Store = store;

    public async Task<EmployeeAgregate?> GetAsync(Guid id)
    {
        using var session = _Store.QuerySession();
        var employee = await session.Events.AggregateStreamAsync<EmployeeAgregate>(id);

        if (employee?.Employeed == true)
            return employee;

        return null;
    }

}