﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner.Api
{
    public class FailedTestRun : TestRun
    {
        private readonly string stackTrace;
        public string StackTrace
        {
            get { return stackTrace; }
        }

        public FailedTestRun(string description, TimeSpan duration, string stackTrace)
            : base(description, duration, TestStatus.Failed)
        {
            if (string.IsNullOrEmpty(stackTrace))
                throw new ArgumentNullException("stackTrace");

            this.stackTrace = stackTrace;
        }
    }
}
