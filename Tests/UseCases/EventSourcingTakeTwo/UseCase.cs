using Tests.UseCases.EventSourcingTakeTwo.Employee;

namespace Tests.UseCases.EventSourcingTakeTwo;

public class EventSourcingTakeTwo
{
    public class UseCase : IUseCase
    {
        private readonly IEventSourcingTakeTwoStore _Store;

        public UseCase(IEventSourcingTakeTwoStore store) => _Store = store;

        public async Task ExecuteAsync()
        {

            Log.Information("Reset database");
            using (Operation.Time(">> Duration"))
                await _Store.Advanced.Clean.CompletelyRemoveAllAsync();
            var list = await CreateEmployees();

            await QueryEmployee(list);

        }

        private async Task<List<Guid>> CreateEmployees()
        {
            var employeesCommands = new EmployeeCommands(_Store);
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
        static void Dump(IEmployee? item)
        {
            if (item != null)
                Log.Information("Employee {id} - {name} - level {level}", item.Id, item.Name, item.Level);
        }

        private async Task QueryEmployee(List<Guid> list)
        {
            var employeesQueries = new EmployeesQueries(_Store);
            Log.Information("List all not terminated employees one by one from stream");
            using (Operation.Time(">> Duration"))
            {
                foreach (var id in list)
                    Dump(await employeesQueries.GetFromStreamAsync(id));
            }

            Log.Information("List all not terminated employees one by one from projection");
            using (Operation.Time(">> Duration"))
            {
                foreach (var id in list)
                    Dump(await employeesQueries.GetFromProjectionAsync(id));
            }

            Log.Information("List all not terminated employees from projection");
            using (Operation.Time(">> Duration"))
            {
                foreach (var item in await employeesQueries.GetAllFromProjectionAsync())
                    Dump(item);
            }
        }

    }
}