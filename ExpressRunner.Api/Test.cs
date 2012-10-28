using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRunner.Api
{
    public class Test
    {
        private readonly string name = string.Empty;
        public string Name
        {
            get { return name; }
        }

        private readonly string path = string.Empty;
        public string Path
        {
            get { return path; }
        }

        private readonly string uniqueId;
        public string UniqueId
        {
            get { return uniqueId; }
        }

        public Test(string name, string path, string uniqueId)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (string.IsNullOrEmpty(uniqueId))
                throw new ArgumentNullException("uniqueId");

            this.name = name;
            this.path = path;
            this.uniqueId = uniqueId;
        }
    }
}
