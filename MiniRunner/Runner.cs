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
        private readonly BindableCollection<TestGroup> testGroups = new BindableCollection<TestGroup>();
        private readonly BindableCollection<Test> tests = new BindableCollection<Test>();

        public IObservableCollection<TestGroup> TestGroups
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
                CreateGroup(groupTitle, assembly.Tests);
            }


            tests.IsNotifying = true;
            tests.Refresh();
        }

        private void CreateGroup(string groupTitle, IEnumerable<Test> tests)
        {
            var root = new TestGroup(groupTitle);

            foreach (var test in tests)
            {
                root.Tests.Add(test);
                var groups = test.Path.Split('/');

                var currentGroup = root;

                foreach (var part in groups)
                {
                    var matchedGroup = currentGroup.SubGroups.FirstOrDefault(subGroup => subGroup.Name.Equals(part));
                    if (matchedGroup == null)
                    {
                        matchedGroup = new TestGroup(part);
                        currentGroup.SubGroups.Add(matchedGroup);
                    }

                    matchedGroup.Tests.Add(test);
                    currentGroup = matchedGroup;
                }
            }

            testGroups.Add(root);
        }

        public void ReloadAssemblies()
        {
            
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
