using Tests.UseCases.EventSourcingTakeTwo.Employee;

namespace Tests.UseCases.EventSourcingTakeTwo;

public interface IEventSourcingTakeTwoStore : IDocumentStore
{

}

internal static class Register
{
    internal static void AddEventSourcingTakeTwoStore(this IServiceCollection services, string connectionString)
    {
        services.AddMartenStore<IEventSourcingTakeTwoStore>(opts =>
        {
            opts.Connection(connectionString);
            opts.AddSerilog();
        });
    }


}