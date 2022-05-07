using Tests.UseCases;
using SerilogTimings;

namespace Tests.Extensions;

static internal class HostExtensions
{
    public async static Task RunUseCase<T>(this IHost host) where T : IUseCase
    {
        using (Operation.Time("Executing UseCase {usecase}", typeof(T).Name))
            await host.Services.GetRequiredService<T>().ExecuteAsync();
    }
}
