namespace Tests.UseCases.EventSourcingTakeTwo.Employee.Events;

public class EmployeePromotedEvent
{
    public Guid Id { get; set; }
    public int LevelIncrease { get; set; } = 0;
}
