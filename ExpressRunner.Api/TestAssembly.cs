using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRunner.Api
{
    public abstract class TestAssembly
    {
        private readonly ITestFramework framework;
        public ITestFramework Framework
        {
            get { return framework; }
        }

        public abstract IEnumerable<Test> Tests { get; }

        public abstract void Reload();
        public abstract void RunTests(IEnumerable<IRunnableTest> tests);

        protected TestAssembly(ITestFramework framework)
        {
            if (framework == null)
                throw new ArgumentNullException("framework");
            this.framework = framework;
        }
    }
}
