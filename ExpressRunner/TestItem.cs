using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ExpressRunner.Api;

namespace ExpressRunner
{
    public class TestItem : PropertyChangedBase, IRunnableTest
    {
        private TimeSpan duration;
        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    NotifyOfPropertyChange(() => Duration);
                }
            }
        }

        private bool isActual;
        public bool IsActual
        {
            get { return isActual; }
            set
            {
                if (isActual != value)
                {
                    isActual = value;
                    NotifyOfPropertyChange(() => IsActual);
                }
            }
        }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }

        public string Name
        {
            get { return Test.Name; }
        }

        private readonly BindableCollection<TestRun> runs = new BindableCollection<TestRun>();
        public IObservableCollection<TestRun> Runs
        {
            get { return runs; }
        }

        private TestStatus status = TestStatus.NotRun;
        public TestStatus Status
        {
            get { return status; }
            private set
            {
                if (status != value)
                {
                    status = value;
                    NotifyOfPropertyChange(() => Status);
                }
            }
        }

        private readonly Test test;
        public Test Test
        {
            get { return test; }
        }

        public TestItem(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            this.test = test;
        }

        public void ResetBeforeRun()
        {
            IsActual = false;
        }

        public void RecordRun(TestRun run)
        {
            if (!IsActual)
            {
                Duration = run.Duration;
                runs.Clear();
                runs.Add(run);
                Status = run.Status;
                IsActual = true;
            }
            else
            {
                Duration += run.Duration;
                UpdateStatus(run);
                runs.Add(run);
            }
        }

        private void UpdateStatus(TestRun run)
        {
            if (run.Status == TestStatus.Failed)
                Status = TestStatus.Failed;
            else if (Status == TestStatus.NotRun)
                Status = run.Status;
        }
    }
}
