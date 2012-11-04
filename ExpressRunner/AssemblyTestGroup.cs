using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    public class AssemblyTestGroup : TestGroup
    {
        private readonly TestAssembly assembly;
        private readonly IEventAggregator eventAggregator;
        private readonly AssemblyFileWatcher fileWatcher;

        private bool isMissing;
        public bool IsMissing
        {
            get { return isMissing; }
            private set
            {
                if (isMissing != value)
                {
                    isMissing = value;
                    NotifyOfPropertyChange(() => IsMissing);
                }
            }
        }

        public event EventHandler ReloadStarting;
        public event EventHandler ReloadFinished;

        public AssemblyTestGroup(TestAssembly assembly, IEventAggregator eventAggregator)
            : base(FormatGroupName(assembly), null)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (eventAggregator == null)
                throw new ArgumentNullException("eventAggregator");

            this.assembly = assembly;
            this.eventAggregator = eventAggregator;
            this.fileWatcher = new AssemblyFileWatcher(this, assembly.SourceFilePath);

            AddTests(assembly.Tests);
        }

        private void AddTests(IEnumerable<Test> tests)
        {
            var testItems = tests.Select(test => new TestItem(test));

            foreach (var testItem in testItems)
            {
                this.Tests.Add(testItem);
                var groups = testItem.Test.Path.Split('/');

                TestGroup currentGroup = this;

                foreach (var part in groups)
                {
                    var matchedGroup = currentGroup.SubGroups.FirstOrDefault(subGroup => subGroup.Name.Equals(part));
                    if (matchedGroup == null)
                    {
                        matchedGroup = new TestGroup(part, currentGroup);
                        currentGroup.SubGroups.Add(matchedGroup);
                    }

                    matchedGroup.Tests.Add(testItem);
                    currentGroup = matchedGroup;
                }
            }
        }

        private static string FormatGroupName(TestAssembly assembly)
        {
            return Path.GetFileName(assembly.SourceFilePath);
        }

        public async Task ReloadAsync()
        {
            OnReloadStarting();

            IsMissing = !File.Exists(assembly.SourceFilePath);
            if (IsMissing)
                Tests.Clear();
            else
                await ReloadAssemblyAsync();

            OnReloadFinished();
        }

        private Task ReloadAssemblyAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                assembly.Reload();
                UpdateReloadedItems();
            });
        }

        private void OnReloadStarting()
        {
            EventHandler handler = ReloadStarting;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnReloadFinished()
        {
            EventHandler handler = ReloadFinished;
            if (handler != null)
                handler(this, EventArgs.Empty);

            eventAggregator.Publish(new AssemblyReloadedEvent(this));
        }

        private void UpdateReloadedItems()
        {
            var itemsToRemove = new List<TestItem>();

            foreach (var item in Tests)
            {
                if (!assembly.Tests.Any(test => test == item.Test))
                    itemsToRemove.Add(item);
            }

            var testsToAdd = new List<Test>();
            foreach (var test in assembly.Tests)
            {
                if (!Tests.Any(item => item.Test == test))
                    testsToAdd.Add(test);
            }

            RemoveTestItems(itemsToRemove);
            MarkTestsAsUnactual();
            AddTests(testsToAdd);
            RemoveEmptySubGroups();
        }

        private void MarkTestsAsUnactual()
        {
            foreach (var item in Tests)
                item.IsActual = false;
        }

        public override Task RunAsync()
        {
            return RunAsync(GetEnabledTests());
        }

        public Task RunAsync(IEnumerable<TestItem> testsToRun)
        {
            OnRunStarting();

            return Task.Factory.StartNew(() =>
            {
                foreach (var test in testsToRun)
                    test.ResetBeforeRun();
                assembly.RunTests(testsToRun);

                Execute.OnUIThread(() => OnRunFinished(testsToRun));
            });
        }

        private void OnRunStarting()
        {
            eventAggregator.Publish(new RunStartingEvent());
        }

        private void OnRunFinished(IEnumerable<TestItem> finishedTests)
        {
            var aggregatedStatus = GetAggregatedStatus(finishedTests);
            eventAggregator.Publish(new RunFinishedEvent(aggregatedStatus));
        }

        private static TestStatus GetAggregatedStatus(IEnumerable<TestItem> runItems)
        {
            if (runItems.Any(item => item.Status == TestStatus.Failed))
                return TestStatus.Failed;

            return TestStatus.Succeeded;
        }
    }
}
