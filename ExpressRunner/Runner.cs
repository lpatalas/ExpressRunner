using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    [Export]
    public class Runner
    {
        public event EventHandler RunStarting;
        public event EventHandler<RunFinishedEventArgs> RunFinished;

        public void RunTests(TestGroup testGroup)
        {
            OnRunStarting();

            Task.Factory.StartNew(() =>
            {
                testGroup.Run();
                var runStatus = GetAggregatedStatus(testGroup.Tests);
                Execute.OnUIThread(() => OnRunFinished(runStatus));
            });
        }

        private TestStatus GetAggregatedStatus(IEnumerable<TestItem> runItems)
        {
            if (runItems.Any(item => item.Status == TestStatus.Failed))
                return TestStatus.Failed;

            return TestStatus.Succeeded;
        }

        private void OnRunStarting()
        {
            EventHandler handler = RunStarting;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnRunFinished(TestStatus status)
        {
            EventHandler<RunFinishedEventArgs> handler = RunFinished;
            if (handler != null)
                handler(this, new RunFinishedEventArgs(status));
        }
    }
}
