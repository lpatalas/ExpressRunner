using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Windows.Threading;
using ExpressRunner.Api;

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
            taskbarItem.ProgressValue = 0;
        }

        private void runner_RunFinished(object sender, RunFinishedEventArgs e)
        {
            taskbarItem.ProgressValue = 1.0;

            if (e.Status == TestStatus.Succeeded)
                taskbarItem.ProgressState = TaskbarItemProgressState.Normal;
            else if (e.Status == TestStatus.Failed)
                taskbarItem.ProgressState = TaskbarItemProgressState.Error;
            else
                taskbarItem.ProgressState = TaskbarItemProgressState.None;
        }
    }
}
