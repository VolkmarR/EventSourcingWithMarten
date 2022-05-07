using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Extensions
{
    static internal class HostExtensions
    {
        public async static Task RunUseCase<T>(this IHost host) where T : IUseCase
            => await host.Services.GetRequiredService<T>().ExecuteAsync();
    }
}
