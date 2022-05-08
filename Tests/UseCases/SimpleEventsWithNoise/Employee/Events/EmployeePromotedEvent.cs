namespace Tests.UseCases.SimpleEventsWithNoise.Employee;

public class EmployeePromotedEvent : IEmployeeId
{
    public Guid Id { get; set; }
    public int LevelIncrease { get; set; } = 0;
}
