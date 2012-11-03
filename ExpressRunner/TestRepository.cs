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
        private readonly IEventAggregator eventAggregator;
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
        public TestRepository(
            [Import] IEventAggregator eventAggregator,
            [ImportMany] IEnumerable<ITestFramework> frameworks)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");

            this.eventAggregator = eventAggregator;
            this.frameworks = frameworks.ToList().AsReadOnly();
        }

        public void LoadTests(string filePath)
        {
            tests.IsNotifying = false;

            foreach (var framework in frameworks)
            {
                var assembly = framework.LoadAssembly(filePath);
                tests.AddRange(assembly.Tests);

                var newGroup = new AssemblyTestGroup(assembly, eventAggregator);
                testGroups.Add(newGroup);
            }


            tests.IsNotifying = true;
            tests.Refresh();
        }

        public async Task ReloadAssembliesAsync()
        {
            foreach (var assemblyTestGroup in testGroups)
                await assemblyTestGroup.ReloadAsync();
        }
    }
}
