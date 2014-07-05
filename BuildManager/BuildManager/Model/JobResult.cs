using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace BuildManager.Model
{

    public enum JobResultStatus
    {
        NotStarted ,
        Scheduled ,
        Waiting ,
        Started ,
        Completed,
        Error ,
        Aborted ,
        BadSchedule
    }


    public class JobResult : INotifyPropertyChanged
    {
        private Int32 jobID;

        public Int32 JobID 
        { 
            get { return this.jobID;}

            set
            {
                this.jobID = value;
                this.OnPropertyChanged("JobID");
            }
        }

       private DateTime startTime;
       
    

       public DateTime StartTime
       {
           get { return this.startTime; }
           set
           {
               this.startTime = value;
               this.OnPropertyChanged("StartTime");
           }
       }

       private DateTime stopTime;

       public DateTime StopTime
       {
           get { return this.stopTime; }
           set
           {
               this.stopTime = value;
               this.OnPropertyChanged("StopTime");
           }
       }

       private string enlistmentName;

       public string EnlistmentName
       {
           get { return this.enlistmentName; }
           set
           {
               this.enlistmentName = value;
               this.OnPropertyChanged("EnlistmentName");
           }
       }

       private string command;


       public string Command
       {
           get { return this.command; }
           set
           {
               this.command = value;
               this.OnPropertyChanged("Command");
           }
       }

       private string status;

       public string Status
       {
           get { return this.status; }
           set
           {
               
               string yellow = Colors.Yellow.ToString();
               string orangeRed = Colors.OrangeRed.ToString();
               this.status = value;
               JobResultStatus s = (JobResultStatus)Enum.Parse(typeof(JobResultStatus), this.status);
               switch ( s)
               {
                   case JobResultStatus.NotStarted:
                       this.StatusColor = "Black";
                       break;
                   case JobResultStatus.Scheduled:
                       this.StatusColor = Colors.OrangeRed.ToString();
                       break;
                   case JobResultStatus.Waiting:
                       this.StatusColor = Colors.Turquoise.ToString();
                       break;
                   case JobResultStatus.Completed:
                       this.StatusColor = Colors.Green.ToString();
                       break;
                   case JobResultStatus.Error:
                   case JobResultStatus.BadSchedule:
                       this.StatusColor = Colors.Red.ToString();
                       break;
                   case JobResultStatus.Aborted:
                       this.StatusColor = Colors.Orange.ToString();
                       break;
                   case JobResultStatus.Started:
                       this.StatusColor = Colors.Blue.ToString();
                       break;
                   default:
                       this.StatusColor = "Black";
                       break;
               }

               this.OnPropertyChanged("Status");
           }
       }

        private string statusColor;


        public string StatusColor
       {
            get 
            {
                return statusColor;
            }
            set
           {
               this.statusColor = value;
               this.OnPropertyChanged("StatusColor");
           }
       }

       private string phoneSku;

       public string PhoneSku
       {
           get { return this.phoneSku; }
           set
           {
               this.phoneSku = value;
               this.OnPropertyChanged("PhoneSku");
           }
       }

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

    }
}
