using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Win32;
using ExpressRunner.Api;

namespace ExpressRunner
{
    [Export]
    public class TestExplorerViewModel : PropertyChangedBase
    {
        private readonly OpenFileDialog openDialog = new OpenFileDialog
        {
            Filter = "Test Assembly Files (*.dll; *.exe)|*.dll;*.exe",
            Title = "Open test assembly..."
        };

        private readonly Runner runner;
        private readonly TestRepository testRepository;

        private TestGroup selectedTestGroup;
        public TestGroup SelectedTestGroup
        {
            get { return selectedTestGroup; }
            set
            {
                if (selectedTestGroup != value)
                {
                    selectedTestGroup = value;
                    NotifyOfPropertyChange(() => SelectedTestGroup);
                }
            }
        }

        public IObservableCollection<AssemblyTestGroup> TestGroups
        {
            get { return testRepository.TestGroups; }
        }

        [ImportingConstructor]
        public TestExplorerViewModel(
            [Import] Runner runner,
            [Import] TestRepository testRepository)
        {
            if (runner == null)
                throw new ArgumentNullException("runner");
            if (testRepository == null)
                throw new ArgumentNullException("testRepository");

            this.runner = runner;
            this.testRepository = testRepository;
        }

        public void OpenAssembly()
        {
            if (openDialog.ShowDialog() == true)
            {
                testRepository.LoadTests(openDialog.FileName);
            }
        }

        public async void ReloadAllAssemblies()
        {
            await testRepository.ReloadAssembliesAsync();
        }

        public async void ReloadAssembly(AssemblyTestGroup assembly)
        {
            await assembly.ReloadAsync();
        }

        public void RemoveAssembly(AssemblyTestGroup assembly)
        {
            testRepository.TestGroups.Remove(assembly);
        }

        public void RunTests(TestGroup assembly)
        {
            runner.RunTests(assembly);
        }

        public void OnSelectedTestGroupChanged(RoutedPropertyChangedEventArgs<object> eventArgs)
        {
            SelectedTestGroup = (TestGroup)eventArgs.NewValue;
        }
    }
}
