using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressRunner.Api
{
    public class Test : IEquatable<Test>
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

        public bool Equals(Test other)
        {
            return (object)other != null
                && UniqueId.Equals(other.UniqueId, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Test;
            return (object)other != null && other.Equals(this);
        }

        public override int GetHashCode()
        {
            return uniqueId.GetHashCode();
        }

        public override string ToString()
        {
            return uniqueId;
        }

        public static bool operator ==(Test first, Test second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else if ((object)first != null)
                return first.Equals(second);
            else
                return false;
        }

        public static bool operator !=(Test first, Test second)
        {
            return !(first == second);
        }
    }
}
