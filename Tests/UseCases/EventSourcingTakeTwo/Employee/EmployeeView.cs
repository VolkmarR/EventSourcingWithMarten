namespace Tests.UseCases.EventSourcingTakeTwo.Employee;

public class EmployeeView : IEmployee
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Level { get; set; }

    public static EmployeeView CreateWith(IEmployee source)
        => new EmployeeView
        {
            Id = source.Id,
            Name = source.Name,
            Level = source.Level,
        };
}
