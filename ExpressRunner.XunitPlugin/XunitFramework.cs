using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniRunner.Api;

namespace MiniRunner.XunitPlugin
{
    [Export(typeof(ITestFramework))]
    public class XunitFramework : ITestFramework
    {
        public string Name
        {
            get { return "xUnit"; }
        }

        public TestAssembly LoadAssembly(string assemblyFileName)
        {
            return new XunitTestAssembly(this, assemblyFileName);
        }
    }
}