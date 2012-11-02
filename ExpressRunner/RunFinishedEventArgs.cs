using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressRunner.Api;

namespace ExpressRunner
{
    public class RunFinishedEventArgs : EventArgs
    {
        private readonly TestStatus status;
        public TestStatus Status
        {
            get { return status; }
        }

        public RunFinishedEventArgs(TestStatus status)
        {
            this.status = status;
        }
    }
}
