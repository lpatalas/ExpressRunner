﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressRunner.Api;
using Xunit;

namespace ExpressRunner.XunitPlugin
{
    public class TestRunnerLogger : IRunnerLogger
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
            return runningTests.ContainsKey(XunitHelper.GetTestUniqueId(type, method));
        }

        private void UpdateFailedTestStatus(string name, string type, string method, string exceptionType, string message, string stackTrace)
        {
            var test = runningTests[XunitHelper.GetTestUniqueId(type, method)];
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
            var test = runningTests[XunitHelper.GetTestUniqueId(type, method)];
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
