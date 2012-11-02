using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExpressRunner.XunitPlugin
{
    public static class XunitHelper
    {
        public static string GetUniqueId(this TestMethod testMethod)
        {
            return XunitHelper.GetTestUniqueId(testMethod.TestClass.TypeName, testMethod.MethodName);
        }

        public static string GetTestUniqueId(string type, string method)
        {
            return type + '.' + method;
        }
    }
}
