using Tests.UseCases;
using System.Reflection;

namespace Tests.Extensions;

static internal class ServiceCollectionExtension
{
    public static IServiceCollection AddUseCases(this IServiceCollection serviceCollection)
    {
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(a => a.GetInterface(nameof(IUseCase)) != null)
            .ToList()
            .ForEach(typeToRegister =>
            {
                serviceCollection.AddTransient(typeToRegister);
            });

        return serviceCollection;
    }
}