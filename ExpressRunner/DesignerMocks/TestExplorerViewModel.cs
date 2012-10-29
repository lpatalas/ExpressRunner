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

        public TestExplorerViewModel()
        {
            selectedTestGroup = new TestGroup("Tests");
            selectedTestGroup.Tests.Add(new TestItem(new Api.Test("First test", "tests", "1")));
            selectedTestGroup.Tests.Add(new TestItem(new Api.Test("Second test", "tests", "2")));
            selectedTestGroup.Tests.Add(new TestItem(new Api.Test("Third test", "tests", "3")));
        }
    }
}
