using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MiniRunner.Api;

namespace MiniRunner
{
    [Export]
    public class Runner
    {
        private readonly IList<ITestFramework> frameworks;
        private readonly List<TestAssembly> testAssemblies = new List<TestAssembly>();
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
        public Runner([ImportMany] IEnumerable<ITestFramework> frameworks)
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
                testAssemblies.Add(assembly);
                tests.AddRange(assembly.Tests);

                var groupTitle = string.Format("{0} [{1}]", Path.GetFileName(filePath), framework.Name);
                CreateGroup(assembly, groupTitle, assembly.Tests);
            }


            tests.IsNotifying = true;
            tests.Refresh();
        }

        private void CreateGroup(TestAssembly assembly, string groupTitle, IEnumerable<Test> tests)
        {
            var root = new AssemblyTestGroup(assembly, groupTitle, tests);
            testGroups.Add(root);
        }

        public void ReloadAssemblies()
        {
            foreach (var assemblyTestGroup in testGroups)
                assemblyTestGroup.Reload();
        }

        public void RunTests(TestGroup testGroup)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var assembly in testAssemblies)
                    assembly.RunTests(testGroup.Tests);
            });
        }
    }
}
