using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressRunner.Api;

namespace ExpressRunner
{
    public class AssemblyTestGroup : TestGroup
    {
        private readonly TestAssembly assembly;

        public AssemblyTestGroup(TestAssembly assembly, string name, IEnumerable<Test> tests)
            : base(name, null)
        {
            this.assembly = assembly;
            CreateSubGroups(tests);
        }

        public void Reload()
        {
            assembly.Reload();

            Reset();
            CreateSubGroups(assembly.Tests);
        }

        public override void Run()
        {
            Run(Tests);
        }

        public void Run(IEnumerable<TestItem> testsToRun)
        {
            foreach (var test in testsToRun)
                test.ResetBeforeRun();
            assembly.RunTests(testsToRun);
        }

        private void Reset()
        {
            SubGroups.Clear();
            Tests.Clear();
        }

        private void CreateSubGroups(IEnumerable<Test> tests)
        {
            var testItems = tests.Select(test => new TestItem(test));

            foreach (var testItem in testItems)
            {
                this.Tests.Add(testItem);
                var groups = testItem.Test.Path.Split('/');

                TestGroup currentGroup = this;

                foreach (var part in groups)
                {
                    var matchedGroup = currentGroup.SubGroups.FirstOrDefault(subGroup => subGroup.Name.Equals(part));
                    if (matchedGroup == null)
                    {
                        matchedGroup = new TestGroup(part, this);
                        currentGroup.SubGroups.Add(matchedGroup);
                    }

                    matchedGroup.Tests.Add(testItem);
                    currentGroup = matchedGroup;
                }
            }
        }
    }
}
