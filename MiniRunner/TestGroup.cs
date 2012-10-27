using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniRunner.Api;

namespace MiniRunner
{
    public class TestGroup
    {
        private readonly string name;
        private readonly IList<TestGroup> subGroups;
        private readonly IList<TestCase> tests;

        public string Name
        {
            get { return name; }
        }

        public IList<TestGroup> SubGroups
        {
            get { return subGroups; }
        }

        public IList<TestCase> Tests
        {
            get { return tests; }
        }

        public TestGroup(string name)
        {
            this.name = name;
            this.subGroups = new List<TestGroup>();
            this.tests = new List<TestCase>();
        }

        public TestGroup(string name, IEnumerable<TestGroup> subGroups, IEnumerable<TestCase> tests)
        {
            this.name = name;
            this.subGroups = subGroups.ToList();
            this.tests = tests.ToList();
        }
    }
}
