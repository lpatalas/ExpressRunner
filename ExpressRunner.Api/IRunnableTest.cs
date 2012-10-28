using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRunner.Api
{
    public interface IRunnableTest
    {
        Test Test { get; }
        void RecordRun(TestStatus status);
    }
}
