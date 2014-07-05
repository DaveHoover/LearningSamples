using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using BuildManager.Data;
using BuildManager.Helpers;
using BuildManager.Model;
using BuildManager.View;
using BuildManager.ViewModel;



namespace BuildManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> results = new List<string>();

        char quote = '"';

        public bool UseConfigurationSet { get; set; }


        /// <summary>
        /// View model
        /// </summary>
        internal MainWindowViewModel viewModel = new MainWindowViewModel();

        internal DispatcherTimer scheduledStartTimer = new DispatcherTimer();

        internal bool scheduleExecutionStart = false;

        /// <summary>
        /// Object used to prevent multiple save/load operations
        /// </summary>
        private object threadLock = new object();

        /// <summary>
        /// Background thread object
        /// </summary>
        private BackgroundWorker worker = new BackgroundWorker();


        SendOrPostCallback callbackDelegate;

        /// <summary>
        /// Test method delegate that will be executed by the background worker thread
        /// </summary>
        private Action<string> testMethodDelegate;

        private string testMethodFileName = string.Empty;

        #region Constructor/ Loaded events/Closing/Exiting

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this.viewModel;
            callbackDelegate = SendOrPostCalbackMethod;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new DoWorkEventHandler(BuildCommandWork);
            ProcessOperations.mainWindow = this;
            EnlistmentDC dc = this.viewModel.dc;
            IEnumerable<string> enlistments = from c in dc.Enlistments
                                              select c.Path;
            ApplicationState.ProgramSavedState =
                ProgSaveStateInformation.DeSerialize(ProgSaveStateInformation.ProgConfigFileName);
            string[] enlistmentData = enlistments.ToArray<string>();
            this.singleEnlistmentComboBox.ItemsSource = enlistmentData;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateUIControlsState(false);


            this.scheduleBuildCheckbox.IsChecked = ApplicationState.ProgramSavedState.ScheduledStart;
            this.dateTimePicker1.DateTimeSelected = ApplicationState.ProgramSavedState.ScheduledStartTime;
            this.singleEnlistmentComboBox.Text = ApplicationState.ProgramSavedState.SingleEnlistmentPath;
            this.singleEnlistmentSkuComboBox.Text = ApplicationState.ProgramSavedState.SingleEnlistmentSku;
            this.processPriorityComboBox.Text = ApplicationState.ProgramSavedState.ProcessPriority.ToString();
            if (String.IsNullOrEmpty(this.processPriorityComboBox.Text) ||
                this.processPriorityComboBox.Text == "0")
            {
                this.processPriorityComboBox.Text = ProcessPriorityClass.Normal.ToString();
            }
            if (ApplicationState.ProgramSavedState.ConfigurationIDs != null)
            {
                this.viewModel.JobSet = Enumerable.Empty<Configuration>();
                List<Configuration> savedConfigs = new List<Configuration>();
                for (Int32 i = 0; i < ApplicationState.ProgramSavedState.ConfigurationIDs.Length; i++)
                {
                    IEnumerable<Configuration> s =
                        from c in this.viewModel.dc.Configurations
                        where c.ID == ApplicationState.ProgramSavedState.ConfigurationIDs[i]
                        select c;
                    foreach (Configuration c in s)
                    {
                        savedConfigs.Add(c);
                    }
                }
                this.viewModel.JobSet = savedConfigs;
                this.CreateJobList();
            }
            this.buildTestTreeCheckbox.IsChecked = ApplicationState.ProgramSavedState.BuildTestTree;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ApplicationState.ProgramSavedState.ScheduledStart = (bool)this.scheduleBuildCheckbox.IsChecked;
            ApplicationState.ProgramSavedState.ScheduledStartTime = this.dateTimePicker1.DateTimeSelected;
            ApplicationState.ProgramSavedState.SingleEnlistmentPath = this.singleEnlistmentComboBox.Text;
            ApplicationState.ProgramSavedState.SingleEnlistmentSku = this.singleEnlistmentSkuComboBox.Text;
            ApplicationState.ProgramSavedState.BuildTestTree = (bool)this.buildTestTreeCheckbox.IsChecked;
            if (this.viewModel.JobSet != null)
            {
                try
                {
                    ApplicationState.ProgramSavedState.ConfigurationIDs = (from c in this.viewModel.JobSet select c.ID).ToArray();
                }
                catch (Exception)
                {
                    ApplicationState.ProgramSavedState.ConfigurationIDs = null;
                }
            }
            ProgSaveStateInformation.Serialize(ApplicationState.ProgramSavedState,
                ProgSaveStateInformation.ProgConfigFileName);
        }

        private void menuFileExit_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        #endregion


        private void UpdateUIControlsState(bool running)
        {
            this.executeSet.IsEnabled         = !running;
            this.abortButton.IsEnabled        = running;
            this.cleanAllButton.IsEnabled     = !running;
            this.syncAllBbutton.IsEnabled     = !running;
            this.dailyBuildButton.IsEnabled   = !running;
            this.cleanSyncAllButton.IsEnabled = !running;
            this.cleanSyncDailBuild.IsEnabled = !running;
            this.singleClean.IsEnabled        = !running;
            this.singleSync.IsEnabled         = !running;
            this.singleBuild.IsEnabled        = !running;
        }

        #region Scroll support for List boxes




        public void ScrollJobListBoxToEnd(int index)
        {

            this.resultsListBox.SelectedIndex = index; ;
            this.ScrollListboxToSelectedItem(this.resultsListBox, (double)this.resultsListBox.SelectedIndex);
            //this.DoEvents();
        }


        public void ScrollJobDetailsBoxToEnd()
        {

            this.resultsDetailsListBox.SelectedIndex = this.resultsDetailsListBox.Items.Count - 1;
            this.ScrollListboxToSelectedItem(this.resultsDetailsListBox, (double)this.resultsDetailsListBox.SelectedIndex);
            //this.DoEvents();
        }



        public void ScrollListboxToSelectedItem(DependencyObject obj, double scrollPosition)
        {
            ScrollViewer scroll = this.FindVisualChild(obj);
            scroll.ScrollToVerticalOffset(scrollPosition);
        }

        private ScrollViewer FindVisualChild(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is ScrollViewer)
                {
                    return (ScrollViewer)child;
                }
                else
                {
                    ScrollViewer childOfChild = FindVisualChild(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        private void SendOrPostCalbackMethod(object state)
        {
            DispatcherFrame fr = state as DispatcherFrame;
            fr.Continue = false;
        }


        void DoEvents()
        {
            DispatcherFrame f = new DispatcherFrame();

            // Lamda expression 
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                (SendOrPostCallback)((arg) =>
                {
                    DispatcherFrame fr = arg as DispatcherFrame;
                    fr.Continue = false;
                }),
                f);

            // Traditional delegate 
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                callbackDelegate, f);

            // Anon. method
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = false;
            }, f);

            Dispatcher.PushFrame(f);
        }

        #endregion

        #region Single Enlistment Combo Box  Configuration, Process priority

        private void processPriorityComboBox_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.processPriorityComboBox.SelectedValue != null)
            {
                this.processPriorityComboBox.Text = this.processPriorityComboBox.SelectedValue.ToString();
                ApplicationState.ProgramSavedState.ProcessPriority =
                    (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), this.processPriorityComboBox.Text);
            }
        }



        /// <summary>
        /// Handles the SelectionChanged event of the singleEnlistmentComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void singleEnlistmentComboBox_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.singleEnlistmentComboBox.IsEnabled = false;
            if (this.singleEnlistmentComboBox.SelectedValue != null)
            {
                string selectedText = this.singleEnlistmentComboBox.SelectedValue.ToString();
                if (!String.IsNullOrEmpty(selectedText))
                {
                    EnlistmentDC dc = this.viewModel.dc;
                    this.singleEnlistmentSkuComboBox.Text = "";
                    IEnumerable<string> enlistments = (from c in dc.Configurations
                                                       where c.Enlistment1.Path == selectedText
                                                       select c.PhoneSKU.Name).Distinct();
                    this.singleEnlistmentSkuComboBox.ItemsSource = enlistments.ToList<string>();
                    this.singleEnlistmentComboBox.Text = selectedText;
                }
            }
            this.singleEnlistmentComboBox.IsEnabled = true;
        }


        /// <summary>
        /// Handles the SelectionChanged event of the singleEnlistmentSkuComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void singleEnlistmentSkuComboBox_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.singleEnlistmentSkuComboBox.SelectedValue != null)
            {
                this.singleEnlistmentSkuComboBox.Text = this.singleEnlistmentSkuComboBox.SelectedValue.ToString();
            }
        }

        #endregion

        #region Button Handlers for Building job set


        /// <summary>
        /// Clean a single enlistment
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard pattern</param>
        private void singleClean_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            string enlist   = this.singleEnlistmentComboBox.Text;
            string sku      = this.singleEnlistmentSkuComboBox.Text;
            if (!(String.IsNullOrEmpty(enlist) || String.IsNullOrEmpty(sku)))
            {
                this.viewModel.JobSet = from c in dc.Configurations
                                        where c.Command.Command1 == DbCommand.Clean.ToString() &&
                                              c.Enlistment1.Path == enlist &&
                                              c.PhoneSKU.Name == sku
                                        select c;
                this.CreateJobList();
            }
        }


        /// <summary>
        /// Sync a single enlistment
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard pattern</param>
        private void singleSync_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            IEnumerable<Configuration> fullJob = Enumerable.Empty<Configuration>();
            fullJob = fullJob.Concat(this.viewModel.JobSet);
            string enlist = this.singleEnlistmentComboBox.Text;
            string sku = this.singleEnlistmentSkuComboBox.Text;
            if (!(String.IsNullOrEmpty(enlist) || String.IsNullOrEmpty(sku)))
            {
                this.viewModel.JobSet = from c in dc.Configurations
                                        where c.Command.Command1 == DbCommand.Sync.ToString() &&
                                              c.Enlistment1.Path == enlist &&
                                              c.PhoneSKU.Name == sku
                                        select c;
                fullJob = fullJob.Concat(this.viewModel.JobSet);
                this.viewModel.JobSet = fullJob;
                this.CreateJobList();
            }
        }


        /// <summary>
        /// Build a single enlistment
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard Pattern</param>
        private void singlebuild_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            IEnumerable<Configuration> fullJob = Enumerable.Empty<Configuration>();
            if (this.viewModel.JobSet != null)
            {
                fullJob = fullJob.Concat(this.viewModel.JobSet);
                string enlist = this.singleEnlistmentComboBox.Text;
                string sku = this.singleEnlistmentSkuComboBox.Text;
                if (!(String.IsNullOrEmpty(enlist) || String.IsNullOrEmpty(sku)))
                {
                    if ((bool)this.buildTestTreeCheckbox.IsChecked)
                    {
                        this.viewModel.JobSet = from c in dc.Configurations
                                                where (c.Command.Command1 == DbCommand.RebuildAll.ToString()  || 
                                                       c.Command.Command1 == DbCommand.WpRebuildAll.ToString())&&
                                                      c.Enlistment1.Path == enlist &&
                                                      c.PhoneSKU.Name == sku
                                                select c;
                    }
                    else
                    {
                        this.viewModel.JobSet = from c in dc.Configurations
                                                where (c.Command.Command1 == DbCommand.Clean_Sync_RebuildNoTest.ToString() || 
                                                       c.Command.Command1 == DbCommand.WpClean_Sync_RebuildNoTest.ToString()) &&
                                                      c.Enlistment1.Path == enlist &&
                                                      c.PhoneSKU.Name == sku
                                                select c;
                    }
                    fullJob = fullJob.Concat(this.viewModel.JobSet);
                    this.viewModel.JobSet = fullJob;
                    this.CreateJobList();
                }
            }
            else
            {
                MessageBox.Show("Build must be added after either a clean or sync.");
            }
        }



        /// <summary>
        /// Clean all enlistments/skus
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard pattern</param>
        private void cleanAllButton_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            IEnumerable<Configuration> xdeClean = 
             (from c in dc.Configurations
              where c.Command.Command1 == DbCommand.Clean.ToString() &&
                    c.PhoneSKU.Name == PhoneSkuInfo.XdeSku  ||
                    c.Command.Command1 == DbCommand.Clean.ToString() &&
                    c.PhoneSKU.Name == PhoneSkuInfo.ApolloDeviceFreSku 
              select c).AsEnumerable<Configuration>().
              Distinct<Configuration>(new ConfigurationEnlistmentValueCompare());

            IEnumerable<Configuration> apolloX86Chk =
            (from c in dc.Configurations
             where c.Command.Command1 == DbCommand.Clean.ToString() &&
                   c.PhoneSKU.Name == PhoneSkuInfo.ApolloDeviceChkSku
             select c).AsEnumerable<Configuration>().
             Distinct<Configuration>(new ConfigurationEnlistmentValueCompare());

            IEnumerable<Configuration> armClean = 
             (from c in dc.Configurations
              where c.Command.Command1 == DbCommand.Clean.ToString() &&
                    c.PhoneSKU.Name == PhoneSkuInfo.E600Sku  ||
                    c.Command.Command1 == DbCommand.Clean.ToString() &&
                    c.PhoneSKU.Name == PhoneSkuInfo.ApolloDeviceArmFreSku 
              select c).AsEnumerable<Configuration>().
              Distinct<Configuration>(new ConfigurationEnlistmentValueCompare());

            IEnumerable<Configuration> apolloArmChk =
           (from c in dc.Configurations
            where 
                  c.Command.Command1 == DbCommand.Clean.ToString() &&
                  c.PhoneSKU.Name == PhoneSkuInfo.ApolloDeviceArmChkSku
            select c).AsEnumerable<Configuration>().
            Distinct<Configuration>(new ConfigurationEnlistmentValueCompare());
            this.viewModel.JobSet = xdeClean.Concat(apolloX86Chk).Concat(armClean).Concat(apolloArmChk);
            this.CreateJobList();
        }

        /// <summary>
        /// Sync all enlistments. Do this once per enlistment
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard pattern</param>
        private void syncAllButton_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            this.viewModel.JobSet = (from c in dc.Configurations
                                    where c.Command.Command1 == DbCommand.Sync.ToString() ||
                                          c.Command.Command1 == DbCommand.WpSync.ToString()
                                    select c).AsEnumerable<Configuration>().
                                    Distinct<Configuration>(new ConfigurationEnlistmentValueCompare());
            this.CreateJobList();
        }

        /// <summary>
        /// Select items that are part of the daily build
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard pattern</param>
        private void dailyBuildButton_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            this.viewModel.JobSet = from c in dc.Configurations
                                    where c.BuildType.Name == DbBuildType.Daily.ToString()
                                    select c;
            this.CreateJobList();
        }


        /// <summary>
        /// Clean all enlistments/skus and sync all enlistments
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard pattern</param>
        private void cleanSyncAllButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Configuration> fullJob = Enumerable.Empty<Configuration>();
            this.cleanAllButton_Click(sender, e);
            fullJob = fullJob.Concat(this.viewModel.JobSet);
            this.syncAllButton_Click(sender, e);
            fullJob = fullJob.Concat(this.viewModel.JobSet);

            this.viewModel.JobSet = fullJob;
            this.CreateJobList();
        }

        /// <summary>
        /// Clean all enlistments/skus, sync enlistments, build daily build
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard pattern</param>
        private void cleanSyncDailyBuild_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Configuration> fullJob = Enumerable.Empty<Configuration>();
            this.cleanAllButton_Click(sender, e);
            fullJob = fullJob.Concat(this.viewModel.JobSet);
            this.syncAllButton_Click(sender, e);
            fullJob = fullJob.Concat(this.viewModel.JobSet);
            this.dailyBuildButton_Click(sender, e);
            fullJob = fullJob.Concat(this.viewModel.JobSet);

            this.viewModel.JobSet = fullJob;
            this.CreateJobList();
        }


        /// <summary>
        /// Create a job list from the configuration items selected
        /// </summary>
        private void CreateJobList()
        {
            this.viewModel.Results.Clear();
            List<Configuration> itemsToRemove = new List<Configuration>();

            foreach (Configuration c in this.viewModel.JobSet)
            {
                if (!FileSys.IsValidEnlistmentRoot(c.EnlistmentPath))
                {
                    itemsToRemove.Add(c);
                }
            }
            this.viewModel.JobSet = this.viewModel.JobSet.Except<Configuration>(
            itemsToRemove.AsEnumerable<Configuration>(), new ConfigurationIdValueCompare());
            foreach (Configuration c in this.viewModel.JobSet)
            {
                JobResult r = new JobResult
                {
                    JobID = c.ID,
                    Status = JobResultStatus.NotStarted.ToString(),
                    Command = c.Command.Command1,
                    EnlistmentName = c.Enlistment1.Path,
                    PhoneSku = c.PhoneSKU.Name
                };
                this.viewModel.Results.Add(r);
            }
        }

        #endregion

        #region Create a Job set from the Sync/Clean/Build options from the Configuration Window

        public void UpdateSelectedSet()
        {
            EnlistmentDC dc = this.viewModel.dc;
            IEnumerable<Configuration> items = from c in this.viewModel.SelectedOperationSet
                                               where c.Command.Command1.ToLowerInvariant() ==
                                               DbCommand.Clean_Sync_Rebuild.ToString().ToLowerInvariant() ||
                                               c.Command.Command1.ToLowerInvariant() ==
                                               DbCommand.Clean_Sync_RebuildNoTest.ToString().ToLowerInvariant() ||
                                               c.Command.Command1.ToLowerInvariant() ==
                                               DbCommand.WpClean_Sync_Rebuild.ToString().ToLowerInvariant() ||
                                               c.Command.Command1.ToLowerInvariant() ==
                                               DbCommand.WpClean_Sync_RebuildNoTest.ToString().ToLowerInvariant()
                                               select c;

            this.viewModel.JobSet = Enumerable.Empty<Configuration>();
            if (items != null)
            {
                if (items.Count() > 0)
                {
                    foreach (Configuration c in items)
                    {
                        this.viewModel.JobSet = this.viewModel.JobSet.Concat(FindSyncCleanBuildFromCombined(c));
                    }
                    IEnumerable<Configuration> nonComboItems = 
                        this.viewModel.SelectedOperationSet.Except(items, new ConfigurationIdValueCompare());
                    this.viewModel.JobSet = this.viewModel.JobSet.Concat(nonComboItems);
                }
                else
                {
                    this.viewModel.JobSet = this.viewModel.SelectedOperationSet;
                }
            }
            else
            {
                this.viewModel.JobSet = this.viewModel.SelectedOperationSet;
            }
            this.CreateJobList();
        }


        private IEnumerable<Configuration> FindSyncCleanBuildFromCombined(Configuration cfg)
        {
            EnlistmentDC dc = this.viewModel.dc;
            IEnumerable<Configuration> jobs = Enumerable.Empty<Configuration>();
            IEnumerable<Configuration> sync = from c in dc.Configurations
                                              where (c.Command.Command1 == DbCommand.Sync.ToString() || 
                                                     c.Command.Command1 == DbCommand.WpSync.ToString()) &&
                                                    c.PhoneSKU.ID == cfg.PhoneSkuID &&
                                                    c.EnlistmentID == cfg.EnlistmentID
                                              select c;
            IEnumerable<Configuration> clean = from c in dc.Configurations
                                               where (c.Command.Command1 == DbCommand.Clean.ToString() || 
                                                     c.Command.Command1 == DbCommand.WpSync.ToString()) &&
                                                    c.PhoneSKU.ID == cfg.PhoneSkuID &&
                                                    c.EnlistmentID == cfg.EnlistmentID
                                               select c;
            string buildCommand = DbCommand.RebuildNoTest.ToString();
            string wpBuildCommand = DbCommand.WpRebuildNoTest.ToString();
            if (cfg.Command.Command1 == DbCommand.Clean_Sync_Rebuild.ToString())
            {
                buildCommand = DbCommand.RebuildAll.ToString();
            }
            if (cfg.Command.Command1 == DbCommand.WpClean_Sync_Rebuild.ToString())
            {
                wpBuildCommand = DbCommand.WpRebuildAll.ToString();
            }

            IEnumerable<Configuration> build = from c in dc.Configurations
                                               where (c.Command.Command1 == buildCommand ||
                                                      c.Command.Command1 == wpBuildCommand )&&
                                                     c.PhoneSKU.ID == cfg.PhoneSkuID &&
                                                     c.EnlistmentID == cfg.EnlistmentID
                                               select c;
            jobs = jobs.Concat(sync).Concat(clean).Concat(build);
            return jobs;
        }


        #endregion

        #region ConfigureWindow/Abort/Execute/ScheduledTimerFired/
        /// <summary>
        /// Open set configuration window to allow selection of jobs to run
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard Arg</param>
        private void configureSet_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindowxaml c = new ConfigurationWindowxaml(this);
            c.Show();
        }

        /// <summary>
        /// Abort the current job operation set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void abortButton_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.RunningJobs.IsRunning = false;
            if (this.scheduleExecutionStart == true)
            {
                this.scheduleExecutionStart = false;
                this.scheduledStartTimer.Stop();
                scheduledStartTimer.Tick -= new EventHandler(ScheduledTimerFired);
            }
            foreach (JobResult r in this.viewModel.Results)
            {
                if (r.Status == JobResultStatus.Waiting.ToString() ||
                    r.Status == JobResultStatus.Started.ToString() ||
                    r.Status == JobResultStatus.Scheduled.ToString())
                {
                    r.Status = JobResultStatus.Aborted.ToString();
                }
            }
            if (this.viewModel.RunningJobs.CommandProcess != null)
            {
                this.viewModel.RunningJobs.CommandProcess.CancelErrorRead();
                this.viewModel.RunningJobs.CommandProcess.CancelOutputRead();
                this.viewModel.RunningJobs.CommandProcess.Kill();
                this.viewModel.RunningJobs.CommandProcess.Close();
                this.viewModel.RunningJobs.CommandProcess.Dispose();
                this.viewModel.RunningJobs.CommandProcess = null;
            }

            this.worker.DoWork -= new DoWorkEventHandler(BuildCommandWork);
            this.worker.Dispose();
            this.worker = new BackgroundWorker();
            this.worker.DoWork += new DoWorkEventHandler(BuildCommandWork);
            // Indicate that we are done, since we might get all callback from the worker 
            // thread, even though we killed it
            this.viewModel.RunningJobs.Index = this.viewModel.RunningJobs.Jobs.Length - 1;
            this.UpdateUIControlsState(false);
        }




        /// <summary>
        /// Start Executing the set of selected buid commands
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Standard arg</param>
        private void executeSet_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateUIControlsState(true);
            string logFileName = this.viewModel.CreateLogFileName();
            this.viewModel.RunningJobs = new JobSetInformation
            {
                Index = 0,
                Jobs = this.viewModel.JobSet.ToArray(),
                Results = this.viewModel.Results.ToArray(),
                IsRunning = true ,
                ErrorInformation = String.Empty ,
                LogFileName = logFileName
            };

            if (this.scheduleBuildCheckbox.IsChecked.Value)
            {
                DateTime scheduledTime = dateTimePicker1.DateTimeSelected;
                TimeSpan s = scheduledTime - DateTime.Now;
                TimeSpan refTime = TimeSpan.FromSeconds(5);
                int compare = s.CompareTo(refTime);
                if (compare > 0)
                {
                    this.scheduleExecutionStart = true;
                    scheduledStartTimer.Interval = s;
                    //scheduledStartTimer.Interval = refTime;
                    scheduledStartTimer.Tick += new EventHandler(ScheduledTimerFired);
                    scheduledStartTimer.Start();
                    foreach (JobResult r in this.viewModel.Results)
                    {
                        r.Status = JobResultStatus.Scheduled.ToString();
                    }
                }
                else
                {
                    foreach (JobResult r in this.viewModel.Results)
                    {
                        r.Status = JobResultStatus.BadSchedule.ToString();
                    }
                    this.UpdateUIControlsState(false);
                }
            }
            else
            {
                foreach (JobResult r in this.viewModel.Results)
                {
                    r.Status = JobResultStatus.Waiting.ToString();
                }
                this.CreateJob();
            }
        }


        /// <summary>
        /// Handler for the scheduled Timer event.
        /// This will start the job set at the correct time.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Standard Arg</param>
        private void ScheduledTimerFired(object sender, EventArgs e)
        {
            this.scheduledStartTimer.Stop();
            scheduledStartTimer.Tick -= new EventHandler(ScheduledTimerFired);
            foreach (JobResult r in this.viewModel.Results)
            {
                r.Status = JobResultStatus.Waiting.ToString();
            }
            this.CreateJob();
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        private void CreateJob()
        {
            JobSetInformation rj = this.viewModel.RunningJobs;
            Configuration c      = rj.Jobs[rj.Index];
            JobResult r          = rj.Results[rj.Index];
            ScrollJobListBoxToEnd(rj.Index);
            rj.IsSuccessful = false;
            r.StartTime = DateTime.Now;
            r.Status = JobResultStatus.Started.ToString();
            this.viewModel.Log.Clear();

            this.testMethodDelegate =
                (s) =>
                {
                    string shell = Path.Combine(
                        Environment.GetEnvironmentVariable("SystemRoot"), @"system32\cmd.exe");
                    string pbxmlFile = c.BuildXml.FilePath;
                    string enlistment = c.Enlistment1.Path;
                    string sku = c.PhoneSKU.Name;
                    string command = c.Command.Command1;
                    string commandLine = c.Command.CommandLine;
                    string wmopen = Path.Combine(enlistment, @"public\COMMON\oak\misc\WMOpen.bat");
                    string args = " /k " + wmopen + " " + pbxmlFile + " " + quote + sku +
                                                      quote + "&&" + commandLine;
                    if (((int)c.PhoneSkuID) >= PhoneSkuInfo.ApolloSku)
                    { 
                        wmopen = Path.Combine(enlistment, @"tools\bat\WPOpen.bat");
                        args = " /k " + wmopen + " " + pbxmlFile + " " +  sku +  " " + "&&" + commandLine + "&&" + "exit";
                    }

                    //  %SystemRoot%\system32\cmd.exe /k F:\Wm7SevenApp\public\COMMON\oak\misc\WMOpen.bat F:\wm.pbxml "E600 ARMV7 Release"
                    string workingDirectory = enlistment;
                    if (c.Command.Command1 == DbCommand.RebuildAll.ToString() ||
                        c.Command.Command1 == DbCommand.RebuildNoTest.ToString() ||
                        c.Command.Command1 == DbCommand.WpRebuildAll.ToString() ||
                        c.Command.Command1 == DbCommand.WpRebuildNoTest.ToString())
                    {
                        rj.IsBuild = true;
                    }
                    else
                    {
                        rj.IsBuild = false;
                    }

                    Action<string> log = ((l) =>
                    {
                        lock (this.viewModel.Log.lockObject)
                        {
                            this.viewModel.Log.Add(l);
                        }
                    });
                    this.Dispatcher.BeginInvoke(log, new object[] { "*********************Job Starting *********************" });
                    this.Dispatcher.BeginInvoke(log, new object[] { "Starting Time was " + r.StartTime.ToLongDateString() + " " +
                        r.StartTime.ToLongTimeString()}); 
                    this.Dispatcher.BeginInvoke(log, new object[] { "Command Executed : " + args });
                    this.Dispatcher.BeginInvoke(log, new object[] { "Working Dir : " + enlistment });
                    
                    if (!File.Exists(shell) || !(Directory.Exists(workingDirectory)))
                    {
                        rj.IsSuccessful = false;
                        if (!File.Exists(shell))
                        {
                            rj.ErrorInformation = "File " + shell + " Did not Exist";
                        }
                        else
                        {
                            rj.ErrorInformation = "Directory " + workingDirectory + " Did not exist";
                        }
                        this.Dispatcher.BeginInvoke(log, new object[] { rj.ErrorInformation });
                    }
                    else
                    {
                        try
                        {
                            ProcessInformation procInfo =
                                ProcessOperations.RunProcess(shell, workingDirectory, args, true);
                            CheckForBuildCompletedSuccesfully(workingDirectory, sku, command, c);
                        }
                        catch (Exception e)
                        {
                            rj.IsSuccessful = false;
                            rj.ErrorInformation = e.Message;
                            if (e.InnerException != null)
                            {
                                rj.ErrorInformation += e.InnerException.Message;
                            }
                            this.Dispatcher.BeginInvoke(log, new object[] { rj.ErrorInformation });
                        }
                    }
                };
            this.ExecuteBuildCommand(SignalBuildCommandCompleted);
        }


        /// <summary>
        /// Checks for build completed succesfully.
        /// We want to check for a failed condition, so we can send email if the
        /// build failed
        /// </summary>
        /// <param name="workingDir">The working dir.</param>
        /// <param name="sku">The sku.</param>
        /// <param name="command">The command.</param>
        /// <param name="c">The c.</param>
        private void CheckForBuildCompletedSuccesfully(
            string workingDir,
            string sku,
            string command,
            Configuration c)
        {
            bool buildErrorFlag = false;
            string buildErrorFile = String.Empty;
            if (c.Command.Command1 == DbCommand.RebuildAll.ToString() ||
                c.Command.Command1 == DbCommand.RebuildNoTest.ToString() ||
                c.Command.Command1 == DbCommand.WpRebuildAll.ToString() ||
                c.Command.Command1 == DbCommand.WpRebuildNoTest.ToString())
            {
                if (c.Command.Command1 == DbCommand.RebuildAll.ToString() ||
                c.Command.Command1 == DbCommand.RebuildNoTest.ToString())
                {
                    buildErrorFile = Path.Combine(workingDir, "build.err");
                    buildErrorFlag = File.Exists(buildErrorFile);
                }
                if (c.Command.Command1 == DbCommand.WpRebuildAll.ToString() ||
                    c.Command.Command1 == DbCommand.WpRebuildNoTest.ToString())
                {
                    string skuInfo = PhoneSkuInfo.GetApolloBuildSkuTypeForLogFiles(c.PhoneSKU.Name);
                    IEnumerable<string> errorFiles = Directory.EnumerateFiles(workingDir, "*" + skuInfo + ".err");
                    foreach (string s in errorFiles)
                    {
                        buildErrorFlag = true;
                        buildErrorFile = s;
                        break;
                    }
                }
                if (buildErrorFlag)
                {
                    string[] buildError = File.ReadAllLines(buildErrorFile);
                    List<string> body = new List<string>();
                    foreach (string b in buildError)
                    {
                        body.Add(b);
                    }
                    string emailAddress = c.Email.EmailAccount;
                    EmailOps.SendMailtoExchange(emailAddress, emailAddress, "Build Break! " +
                        workingDir + " " + sku, body);
                    this.viewModel.RunningJobs.IsSuccessful = false;
                }
                else
                {
                    this.viewModel.RunningJobs.IsSuccessful = true;
                }
            }
            else
            {
                this.viewModel.RunningJobs.IsSuccessful = true;
            }
        }


        /// <summary>
        /// Deferred startup work to improve page render performance.
        /// </summary>
        /// <param name="completed">Delegate to call on completed</param>
        internal void ExecuteBuildCommand(Action completed)
        {
            lock (this.viewModel.Log.lockObject)
            {
                this.viewModel.Log.Clear();
            }
            this.worker.RunWorkerAsync(completed);
        }

        /// <summary>
        /// Background thread work for loading 
        /// The loading of the supported conversions from the Xap as well as the 
        /// favorites from isolated storage is done on a non UI thread to allow the 
        /// fastest possible page render
        /// </summary>
        /// <param name="sender">worker thread object</param>
        /// <param name="e">Action delegate for completion</param>
        private void BuildCommandWork(object sender, DoWorkEventArgs e)
        {
            Action completed = e.Argument as Action;
            lock (threadLock)
            {

                if (this.testMethodDelegate != null)
                {
                    this.testMethodDelegate(this.testMethodFileName);
                }
            }

            if (completed != null)
            {
                completed();
            }
        }


        /// <summary>
        /// Interface between the build command executing on a background thread and 
        /// the UI thread
        /// </summary>
        private void SignalBuildCommandCompleted()
        {
            Dispatcher.BeginInvoke(new Action(this.NotifyUIOfWorkerThreadCompleted));
        }

        /// <summary>
        /// Called after the build command has completed, so that we can update the UI
        /// and schedule another build command if one remains to be executed.
        /// </summary>
        private void NotifyUIOfWorkerThreadCompleted()
        {
            JobSetInformation rj = this.viewModel.RunningJobs;
            Configuration c = rj.Jobs[rj.Index];
            JobResult r = rj.Results[rj.Index];
            if (this.viewModel.RunningJobs.IsRunning)
            {
                r.StopTime = DateTime.Now;
                // Need to check for build status, to know if we should email
                if (rj.IsSuccessful)
                {
                    r.Status = JobResultStatus.Completed.ToString();
                }
                else
                {
                    r.Status = JobResultStatus.Error.ToString();
                }
                Action<string> log = ((l) =>
                {
                    lock (this.viewModel.Log.lockObject)
                    {
                        this.viewModel.Log.Add(l);
                    }
                });
                this.viewModel.Log.Add("Finish Time was " + r.StopTime.ToLongDateString() + " " +
                        r.StopTime.ToLongTimeString());
                TimeSpan elapsedTime  = r.StopTime - r.StartTime;
                string elapsed = "Job Time: H:M:S = " + elapsedTime.Hours.ToString() + ":" +
                    elapsedTime.Minutes.ToString() + ":" + elapsedTime.Seconds.ToString();
                this.viewModel.Log.Add(elapsed);
            }
            File.AppendAllLines(rj.LogFileName, this.viewModel.Log.ToArray());
            if (rj.Jobs.Length > rj.Index + 1)
            {
                rj.Index++;
                int count = 0;
                do
                {
                    Thread.Sleep(100);
                    count++;
                }
                while (this.worker.IsBusy && count < 10);
                this.worker.DoWork -= new DoWorkEventHandler(BuildCommandWork);
                this.worker.Dispose();
                this.worker = new BackgroundWorker();
                this.worker.WorkerSupportsCancellation = true;
                this.worker.DoWork += new DoWorkEventHandler(BuildCommandWork);
                if (this.viewModel.RunningJobs.IsRunning)
                {
                    this.CreateJob();
                }
            }
            else
            {
                this.viewModel.RunningJobs.IsRunning = false;
                this.UpdateUIControlsState(false);
            }
        }

        #region Menu Commands

        private void menuBuildSingle_Clicked(object sender, RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
        }

        private void menuBuildResults_Clicked(object sender, RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
        }


        private void menuConfigEnlistments_Clicked(object sender, RoutedEventArgs e)
        {

        }


        private void menuEnlistmentsManage_Clicked(object sender, RoutedEventArgs e)
        {
            EnlistmentManagement c = new EnlistmentManagement(this);
            c.Show();

        }

        private void menuPhoneSkusManage_Clicked(object sender, RoutedEventArgs e)
        {
            PhoneSkuManagement w = new PhoneSkuManagement(this);
            w.Show();
        }

        private void menuBuildCommandsManage_Clicked(object sender, RoutedEventArgs e)
        {
            BuildCommandManagement w = new BuildCommandManagement(this);
            w.Show();

        }

        private void menuEmailManage_Clicked(object sender, RoutedEventArgs e)
        {
            EmailManagement w = new EmailManagement(this);
            w.Show();
        }

        private void menuPbXmlManage_Clicked(object sender, RoutedEventArgs e)
        {
            PbXmlManagement w = new PbXmlManagement(this);
            w.Show();
        }

        private void menuUserManage_Clicked(object sender, RoutedEventArgs e)
        {
            UserManagement w = new UserManagement(this);
            w.Show();
        }

        private void menuConfigSelection_Clicked(object sender, RoutedEventArgs e)
        {
            ConfigurationWindowxaml c = new ConfigurationWindowxaml(this);
            c.Show();
        }
        private void menuBuildDaily_Clicked(object sender, RoutedEventArgs e)
        {
            ConfigurationWindowxaml c = new ConfigurationWindowxaml(this);
            c.Show();

        }
        private void menuBuildWeekly_Clicked(object sender, RoutedEventArgs e)
        {
            ConfigurationWindowxaml c = new ConfigurationWindowxaml(this);
            c.Show();

        }
        private void menuHelpAbout_Clicked(object sender, RoutedEventArgs e)
        {
            AboutBox b = new AboutBox();
            b.ShowDialog();
        }

        #endregion

        #region Older Test code, could be deleted


        private void TestCodeArchive()
        {
            EmailOps.SendMailtoExchange("dahoover@microsoft.com", "dahoover@microsoft.com", "TestMessage",
                new List<string> { "Line1", "Line2" });
            this.CleanEnlistments();
            SyncEnlistments();
            BuildEnlistment();
        }


        public void CleanEnlistments()
        {
            this.testMethodDelegate =
                (s) =>
                {
                    string shell = Path.Combine(System.Environment.GetEnvironmentVariable("SystemRoot"), @"system32\cmd.exe");
                    //  %SystemRoot%\system32\cmd.exe /k F:\Wm7SevenApp\public\COMMON\oak\misc\WMOpen.bat F:\wm.pbxml "E600 ARMV7 Release"
                    string args = @" /k F:\Wm7SevenApp\public\COMMON\oak\misc\WMOpen.bat F:\wm.pbxml " + quote + "E600 ARMV7 Release" + quote + "&& ceclean.bat";
                    string workingDirectory = Path.GetDirectoryName(@"F:\Wm7SevenApp");
                    ProcessInformation procInfo = ProcessOperations.RunProcess(shell, workingDirectory, args, true);
                    string[] stdInfo = procInfo.GetStandardOutput();
                    string[] errInfo = procInfo.GetErrorOutput();
                    Debug.WriteLine("Finished");
                    args = @" /k F:\Wm7SevenApp\public\COMMON\oak\misc\WMOpen.bat F:\wm.pbxml " + quote + "Windows Phone Emulator x86 Release" + quote + "&& ceclean.bat";
                    workingDirectory = Path.GetDirectoryName(@"F:\Wm7SevenApp");
                    procInfo = ProcessOperations.RunProcess(shell, workingDirectory, args, true);
                    string[] stdInfo1 = procInfo.GetStandardOutput();
                    string[] errInfo1 = procInfo.GetErrorOutput();
                    Debug.WriteLine("Finished");
                };

            this.ExecuteBuildCommand(SignalBuildCommandCompleted);
        }

        public void SyncEnlistments()
        {
            this.testMethodDelegate =
                           (s) =>
                           {
                               string shell = Path.Combine(System.Environment.GetEnvironmentVariable("SystemRoot"), @"system32\cmd.exe");
                               //  %SystemRoot%\system32\cmd.exe /k F:\Wm7SevenApp\public\COMMON\oak\misc\WMOpen.bat F:\wm.pbxml "E600 ARMV7 Release"
                               string args = @" /k F:\Wm7SevenApp\public\COMMON\oak\misc\WMOpen.bat F:\wm.pbxml " + quote + "E600 ARMV7 Release" + quote + "&& Wm Sync";
                               string workingDirectory = @"F:\Wm7SevenApp";
                               ProcessInformation procInfo = ProcessOperations.RunProcess(shell, workingDirectory, args, true);
                               string[] stdInfo = procInfo.GetStandardOutput();
                               string[] errInfo = procInfo.GetErrorOutput();
                               Debug.WriteLine("Finished");
                           };
            this.ExecuteBuildCommand(SignalBuildCommandCompleted);
        }

        public void BuildEnlistment()
        {
            this.testMethodDelegate =
                           (s) =>
                           {
                               string shell = Path.Combine(System.Environment.GetEnvironmentVariable("SystemRoot"), @"system32\cmd.exe");
                               //  %SystemRoot%\system32\cmd.exe /k F:\Wm7SevenApp\public\COMMON\oak\misc\WMOpen.bat F:\wm.pbxml "E600 ARMV7 Release"
                               string args = @" /k F:\Wm7SevenApp\public\COMMON\oak\misc\WMOpen.bat F:\wm.pbxml " + quote + "E600 ARMV7 Release" + quote + "&& Wm Sync";
                               string workingDirectory = @"F:\Wm7SevenApp";
                               ProcessInformation procInfo = ProcessOperations.RunProcess(shell, workingDirectory, args, true);
                               string[] stdInfo = procInfo.GetStandardOutput();
                               string[] errInfo = procInfo.GetErrorOutput();
                               string buildErrorFile = Path.Combine(workingDirectory, "build.err");
                               if (File.Exists(buildErrorFile))
                               {
                                   string[] buildError = File.ReadAllLines(buildErrorFile);
                                   List<string> body = new List<string>();
                                   foreach (string b in buildError)
                                   {
                                       body.Add(b);
                                   }
                                   EmailOps.SendMailtoExchange("dahoover@microsoft.com", "dahoover@microsoft.com", "Build Break! " + @"F:\Wm7App" + " " + "E600", body);
                               }
                               Debug.WriteLine("Finished");
                           };
            this.ExecuteBuildCommand(SignalBuildCommandCompleted);

        }



        #endregion


    }
}
