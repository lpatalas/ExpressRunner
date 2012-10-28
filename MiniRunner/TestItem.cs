using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MiniRunner.Api;

namespace MiniRunner
{
    public class TestItem : PropertyChangedBase
    {
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
    }
}
