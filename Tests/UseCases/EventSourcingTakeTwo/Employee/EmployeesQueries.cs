namespace Tests.UseCases.EventSourcingTakeTwo.Employee;

public class EmployeesQueries
{
    private IEventSourcingTakeTwoStore _Store;

    public EmployeesQueries(IEventSourcingTakeTwoStore store)
        => _Store = store;

    public async Task<Employee?> GetFromStreamAsync(Guid id)
    {
        using var session = _Store.QuerySession();
        var employee = await session.Events.AggregateStreamAsync<Employee>(id);

        if (employee?.Employeed == true)
            return employee;

        return null;
    }

    public async Task<EmployeeView?> GetFromProjectionAsync(Guid id)
    {
        using var session = _Store.QuerySession();
        return await session.LoadAsync<EmployeeView>(id);
    }

    public async Task<IReadOnlyList<EmployeeView>> GetAllFromProjectionAsync()
    {
        using var session = _Store.QuerySession();
        return await session.Query<EmployeeView>().ToListAsync();
    }

}