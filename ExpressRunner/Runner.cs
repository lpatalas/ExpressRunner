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
        public event EventHandler RunFinished;

        public void RunTests(TestGroup testGroup)
        {
            OnRunStarting();

            Task.Factory.StartNew(() =>
            {
                testGroup.Run();
                Execute.OnUIThread(() => OnRunFinished());
            });
        }

        private void OnRunStarting()
        {
            EventHandler handler = RunStarting;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnRunFinished()
        {
            EventHandler handler = RunFinished;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
