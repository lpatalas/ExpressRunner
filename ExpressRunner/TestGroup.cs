using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    public class TestGroup : PropertyChangedBase
    {
        private readonly string name;
        private readonly TestGroup parentGroup;
        private readonly BindableCollection<TestGroup> subGroups;
        private readonly BindableCollection<TestItem> tests;

        private bool? isAutoRunEnabled = true;
        public bool? IsAutoRunEnabled
        {
            get { return isAutoRunEnabled; }
            set { SetIsAutoRunEnabledValue(value, true, true); }
        }

        private void SetIsAutoRunEnabledValue(bool? newValue, bool updateTests, bool updateParent)
        {
            if (newValue != IsAutoRunEnabled)
            {
                isAutoRunEnabled = newValue;
                NotifyOfPropertyChange(() => IsAutoRunEnabled);

                if (newValue.HasValue && updateTests)
                {
                    foreach (var item in Tests)
                        item.IsEnabled = newValue.Value;
                }

                foreach (var subGroup in subGroups)
                    subGroup.SetIsAutoRunEnabledValue(newValue, false, false);

                if (updateParent && parentGroup != null)
                    parentGroup.CalculateIsAutoRunEnabledValueFromSubGroupsState();
            }
        }

        private void CalculateIsAutoRunEnabledValueFromSubGroupsState()
        {
            var firstSubGroupState = subGroups[0].IsAutoRunEnabled;
            bool? newValue;

            if (subGroups.Skip(1).Any(subGroup => subGroup.IsAutoRunEnabled != firstSubGroupState))
                newValue = null;
            else
                newValue = firstSubGroupState;

            if (newValue != IsAutoRunEnabled)
            {
                isAutoRunEnabled = newValue;
                NotifyOfPropertyChange(() => IsAutoRunEnabled);

                if (parentGroup != null)
                    parentGroup.CalculateIsAutoRunEnabledValueFromSubGroupsState();
            }
        }

        public string Name
        {
            get { return name; }
        }

        public IList<TestGroup> SubGroups
        {
            get { return subGroups; }
        }

        public IObservableCollection<TestItem> Tests
        {
            get { return tests; }
        }

        public TestGroup(string name, TestGroup parentGroup)
            : this(name, parentGroup, Enumerable.Empty<TestGroup>(), Enumerable.Empty<Test>())
        {
        }

        public TestGroup(string name, TestGroup parentGroup, IEnumerable<TestGroup> subGroups, IEnumerable<Test> tests)
        {
            this.name = name;
            this.parentGroup = parentGroup;
            this.subGroups = new BindableCollection<TestGroup>(subGroups);
            this.tests = new BindableCollection<TestItem>(tests.Select(test => new TestItem(test)));
        }

        public virtual Task RunAsync()
        {
            var parentAssembly = GetParentAssemblyTestGroup();
            return parentAssembly.RunAsync(GetEnabledTests());
        }

        protected IEnumerable<TestItem> GetEnabledTests()
        {
            return Tests
                .Where(item => item.IsEnabled)
                .ToList()
                .AsReadOnly();
        }

        private AssemblyTestGroup GetParentAssemblyTestGroup()
        {
            for (var group = this; group != null; group = group.parentGroup)
            {
                var assemblyGroup = group as AssemblyTestGroup;
                if (assemblyGroup != null)
                    return assemblyGroup;
            }

            throw new InvalidOperationException("Can't find parent AssemblyTestGroup for group: " + Name);
        }

        protected void RemoveEmptySubGroups()
        {
            for (int i = subGroups.Count - 1; i >= 0; i--)
            {
                if (subGroups[i].Tests.Count == 0)
                    subGroups.RemoveAt(i);
                else
                    subGroups[i].RemoveEmptySubGroups();
            }
        }

        protected void RemoveTestItems(IEnumerable<TestItem> itemsToRemove)
        {
            Tests.RemoveRange(itemsToRemove);
            foreach (var subGroup in SubGroups)
                subGroup.RemoveTestItems(itemsToRemove);
        }
    }
}
