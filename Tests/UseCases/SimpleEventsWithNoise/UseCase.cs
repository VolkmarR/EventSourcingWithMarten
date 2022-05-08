using Tests.UseCases.SimpleEventsWithNoise.Employee;

namespace Tests.UseCases.SimpleEventsWithNoise;

public class UseCase : IUseCase
{
    private readonly IDocumentStore _Store;

    public UseCase(IDocumentStore store) => _Store = store;

    public async Task ExecuteAsync()
    {

        Log.Information("Delete all existing events");
        using (Operation.Time(">> Duration"))
            await _Store.Advanced.Clean.DeleteAllEventDataAsync();

        var list = await CreateEmployees();

        await QueryEmployees(list);
    }

    private async Task<List<Guid>> CreateEmployees()
    {
        var employeesCommands = new EmployeesCommands(_Store);
        var cases = 50;
        var list = new List<Guid>();

        Log.Information("Create {cases} employees", cases);
        using (Operation.Time(">> Duration"))
        {
            for (var i = 0; i < cases; i++)
                list.Add(await employeesCommands.HireAsync(EmployeeFaker.CreateName()));
        }

        Log.Information("Promote all {cases} employees", cases);
        using (Operation.Time(">> Duration"))
        {
            foreach (var id in list)
                await employeesCommands.PromoteAsync(id, 1);
        }

        Log.Information("Terminate first {cases} employees", cases / 2);
        using (Operation.Time(">> Duration"))
        {
            foreach (var id in list.Take(cases / 2))
                await employeesCommands.TerminateAsync(id);
        }

        return list;
    }

    private async Task QueryEmployees(List<Guid> list)
    {
        var employeesQueries = new EmployeesQueries(_Store);
        Log.Information("List all not terminated employees one by one");
        using (Operation.Time(">> Duration"))
        {
            foreach (var id in list)
            {
                var employee = await employeesQueries.GetAsync(id);
                if (employee != null && employee.Employeed)
                    Log.Information("Employee {id} - {name} - level {level}", employee.Id, employee.Name, employee.Level);
            }
        }
    }
}