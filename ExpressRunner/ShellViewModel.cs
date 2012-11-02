using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Shell;
using Caliburn.Micro;

namespace ExpressRunner
{
    [Export]
    public class ShellViewModel
    {
        private readonly TaskbarProgressIndicator taskbarProgressIndicator;
        private readonly TestExplorerViewModel testExplorer;

        public TaskbarItemInfo TaskbarItemInfo
        {
            get { return taskbarProgressIndicator.TaskbarItemInfo; }
        }

        public TestExplorerViewModel TestExplorer
        {
            get { return testExplorer; }
        }

        public ShellViewModel()
        {
            taskbarProgressIndicator = IoC.Get<TaskbarProgressIndicator>();
            testExplorer = IoC.Get<TestExplorerViewModel>();
        }
    }
}
