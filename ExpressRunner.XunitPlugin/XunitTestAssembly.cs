using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressRunner.Api;
using Xunit;

namespace ExpressRunner.XunitPlugin
{
    public class XunitTestAssembly : Api.TestAssembly
    {
        private string assemblyFileName;
        private IExecutorWrapper executorWrapper;
        private TestCollection tests = new TestCollection();

        public override IEnumerable<Test> Tests
        {
            get { return tests; }
        }

        public XunitTestAssembly(XunitFramework framework, string assemblyFileName)
            : base(assemblyFileName)
        {
            this.assemblyFileName = assemblyFileName;

            Reload();
        }

        public Test GetTestByMethod(string type, string method)
        {
            return tests[XunitHelper.GetTestUniqueId(type, method)];
        }

        public Test GetTestByName(string testName)
        {
            return tests[testName];
        }

        private static string FormatName(TestMethod testMethod)
        {
            var className = testMethod.TestClass.TypeName;
            var nestedClassNameIndex = className.IndexOf('+');
            if (nestedClassNameIndex > 0)
            {
                className = className.Substring(nestedClassNameIndex + 1);
            }
            else
            {
                var nameIndex = className.LastIndexOf('.');
                if (nameIndex > 0)
                    className = className.Substring(nameIndex + 1);
            }

            return FormatClassName(className) + " " + FormatMethodName(testMethod.MethodName);
        }

        private static string FormatClassName(string className)
        {
            if (className.EndsWith("Method"))
                return className.Substring(0, className.Length - 6) + " method";
            else if (className.EndsWith("Property"))
                return className.Substring(0, className.Length - 8) + " property";
            else if (className.EndsWith("Class"))
                return className.Substring(0, className.Length - 5) + " class";
            else if (className.EndsWith("Tests"))
                return className.Substring(0, className.Length - 5);
            else
                return className;
        }

        private static string FormatMethodName(string name)
        {
            name = name.Replace('_', ' ');
            if (name.StartsWith("Should") || name.StartsWith("Can"))
                name = char.ToLower(name[0]) + name.Substring(1);

            return name;
        }

        public override void Reload()
        {
            this.executorWrapper = new ExecutorWrapper(assemblyFileName, null, true);

            var testAssembly = TestAssemblyBuilder.Build(executorWrapper);

            tests.Clear();
            foreach (var testMethod in testAssembly.EnumerateTestMethods())
                tests.Add(CreateTest(testMethod));
        }

        private static Test CreateTest(TestMethod testMethod)
        {
            var name = FormatName(testMethod);
            var path = testMethod.TestClass.TypeName.Replace('.', '/').Replace('+', '/');
            var uniqueId = testMethod.GetUniqueId();

            return new Test(name, path, uniqueId);
        }

        public override void RunTests(IEnumerable<IRunnableTest> tests)
        {
            var testRunner = new TestRunner(executorWrapper, new TestRunnerLogger(this, tests));
            testRunner.RunAssembly();
        }
    }
}
