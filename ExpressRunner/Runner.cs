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
        public void RunTests(TestGroup testGroup)
        {
            Task.Factory.StartNew(() =>
            {
                testGroup.Run();
            });
        }
    }
}
