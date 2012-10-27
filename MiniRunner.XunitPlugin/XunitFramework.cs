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
        public string Name
        {
            get { return "xUnit"; }
        }

        public ITestAssembly LoadAssembly(string assemblyFileName)
        {
            return new XunitTestAssembly(assemblyFileName);
        }
    }
}