using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniRunner.Api;
using Xunit;

namespace MiniRunner.XunitPlugin
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
            : base(framework)
        {
            this.assemblyFileName = assemblyFileName;

            Reload();
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
                var nestedClassName = className.Substring(nestedClassNameIndex + 1);
                return nestedClassName + " " + testMethod.MethodName.Replace("_", " ");
            }
            else
            {
                var nameIndex = className.LastIndexOf('.');
                if (nameIndex > 0)
                    className = className.Substring(nameIndex + 1);
                return className + " " + testMethod.MethodName.Replace('_', ' ');
            }
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
            var uniqueId = testMethod.DisplayName;

            return new Test(name, path, uniqueId);
        }

        public override void RunTests(IEnumerable<Test> tests)
        {
            var testRunner = new TestRunner(executorWrapper, new TestRunnerLogger(this, tests));
            testRunner.RunAssembly();
        }

        private class TestRunnerLogger : IRunnerLogger
        {
            private readonly XunitTestAssembly testAssembly;
            private readonly HashSet<string> testsToRun = new HashSet<string>();

            public TestRunnerLogger(XunitTestAssembly testAssembly, IEnumerable<Test> testsToRun)
            {
                this.testAssembly = testAssembly;
                this.testsToRun = new HashSet<string>(testsToRun.Select(c => c.UniqueId));
            }

            public void AssemblyFinished(string assemblyFilename, int total, int failed, int skipped, double time)
            {
            }

            public void AssemblyStart(string assemblyFilename, string configFilename, string xUnitVersion)
            {
            }

            public bool ClassFailed(string className, string exceptionType, string message, string stackTrace)
            {
                return true;
            }

            public void ExceptionThrown(string assemblyFilename, Exception exception)
            {
            }

            public void TestFailed(string name, string type, string method, double duration, string output, string exceptionType, string message, string stackTrace)
            {
                var test = testAssembly.GetTestByName(name);
                test.Status = Api.TestStatus.Failed;
            }

            public bool TestFinished(string name, string type, string method)
            {
                return true;
            }

            public void TestPassed(string name, string type, string method, double duration, string output)
            {
                var test = testAssembly.GetTestByName(name);
                test.Status = Api.TestStatus.Succeeded;
            }

            public void TestSkipped(string name, string type, string method, string reason)
            {
                var test = testAssembly.GetTestByName(name);
                test.Status = Api.TestStatus.NotRun;
            }

            public bool TestStart(string name, string type, string method)
            {
                return testsToRun.Contains(name);
            }
        }
    
}
}
