using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Windows;

namespace MediaFileSorter.Model
{
    public class AppSettings : INotifyPropertyChanged
    {


        private string filePathForAutoUpdate = String.Empty;

        private Int32 startingNumericalSequenceNumber;

        public Int32 StartingNumericalSequenceNumber
        {
            get { return this.startingNumericalSequenceNumber; }
            set
            {
                this.startingNumericalSequenceNumber = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("StartingNumericalSequenceNumber");
            }
        }


        private bool applyRecursively;

        public bool ApplyRecursively
        {
            get { return this.applyRecursively; }
            set
            {
                this.applyRecursively = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("ApplyRecusively");
            }
        }

        private bool correctForNumericalSort;

        public bool CorrectForNumericalSort
        {
            get { return this.correctForNumericalSort; }
            set
            {
                this.correctForNumericalSort = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("CorrectForNumericalSort");
            }
        }

        private bool createNumericalListFromRawImages;

        public bool CreateNumericalListFromRawImages
        {
            get { return this.createNumericalListFromRawImages; }
            set
            {
                this.createNumericalListFromRawImages = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("CreateNumericalListFromRawImages");
            }
        }

        private bool exactMatch;

        public bool ExactMatch
        {
            get { return this.exactMatch; }
            set
            {
                this.exactMatch = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("ExactMatch");
            }
        }

        private string sourceFolder;

        public string SourceFolder
        {
            get { return this.sourceFolder; }
            set
            {
                this.sourceFolder = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("SourceFolder");
            }
        }

        private string targetFolder;

        public string TargetFolder
        {
            get { return this.targetFolder; }
            set
            {
                this.targetFolder = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("TargetFolder");
            }
        }

        private string prefix;

        public string Prefix
        {
            get { return this.prefix; }
            set
            {
                this.prefix = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("Prefix");
            }
        }

        private string mainString;

        public string MainString
        {
            get { return this.mainString; }
            set
            {
                this.mainString = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("MainString");
            }
        }

        private string searchSource;

        public string SearchSource
        {
            get { return this.searchSource; }
            set
            {
                this.searchSource = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("SearchSource");
            }
        }

        private string searchTarget;

        public string SearchTarget
        {
            get { return this.searchTarget; }
            set
            {
                this.searchTarget = value;
                this.UpdateDisplayCalibrationConfigurationFile();
                this.OnPropertyChanged("SearchTarget");
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
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, args);
            }
        }




        public void Save(string fileName)
        {
            using (TextWriter writer = new StreamWriter(fileName))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(AppSettings));
                xmlSerializer.Serialize(writer, this);
                writer.Close();
            }
        }

        public static  AppSettings Load(string fileName)
        {
            AppSettings temp = null;
            XmlSerializer mySerializer = new XmlSerializer(typeof(AppSettings));
            if (File.Exists(fileName))
            {
                using (TextReader myReader = new StreamReader(fileName))
                {
                    temp = (AppSettings)mySerializer.Deserialize(myReader);
                }
            }
            else
            {
                temp = new AppSettings();
                temp.InitializeEmptyObject();
                temp.Save(fileName);
            }
            return temp;

        }

        public void InitializeEmptyObject()
        {
            this.SourceFolder = @"C:\Tmp\Photos\Source";
            this.TargetFolder = @"C:\Tmp\Photos\Target";
            this.ApplyRecursively = true;
            this.CorrectForNumericalSort = true;
            this.CreateNumericalListFromRawImages = true;
            this.ExactMatch = true;
            this.Prefix = "Prefix";
            this.MainString = "Main";
            this.SearchSource = "Image";
            this.SearchTarget = "Cannon";
        }


        public void SetAutoFileSavePath(string filePath)
        {
            this.filePathForAutoUpdate = filePath;
        }


        private void UpdateDisplayCalibrationConfigurationFile()
        {
            if (!string.IsNullOrEmpty(this.filePathForAutoUpdate))
            {
                this.Save(this.filePathForAutoUpdate);
            }
        }


    }
}
