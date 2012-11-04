using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Windows.Threading;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    [Export]
    public class TaskbarProgressIndicator
        : IHandle<RunStartingEvent>
        , IHandle<RunFinishedEvent>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly TaskbarItemInfo taskbarItem;

        public TaskbarItemInfo TaskbarItemInfo
        {
            get { return taskbarItem; }
        }

        [ImportingConstructor]
        public TaskbarProgressIndicator(
            [Import] IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");

            this.eventAggregator = eventAggregator;
            this.taskbarItem = new TaskbarItemInfo();

            eventAggregator.Subscribe(this);
        }

        void IHandle<RunStartingEvent>.Handle(RunStartingEvent message)
        {
            taskbarItem.ProgressState = TaskbarItemProgressState.Indeterminate;
            taskbarItem.ProgressValue = 0;
        }

        void IHandle<RunFinishedEvent>.Handle(RunFinishedEvent message)
        {
            taskbarItem.ProgressValue = 1.0;

            if (message.Status == TestStatus.Succeeded)
                taskbarItem.ProgressState = TaskbarItemProgressState.Normal;
            else if (message.Status == TestStatus.Failed)
                taskbarItem.ProgressState = TaskbarItemProgressState.Error;
            else
                taskbarItem.ProgressState = TaskbarItemProgressState.None;
        }
    }
}
