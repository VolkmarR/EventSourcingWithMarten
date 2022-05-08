namespace Tests.UseCases.SimpleEventsWithNoise;

public static class EmployeeFaker
{
    static Faker _NameFaker = new();

    public static string CreateName() => _NameFaker.Name.FullName();
}
