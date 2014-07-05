using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BuildManager.Model
{
    public class JobLog : ObservableCollection<string>
    {

        public object lockObject = new object();

        public JobLog()
        {
        }
    }
}
