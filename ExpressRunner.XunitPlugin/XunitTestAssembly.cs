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
            : base(framework)
        {
            this.assemblyFileName = assemblyFileName;

            Reload();
        }

        public Test GetTestByMethod(string type, string method)
        {
            return tests[GetUniqueId(type, method)];
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
            var uniqueId = GetUniqueId(testMethod);

            return new Test(name, path, uniqueId);
        }

        private static string GetUniqueId(TestMethod testMethod)
        {
            return GetUniqueId(testMethod.TestClass.TypeName, testMethod.MethodName);
        }

        private static string GetUniqueId(string type, string method)
        {
            return type + '.' + method;
        }

        public override void RunTests(IEnumerable<IRunnableTest> tests)
        {
            var testRunner = new TestRunner(executorWrapper, new TestRunnerLogger(this, tests));
            testRunner.RunAssembly();
        }

        private class TestRunnerLogger : IRunnerLogger
        {
            private readonly XunitTestAssembly testAssembly;
            private readonly Dictionary<string, IRunnableTest> runningTests = new Dictionary<string, IRunnableTest>();

            public TestRunnerLogger(XunitTestAssembly testAssembly, IEnumerable<IRunnableTest> testsToRun)
            {
                this.testAssembly = testAssembly;

                foreach (var item in testsToRun)
                    runningTests.Add(item.Test.UniqueId, item);
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
                UpdateFailedTestStatus(name, type, method, exceptionType, message, stackTrace);
            }

            public bool TestFinished(string name, string type, string method)
            {
                return true;
            }

            public void TestPassed(string name, string type, string method, double duration, string output)
            {
                UpdateTestStatus(name, type, method, Api.TestStatus.Succeeded);
            }

            public void TestSkipped(string name, string type, string method, string reason)
            {
                UpdateTestStatus(name, type, method, Api.TestStatus.NotRun);
            }

            public bool TestStart(string name, string type, string method)
            {
                return ShouldRunTest(type, method);
            }

            private bool ShouldRunTest(string type, string method)
            {
                return runningTests.ContainsKey(XunitTestAssembly.GetUniqueId(type, method));
            }

            private void UpdateFailedTestStatus(string name, string type, string method, string exceptionType, string message, string stackTrace)
            {
                var test = runningTests[XunitTestAssembly.GetUniqueId(type, method)];
                var theoryArguments = ExtractTheoryArguments(name);
                string runName;

                if (!string.IsNullOrEmpty(theoryArguments))
                    runName = string.Format("{0} - {1}", theoryArguments, message);
                else
                    runName = message;

                test.RecordRun(new FailedTestRun(runName, stackTrace.Trim()));
            }

            private void UpdateTestStatus(string name, string type, string method, Api.TestStatus status)
            {
                var test = runningTests[XunitTestAssembly.GetUniqueId(type, method)];
                var theoryArguments = ExtractTheoryArguments(name);
                var runName = string.IsNullOrEmpty(theoryArguments)
                    ? status.ToString()
                    : theoryArguments + " - " + status;
                test.RecordRun(new TestRun(runName, status));
            }

            private string ExtractTheoryArguments(string name)
            {
                var argumentsIndex = name.IndexOf('(');
                if (argumentsIndex > 0)
                    return name.Substring(argumentsIndex + 1, name.Length - argumentsIndex - 2);

                return string.Empty;
            }
        }
    
}
}
