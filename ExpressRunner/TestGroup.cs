using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    public class TestGroup
    {
        private readonly string name;
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

        public IList<TestItem> Tests
        {
            get { return tests; }
        }

        public TestGroup(string name)
            : this(name, Enumerable.Empty<TestGroup>(), Enumerable.Empty<Test>())
        {
        }

        public TestGroup(string name, IEnumerable<TestGroup> subGroups, IEnumerable<Test> tests)
        {
            this.name = name;
            this.subGroups = new BindableCollection<TestGroup>(subGroups);
            this.tests = new BindableCollection<TestItem>(tests.Select(test => new TestItem(test)));
        }
    }
}
