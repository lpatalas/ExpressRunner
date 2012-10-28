using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace ExpressRunner
{
    [Export]
    public class ShellViewModel
    {
        private readonly TestExplorerViewModel testExplorer;

        public string HelloMessage { get; set; }

        public TestExplorerViewModel TestExplorer
        {
            get { return testExplorer; }
        }

        public ShellViewModel()
        {
            testExplorer = IoC.Get<TestExplorerViewModel>();
            HelloMessage = string.Format("Hello {0}", DateTime.Now);
        }
    }
}
