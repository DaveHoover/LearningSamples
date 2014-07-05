using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.ComponentModel;
using BuildManager.Data;
using BuildManager.Model;
using System.Diagnostics;
using System.Threading;

namespace BuildManager.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        public const string LogFileBaseName = "BuildManagerLog_";

        public EnlistmentDC dc = Shared.EnlistmentDataContext;




      

        public JobResults Results { get; set; }

        public JobLog Log { get; set; }


        public IEnumerable<Configuration> SelectedOperationSet { get; set; }

        public IEnumerable<Configuration> JobSet { get; set; }


        public JobSetInformation RunningJobs;

        public string[] ProcessPriorities { get; set; }







      

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Standard pattern for data binding and notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Notify subscribers of a change in the property
        /// </summary>
        /// <param name="propertyName">Name of the property to signal there has been a changed</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, args);
            }
        }



        public MainWindowViewModel()
        {
         
            Results = new JobResults();
            Log = new JobLog();
            this.ProcessPriorities = Enum.GetNames(typeof(ProcessPriorityClass));
        }


        internal string CreateLogFileName()
        {
            DateTime current = DateTime.Now;
            string fileName = LogFileBaseName + current.ToShortDateString() + "_" + current.ToShortTimeString() + ".txt";
            fileName = fileName.Replace(':', '_');
            fileName = fileName.Replace('/', '_');

            return fileName;
        }


        public void AddItemsToResults ()
        {
            Results.Add(new JobResult { JobID = 1, Command = "Wm Sync", EnlistmentName = @"F:\SevenApp", StartTime = DateTime.Now, StopTime = DateTime.Now, Status = "Not Running" });
            Results.Add(new JobResult { JobID = 2, Command = "Wm Sync1", EnlistmentName = @"F:\SevenApp1", StartTime = DateTime.Now, StopTime = DateTime.Now, Status = "Not Running" });

            Log.Add("Entry #1");
            Log.Add("Entry #2");

        }
        public void ModifyResults()
        {
            Results[0].Status = "Running";
            Results[1].Status = "Completed";
            Log.Add("Entry #3");
        }

    }
}
