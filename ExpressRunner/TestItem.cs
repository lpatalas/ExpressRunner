using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    public class TestItem : PropertyChangedBase, IRunnableTest
    {
        private int runCount;
        public int RunCount
        {
            get { return runCount; }
            private set
            {
                if (runCount != value)
                {
                    runCount = value;
                    NotifyOfPropertyChange(() => RunCount);
                }
            }
        }

        private TestStatus status = TestStatus.NotRun;
        public TestStatus Status
        {
            get { return status; }
            private set
            {
                if (status != value)
                {
                    status = value;
                    NotifyOfPropertyChange("Status");
                }
            }
        }

        private readonly Test test;
        public Test Test
        {
            get { return test; }
        }

        public TestItem(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            this.test = test;
        }

        public void RecordRun(TestStatus status)
        {
            RunCount++;
            Status = status;
        }
    }
}
