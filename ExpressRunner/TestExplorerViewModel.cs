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
            get { return runner.TestGroups; }
        }

        [ImportingConstructor]
        public TestExplorerViewModel([Import] Runner runner)
        {
            this.runner = runner;
        }

        public void OpenAssembly()
        {
            if (openDialog.ShowDialog() == true)
            {
                runner.LoadTests(openDialog.FileName);
            }
        }

        public void ReloadAssemblies()
        {
            runner.ReloadAssemblies();
        }

        public void RemoveAssembly(AssemblyTestGroup assembly)
        {
            runner.TestGroups.Remove(assembly);
        }

        public void RunTests()
        {
            if (SelectedTestGroup != null)
                runner.RunTests(SelectedTestGroup);
        }

        public void OnSelectedTestGroupChanged(RoutedPropertyChangedEventArgs<object> eventArgs)
        {
            SelectedTestGroup = (TestGroup)eventArgs.NewValue;
        }
    }
}
