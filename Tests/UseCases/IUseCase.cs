using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UseCases
{
    internal interface IUseCase
    {
        Task ExecuteAsync();
    }
}
