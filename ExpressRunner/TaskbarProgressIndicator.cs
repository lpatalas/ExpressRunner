using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Windows.Threading;

namespace ExpressRunner
{
    [Export]
    public class TaskbarProgressIndicator
    {
        private readonly Runner runner;
        private readonly TaskbarItemInfo taskbarItem;

        public TaskbarItemInfo TaskbarItemInfo
        {
            get { return taskbarItem; }
        }

        [ImportingConstructor]
        public TaskbarProgressIndicator(
            [Import] Runner runner)
        {
            if (runner == null)
                throw new ArgumentNullException("runner");

            this.runner = runner;
            this.taskbarItem = new TaskbarItemInfo();

            HookRunnerEvents();
        }

        private void HookRunnerEvents()
        {
            runner.RunStarting += runner_RunStarting;
            runner.RunFinished += runner_RunFinished;
        }

        private void runner_RunStarting(object sender, EventArgs e)
        {
            taskbarItem.ProgressState = TaskbarItemProgressState.Indeterminate;
        }

        private void runner_RunFinished(object sender, EventArgs e)
        {
            taskbarItem.ProgressState = TaskbarItemProgressState.None;
        }
    }
}
