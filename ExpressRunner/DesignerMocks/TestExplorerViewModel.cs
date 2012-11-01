using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner.DesignerMocks
{
    public class TestExplorerViewModel
    {
        private readonly TestGroup selectedTestGroup;
        public TestGroup SelectedTestGroup
        {
            get { return selectedTestGroup; }
        }

        private readonly IEnumerable<TestGroup> testGroups;
        public IEnumerable<TestGroup> TestGroups
        {
            get { return testGroups; }
        }

        public TestExplorerViewModel()
        {
            selectedTestGroup = new TestGroup("Tests", null);
            selectedTestGroup.Tests.Add(new TestItem(new Api.Test("First test", "tests", "1")));
            selectedTestGroup.Tests.Add(new TestItem(new Api.Test("Second test", "tests", "2")));
            selectedTestGroup.Tests.Add(new TestItem(new Api.Test("Third test", "tests", "3")));

            var rootGroup = new TestGroup("Root", null);
            rootGroup.SubGroups.Add(new TestGroup("First", null));
            rootGroup.SubGroups.Add(new TestGroup("Second", null));
            rootGroup.SubGroups.Add(new TestGroup("Third", null));
            testGroups = Enumerable.Repeat(rootGroup, 1);
        }
    }
}
