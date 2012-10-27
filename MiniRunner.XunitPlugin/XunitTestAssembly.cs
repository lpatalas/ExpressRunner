using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniRunner.Api;
using Xunit;

namespace MiniRunner.XunitPlugin
{
    public class XunitTestAssembly : ITestAssembly
    {
        private readonly IExecutorWrapper executorWrapper;
        private readonly IDictionary<string, TestCase> testByName = new Dictionary<string, TestCase>();
        private readonly IList<TestCase> testCases;

        public IEnumerable<TestCase> Tests
        {
            get { return testCases; }
        }

        public XunitTestAssembly(string assemblyFileName)
        {
            this.executorWrapper = new ExecutorWrapper(assemblyFileName, null, true);

            var testAssembly = TestAssemblyBuilder.Build(executorWrapper);

            foreach (var testMethod in testAssembly.EnumerateTestMethods())
            {
                testByName.Add(testMethod.DisplayName, CreateTestCase(testMethod));
            }

            this.testCases = testByName.Values.ToList().AsReadOnly();
        }

        private static TestCase CreateTestCase(TestMethod testMethod)
        {
            return new TestCase
            {
                Path = testMethod.TestClass.TypeName.Replace('.', '/').Replace('+', '/'),
                Name = FormatName(testMethod)
            };
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

        public TestCase GetTestCaseByName(string testName)
        {
            return testByName[testName];
        }

        public void RunTests()
        {
            var testRunner = new TestRunner(executorWrapper, new TestRunnerLogger(this));
            testRunner.RunAssembly();
        }

        private class TestRunnerLogger : IRunnerLogger
        {
            private readonly XunitTestAssembly testAssembly;

            public TestRunnerLogger(XunitTestAssembly testAssembly)
            {
                if (testAssembly == null)
                    throw new ArgumentNullException("testAssembly");
                this.testAssembly = testAssembly;
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
                var test = testAssembly.GetTestCaseByName(name);
                test.Status = Api.TestStatus.Failed;
            }

            public bool TestFinished(string name, string type, string method)
            {
                return true;
            }

            public void TestPassed(string name, string type, string method, double duration, string output)
            {
                var test = testAssembly.GetTestCaseByName(name);
                test.Status = Api.TestStatus.Succeeded;
            }

            public void TestSkipped(string name, string type, string method, string reason)
            {
                var test = testAssembly.GetTestCaseByName(name);
                test.Status = Api.TestStatus.NotRun;
            }

            public bool TestStart(string name, string type, string method)
            {
                return true;
            }
        }
    
}
}
