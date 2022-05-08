namespace Tests.UseCases.SimpleEventsWithNoise.Employee;

public class EmployeeAgregate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    public int Level { get; private set; } = 0;
    public bool Employeed { get; private set; } = false;


    public void Apply(EmployeeHiredEvent hired)
    {
        Id = hired.Id;
        Name = hired.Name;
        Level = hired.Level;
        Employeed = true;
    }

    public void Apply(EmployeePromotedEvent promoted)
    {
        Level += promoted.LevelIncrease;
    }

    public void Apply(EmployeeTerminatedEvent terminated)
    {
        Employeed = false;
    }
}
