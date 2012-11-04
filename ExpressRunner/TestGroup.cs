using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    public class TestGroup : PropertyChangedBase
    {
        private readonly string name;
        private readonly AssemblyTestGroup parentAssembly;
        private readonly BindableCollection<TestGroup> subGroups;
        private readonly BindableCollection<TestItem> tests;

        public string Name
        {
            get { return name; }
        }

        public IList<TestGroup> SubGroups
        {
            get { return subGroups; }
        }

        public IObservableCollection<TestItem> Tests
        {
            get { return tests; }
        }

        public TestGroup(string name, AssemblyTestGroup parentAssembly)
            : this(name, parentAssembly, Enumerable.Empty<TestGroup>(), Enumerable.Empty<Test>())
        {
        }

        public TestGroup(string name, AssemblyTestGroup parentAssembly, IEnumerable<TestGroup> subGroups, IEnumerable<Test> tests)
        {
            this.name = name;
            this.parentAssembly = parentAssembly;
            this.subGroups = new BindableCollection<TestGroup>(subGroups);
            this.tests = new BindableCollection<TestItem>(tests.Select(test => new TestItem(test)));
        }

        public virtual Task RunAsync()
        {
            return parentAssembly.RunAsync(Tests);
        }

        protected void RemoveEmptySubGroups()
        {
            for (int i = subGroups.Count - 1; i >= 0; i--)
            {
                if (subGroups[i].Tests.Count == 0)
                    subGroups.RemoveAt(i);
                else
                    subGroups[i].RemoveEmptySubGroups();
            }
        }

        protected void RemoveTestItems(IEnumerable<TestItem> itemsToRemove)
        {
            Tests.RemoveRange(itemsToRemove);
            foreach (var subGroup in SubGroups)
                subGroup.RemoveTestItems(itemsToRemove);
        }
    }
}
