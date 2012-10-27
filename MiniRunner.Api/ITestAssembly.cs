using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRunner.Api
{
    public interface ITestAssembly
    {
        IEnumerable<TestCase> Tests { get; }
        void RunTests();
    }
}
