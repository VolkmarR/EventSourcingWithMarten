
namespace Tests.UseCases.EventSourcingTakeTwo.Employee
{
    public interface IEmployee
    {
        Guid Id { get; }
        string Name { get; }
        int Level { get; }
    }
}