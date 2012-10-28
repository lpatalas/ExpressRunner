using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner.Api
{
    public interface ITestFramework
    {
        string Name { get; }
        TestAssembly LoadAssembly(string assemblyFileName);
    }
}
