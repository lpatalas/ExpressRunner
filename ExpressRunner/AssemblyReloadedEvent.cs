using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner
{
    public class AssemblyReloadedEvent
    {
        private readonly AssemblyTestGroup assembly;
        public AssemblyTestGroup Assembly
        {
            get { return assembly; }
        }

        public AssemblyReloadedEvent(AssemblyTestGroup assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            this.assembly = assembly;
        }
    }
}
