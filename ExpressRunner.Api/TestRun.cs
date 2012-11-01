using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner.Api
{
    public class TestRun
    {
        private readonly string name;
        public string Name
        {
            get { return name; }
        }

        private readonly TestStatus status;
        public TestStatus Status
        {
            get { return status; }
        }

        public TestRun(string name, TestStatus status)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.status = status;
        }
    }
}
