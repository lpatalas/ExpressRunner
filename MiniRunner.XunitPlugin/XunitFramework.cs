using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniRunner.Api;
using Xunit;

namespace MiniRunner.XunitPlugin
{
    [Export(typeof(ITestFramework))]
    public class XunitFramework : ITestFramework
    {
        public ITestAssembly LoadAssembly(string assemblyFileName)
        {
            return new XunitTestAssembly(assemblyFileName);
        }

        public void RunTests(ITestAssembly testCaseSet, IEnumerable<TestCase> testsToRun)
        {
            var xunitTestCaseSet = (XunitTestAssembly)testCaseSet;
            xunitTestCaseSet.RunTests();
        }
    }
}