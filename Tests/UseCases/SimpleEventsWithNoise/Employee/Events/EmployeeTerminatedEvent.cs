namespace Tests.UseCases.SimpleEventsWithNoise.Employee;

public class EmployeeTerminatedEvent : IEmployeeId
{
    public Guid Id { get; set; }
}
