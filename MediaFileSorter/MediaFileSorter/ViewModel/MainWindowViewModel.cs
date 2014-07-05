using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MediaFileSorter.Model;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MediaFileSorter.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        internal string numberMatchPattern   = "[0-9]+";

        private AppSettings config;

        public AppSettings Config
        {
            get { return this.config; }
            set
            {
                this.config = value;
                this.OnPropertyChanged("Config");
            }
        }


        internal FileListCollection SourceFileList { get; set; }

        internal FileListCollection TargetFileList { get; set; }

        internal List<string> selectedFileList = new List<string>();

        internal bool SelectedItemsFromUI  {get; set;}



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

        public MainWindowViewModel()
        {
            this.SelectedItemsFromUI = false;
            this.Config = new AppSettings();
            this.Config = AppSettings.Load("Config.xml");
            this.Config.SetAutoFileSavePath("Config.xml");
            this.SourceFileList = new FileListCollection();
            this.TargetFileList = new FileListCollection();
        }

        #region Folder Selection and Check for existing Source/Target Folder

        /// <summary>
        /// Helper method to select a folder for the source/target location
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal string SelectFolder (bool source )
        {
            string directory = string.Empty;
            string startingFolder = source ? this.Config.SourceFolder : this.Config.TargetFolder;
            System.Windows.Forms.FolderBrowserDialog f = new System.Windows.Forms.FolderBrowserDialog();
            f.Description =  source ? "Select the source folder for media files" :
                                      "Select the target folder for converted media file names";
            if (startingFolder != String.Empty)
            {
                f.SelectedPath = startingFolder;
            }
            System.Windows.Forms.DialogResult r = f.ShowDialog();
            if (r == System.Windows.Forms.DialogResult.OK)
            {

                directory = f.SelectedPath;
                if (source)
                {
                    this.Config.SourceFolder = directory;
                }
                else
                {
                    this.Config.TargetFolder = directory;
                }
            }

            return directory;

        }

        private bool CheckForExistingSourceFolder(bool unitTest)
        {
            if (Directory.Exists(this.Config.SourceFolder))
            {
                return true;
            }
            else
            {
                if (!unitTest)
                {
                    System.Windows.Forms.MessageBox.Show("Selected Source Folder does not exist. No operation attempted ");
                }
                return false;
            }
        }

        private bool CheckForExistingTargetFolder()
        {
            if (this.Config.SourceFolder != this.Config.TargetFolder)
            {
                try
                {
                    if (Directory.Exists(this.Config.TargetFolder))
                    {
                        Directory.Delete(this.Config.TargetFolder, true);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Could Not delete target folder path. Error was " +
                        e.Message);
                    return false;
                }
            }
            return true;
        }

        #endregion



        /// <summary>
        /// Rescan the source/Target folders and update the list boxes
        /// </summary>
        /// <param name="unitTest"></param>
        internal void RescanButton(bool unitTest)
        {
            string[] files = null;
            SearchOption opt = this.Config.ApplyRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            if (!this.CheckForExistingSourceFolder(unitTest)) return;
            files = Directory.GetFiles(this.Config.SourceFolder, "*.*", opt);
            this.SourceFileList.Clear();
            foreach (string f in files)
            {
                this.SourceFileList.Add(f);
            }

            if (this.Config.SourceFolder == this.Config.TargetFolder)
            {
                foreach (string f in files)
                {
                    this.TargetFileList.Add(f);
                }
            }
            else
            {
                if (Directory.Exists(this.Config.TargetFolder))
                {
                    files = Directory.GetFiles(this.Config.TargetFolder, "*.*", opt);
                    this.TargetFileList.Clear();
                    foreach (string f in files)
                    {
                        this.TargetFileList.Add(f);
                    }
                }
            }
        }


        /// <summary>
        /// Execute the Search/Replace functionality
        /// </summary>
        /// <param name="unitTest"></param>
        internal void SearchUpdate(bool unitTest)
        {
            SearchOption opt = this.Config.ApplyRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] files = null;
            if (!this.CheckForExistingSourceFolder(unitTest)) return;
            if (this.SelectedItemsFromUI)
            {
                files = this.selectedFileList.ToArray<string>();
            }
            else
            {
                if (!this.CheckForExistingTargetFolder())
                {
                    return;
                }
                files = Directory.GetFiles(this.Config.SourceFolder, "*.*", opt);
            }
            
            bool srcTargetEqual = this.Config.SourceFolder == this.Config.TargetFolder ? true : false;
            foreach (string f in files)
            {
                string targetFile = string.Empty;
                string fileNameBase = Path.GetFileName(f);
                string newFileNameBase = fileNameBase.Replace(this.Config.SearchSource, this.Config.SearchTarget);
                if (!srcTargetEqual)
                {
                    targetFile = f.Replace(this.Config.SourceFolder, this.Config.TargetFolder);
                    if (this.SelectedItemsFromUI)
                    {
                        // Need to remove the original file if it already exists
                        string origFileToDelete = Path.Combine(Path.GetDirectoryName(targetFile), fileNameBase);
                        if (File.Exists(origFileToDelete))
                        {
                            File.Delete(origFileToDelete);
                        }
                    }
                    targetFile = Path.Combine(Path.GetDirectoryName(targetFile), newFileNameBase);
                    string newDir = Path.GetDirectoryName(targetFile);
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                    if (this.SelectedItemsFromUI)
                    {
                        if (File.Exists(targetFile))
                        {
                            File.Delete(targetFile);
                        }
                    }
                    File.Copy(f, targetFile);
                }
                else
                {
                    targetFile = Path.Combine(Path.GetDirectoryName(f), newFileNameBase);
                    Directory.Move(f, targetFile);
                }
            }
            this.RescanButton(unitTest);
        }


        /// <summary>
        /// Execute the loop to create a numerical list from a set of input images.
        /// Note: The image numerical sequence will continue across all entries and wil
        /// not reset for each directory.
        /// </summary>
        /// <param name="unitTest"></param>
        internal void CreateNumericalList(bool unitTest)
        {
            SearchOption opt = this.Config.ApplyRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] files = null;
            if (!this.CheckForExistingSourceFolder(unitTest)) return;
            if (this.SelectedItemsFromUI)
            {
                files = this.selectedFileList.ToArray<string>();
            }
            else
            {
                if (!this.CheckForExistingTargetFolder())
                {
                    return;
                }
                files = Directory.GetFiles(this.Config.SourceFolder, "*.*", opt);
            }
            Int32 numberOfFiles = files.Count();
            int numberOfDigits = this.FindNumberOfDigits(numberOfFiles, 1);
            string formatString = "d"+ (numberOfDigits + 1).ToString();
            Int32 i = this.Config.StartingNumericalSequenceNumber;
            bool srcTargetEqual = this.Config.SourceFolder == this.Config.TargetFolder ? true : false;
            foreach (string f in files)
            {
                string targetFile = string.Empty;
                string fileNameBase = this.Config.Prefix + i.ToString(formatString) + this.Config.MainString;
                i++;
                if (!srcTargetEqual)
                {
                    targetFile = f.Replace(this.Config.SourceFolder, this.Config.TargetFolder);
                    targetFile = Path.Combine(Path.GetDirectoryName(targetFile), fileNameBase);
                    string newDir = Path.GetDirectoryName(targetFile);
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                    if (this.SelectedItemsFromUI)
                    {
                        if (File.Exists(targetFile))
                        {
                            File.Delete(targetFile);
                        }
                    }
                    File.Copy(f, targetFile);
                }
                else
                {
                    targetFile = Path.Combine(Path.GetDirectoryName(f), fileNameBase);
                    Directory.Move(f, targetFile);
                }
            }
            this.RescanButton(unitTest);
        }

        #region Modify the Numerical sorting to fix problems with items that do not have leading zeros




        internal void UpdateNames(bool unitTest)
        {

            this.CorrectForNumericalSort(unitTest);

            // Not supporting the check box at this point
            //if (this.Config.CorrectForNumericalSort)
            //{
            //    this.CorrectForNumericalSort(unitTest);
            //}
        }


        internal void CorrectForNumericalSort(bool unitTest)
        {
            string directory = this.Config.SourceFolder;
            if (!this.CheckForExistingSourceFolder(unitTest)) return;
            if (!this.CheckForExistingTargetFolder()) return;
            // Update the files in the root input directory
            this.UpdateFileNumbering(directory);
            if (this.Config.ApplyRecursively)
            {
                this.UpdateFileNumberingRecursively(directory);
            }
            this.RescanButton(unitTest);
        }

        /// <summary>
        /// Update the numbering for files in all subdirectories of the current directory
        /// </summary>
        /// <param name="directory"></param>
        private void UpdateFileNumberingRecursively ( string directory)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(directory))
                {
                    this.UpdateFileNumbering(d);
                    UpdateFileNumberingRecursively(d);
                }
                
            }
            catch (System.Exception excpt)
            {
                Debug.WriteLine(excpt.Message);
            }

        }

        /// <summary>
        /// Update the file numbering in the selected directory.
        /// 
        /// </summary>
        /// <param name="directory"></param>
        private void UpdateFileNumbering (string directory)
        {

            string[] files = Directory.GetFiles(directory);
            List<Int32> fileNumbers = new List<Int32>();
            List<string> fileNumberStrings = new List<string>();
            // Extract an existing numerical value from each file. 
            // Create a list of the strings/and numbers for the matching entries
            foreach (string f in files)
            {
                string baseFileName = Path.GetFileNameWithoutExtension(f);
               
                Match m = Regex.Match(baseFileName, this.numberMatchPattern);
                if (m.Success)
                {
                    Int32 number = 0;
                    if (Int32.TryParse(m.Value, out number))
                    {
                        fileNumberStrings.Add(m.Value);
                        fileNumbers.Add(number);
                    }
                }
            }
            int max = fileNumbers.Max();
            int min = fileNumbers.Min();
            int numberOfDigits = this.FindNumberOfDigits(max, 1);
            Int32[] fileNumberArray = fileNumbers.ToArray();
            string[] fileNumberStringAray = fileNumberStrings.ToArray();
            Int32 i = 0;
            // Change the number format to have 1 extra digit than the max number length.
            // leading zeros will be included
            string format = "d" + (numberOfDigits + 1).ToString();

            // Update the file names with the new numerical value
            // To be sure we don't modify the wrong files, I check again to be sure that 
            // there is a match for the numerical entry before changing it.
            foreach (string f in files)
            {
                string baseFileName = Path.GetFileNameWithoutExtension(f);
                Match m = Regex.Match(baseFileName, this.numberMatchPattern);
                if (m.Success)
                {
                    string modifiedBaseFileName = baseFileName.Replace(fileNumberStringAray[i], fileNumberArray[i].ToString(format));
                    string newFileName = Path.Combine(Path.GetDirectoryName(f), modifiedBaseFileName);
                    i++;
                    if (this.Config.SourceFolder == this.Config.TargetFolder)
                    {
                        Directory.Move(f, newFileName);
                    }
                    else
                    {
                        newFileName = newFileName.Replace(this.Config.SourceFolder, this.Config.TargetFolder);
                        string newDir = Path.GetDirectoryName(newFileName);
                        if (!Directory.Exists(newDir))
                        {
                            Directory.CreateDirectory(newDir);
                        }
                        File.Copy(f, newFileName);
                    }
                }
            }
        }

        /// <summary>
        /// Simple function to determine how many digits are in the input number. There 
        /// may be  a better way to do this. The goal was to know how many digits are in 
        /// a particular number so I know how many digits I need to use for the updated 
        /// formatting.
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        private int FindNumberOfDigits(int inputValue , int numberOfDigits)
        {
            if (inputValue > 9)
            {
                int newValue = inputValue / 10;
                numberOfDigits++;
                numberOfDigits = FindNumberOfDigits(newValue, numberOfDigits);
            }
            else
            {
                return numberOfDigits;
            }
            return numberOfDigits;
        }

        #endregion

    }
}
