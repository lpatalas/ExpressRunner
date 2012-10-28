using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRunner.Api
{
    public class Test : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyOfPropertyChange("Name");
                }
            }
        }

        private string path = string.Empty;
        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    NotifyOfPropertyChange("Path");
                }
            }
        }

        private TestStatus status = TestStatus.NotRun;
        public TestStatus Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    NotifyOfPropertyChange("Status");
                }
            }
        }

        private readonly string uniqueId;
        public string UniqueId
        {
            get { return uniqueId; }
        }

        public Test(string uniqueId)
        {
            if (string.IsNullOrEmpty(uniqueId))
                throw new ArgumentNullException("uniqueId");
            this.uniqueId = uniqueId;
        }

        protected void NotifyOfPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
