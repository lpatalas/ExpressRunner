using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    [Export]
    public class TestRepository
    {
        private readonly IList<ITestFramework> frameworks;
        private readonly BindableCollection<AssemblyTestGroup> testGroups = new BindableCollection<AssemblyTestGroup>();
        private readonly BindableCollection<Test> tests = new BindableCollection<Test>();

        public IObservableCollection<AssemblyTestGroup> TestGroups
        {
            get { return testGroups; }
        }

        public IObservableCollection<Test> Tests
        {
            get { return tests; }
        }

        [ImportingConstructor]
        public TestRepository([ImportMany] IEnumerable<ITestFramework> frameworks)
        {
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");
            this.frameworks = frameworks.ToList().AsReadOnly();
        }

        public void LoadTests(string filePath)
        {
            tests.IsNotifying = false;

            foreach (var framework in frameworks)
            {
                var assembly = framework.LoadAssembly(filePath);
                tests.AddRange(assembly.Tests);

                var groupTitle = Path.GetFileName(filePath);
                CreateGroup(assembly, groupTitle, assembly.Tests);
            }


            tests.IsNotifying = true;
            tests.Refresh();
        }

        public void ReloadAssemblies()
        {
            foreach (var assemblyTestGroup in testGroups)
                assemblyTestGroup.Reload();
        }

        private void CreateGroup(TestAssembly assembly, string groupTitle, IEnumerable<Test> tests)
        {
            var root = new AssemblyTestGroup(assembly, groupTitle, tests);
            testGroups.Add(root);
        }
    }
}
