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

        private readonly string name = string.Empty;
        public string Name
        {
            get { return name; }
        }

        private readonly string path = string.Empty;
        public string Path
        {
            get { return path; }
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

        public Test(string name, string path, string uniqueId)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (string.IsNullOrEmpty(uniqueId))
                throw new ArgumentNullException("uniqueId");

            this.name = name;
            this.path = path;
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
