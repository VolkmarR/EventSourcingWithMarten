namespace Tests.UseCases.SimpleEventsWithNoise.Employee;

public class EmployeeHiredEvent : IEmployeeId
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Level { get; set; } = 0;
}
