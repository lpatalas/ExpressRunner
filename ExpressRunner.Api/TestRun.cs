using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner.Api
{
    public class TestRun
    {
        private readonly string description;
        public string Description
        {
            get { return description; }
        }

        private readonly TimeSpan duration;
        public TimeSpan Duration
        {
            get { return duration; }
        }

        private readonly TestStatus status;
        public TestStatus Status
        {
            get { return status; }
        }

        public TestRun(string description, TimeSpan duration, TestStatus status)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");

            this.description = description;
            this.duration = duration;
            this.status = status;
        }
    }
}
