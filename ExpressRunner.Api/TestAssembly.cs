using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner.Api
{
    public abstract class TestAssembly
    {
        private readonly string sourceFilePath;
        public string SourceFilePath
        {
            get { return sourceFilePath; }
        }

        public abstract IEnumerable<Test> Tests { get; }

        public abstract void Reload();
        public abstract void RunTests(IEnumerable<IRunnableTest> tests);

        protected TestAssembly(string sourceFilePath)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
                throw new ArgumentNullException("sourceFilePath");

            this.sourceFilePath = sourceFilePath;
        }
    }
}
