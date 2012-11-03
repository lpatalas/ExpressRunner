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

        public AssemblyTestGroup(TestAssembly assembly, string name, IEnumerable<Test> tests)
            : base(name, null)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.assembly = assembly;
            this.fileWatcher = new AssemblyFileWatcher(this, assembly.SourceFilePath);

            AddTests(tests);
        }

        public void Reload()
        {
            OnReloadStarting();

            if (!File.Exists(assembly.SourceFilePath))
            {
                IsMissing = true;
                Tests.Clear();
                OnReloadFinished();
            }
            else
            {
                IsMissing = false;

                Task.Factory.StartNew(() =>
                {
                    assembly.Reload();
                    UpdateReloadedItems();
                    Execute.OnUIThread(() => OnReloadFinished());
                });
            }
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

        public override void Run()
        {
            Run(Tests);
        }

        public void Run(IEnumerable<TestItem> testsToRun)
        {
            foreach (var test in testsToRun)
                test.ResetBeforeRun();
            assembly.RunTests(testsToRun);
        }

        private void Reset()
        {
            SubGroups.Clear();
            Tests.Clear();
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
                        matchedGroup = new TestGroup(part, this);
                        currentGroup.SubGroups.Add(matchedGroup);
                    }

                    matchedGroup.Tests.Add(testItem);
                    currentGroup = matchedGroup;
                }
            }
        }
    }
}
