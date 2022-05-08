namespace Tests.UseCases.EventSourcingTakeTwo.Employee.Events;

public class EmployeeHiredEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Level { get; set; } = 0;
}
