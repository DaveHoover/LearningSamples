using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BuildManager.ViewModel;
using BuildManager.Data;
using BuildManager.Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using BuildManager.Helpers;
using System.Data;
using System.Windows.Controls.Primitives;

namespace BuildManager.View
{
    /// <summary>
    /// Interaction logic for ConfigurationWindowxaml.xaml
    /// </summary>
    public partial class ConfigurationWindowxaml : Window
    {

        public const string DailyBuildHeader = "DailyBuild";

        public const string EnlistmentHeader = "EnlistmentID";

        public const string PhoneSkuHeader = "PhoneSkuID";

        public const string CommandHeader = "CommandID";

        internal ConfigurationWindowViewModel viewModel = new ConfigurationWindowViewModel();

        public const string BuildTypeField = "BuildType";
        public const string BuildXmlField = "BuildXml";
        public const string BuildCommandField = "BuildCommand";
        public const string EmailField = "Email";
        public const string EnlistmentField = "Enlistment";
        public const string PhoneSkuField = "PhoneSku";
        public const string UserField = "UserName";

        public ICollectionView Configurations { get; private set; }

        protected DataRowView rowBeingEdited = null;


        Configuration testConfigItem;

        Configuration CurrentItemToEdit { get; set; }

        List<Configuration> userList = new List<Configuration>();

        MainWindow mainWindow = null;

        public bool UseCustomSort = false;


        public ConfigurationWindowxaml()
        {
            this.Initialize(null);
        }
        public ConfigurationWindowxaml(MainWindow main)
        {
            this.Initialize(main);
        }


        private void Initialize(MainWindow mainWindowReference)
        {
            InitializeComponent();
            this.mainWindow = mainWindowReference;
            this.DataContext = this.viewModel;
            Configurations = CollectionViewSource.GetDefaultView(this.viewModel.dc.Configurations);
            this.configDataGridMan.ItemsSource = Configurations;
            EnlistmentDC dc = this.viewModel.dc;

            this.ConfigurationFieldSelectComboxBox(this.query0FieldSelectComboBox);
            this.ConfigurationFieldSelectComboxBox(this.query1FieldSelectComboBox);
            this.ConfigurationFieldSelectComboxBox(this.query2FieldSelectComboBox);
            this.ConfigurationFieldSelectComboxBox(this.query3FieldSelectComboBox);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ProgSaveStateInformation p = ApplicationState.ProgramSavedState;
            this.UpdateQueryParameters(query0FieldSelectComboBox, query0ItemSelectComboBox, p.Query[0]);
            this.UpdateQueryParameters(query1FieldSelectComboBox, query1ItemSelectComboBox, p.Query[1]);
            this.UpdateQueryParameters(query2FieldSelectComboBox, query2ItemSelectComboBox, p.Query[2]);
            this.UpdateQueryParameters(query3FieldSelectComboBox, query3ItemSelectComboBox, p.Query[3]);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProgSaveStateInformation p = ApplicationState.ProgramSavedState;
            this.ConfigureQueryParameters(query0FieldSelectComboBox, query0ItemSelectComboBox, p.Query[0]);
            this.ConfigureQueryParameters(query1FieldSelectComboBox, query1ItemSelectComboBox, p.Query[1]);
            this.ConfigureQueryParameters(query2FieldSelectComboBox, query2ItemSelectComboBox, p.Query[2]);
            this.ConfigureQueryParameters(query3FieldSelectComboBox, query3ItemSelectComboBox, p.Query[3]);
        }

        public void WPF_DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (UseCustomSort)
            {
                e.Handled = true;   // prevent the built-in sort from sorting
                PerformCustomSort(e.Column);
            }
        }

        private void PerformCustomSort(DataGridColumn column)
        {
            ListSortDirection direction = (column.SortDirection != ListSortDirection.Ascending) ?
                ListSortDirection.Ascending : ListSortDirection.Descending;
            column.SortDirection = direction;
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(this.configDataGridMan.ItemsSource);
            WpfDataGridSort mySort = new WpfDataGridSort(direction, column);
            lcv.CustomSort = mySort;  // provide our own sort    
        }


        private void ConfigureQueryParameters(
            ComboBox tableSelect,
            ComboBox fieldSelect,
            QueryParameters q)
        {
            tableSelect.Text = q.TableSelection;
            fieldSelect.Text = q.FieldItemSelection;
        }

        private void UpdateQueryParameters(
            ComboBox tableSelect,
            ComboBox fieldSelect,
            QueryParameters q)
        {
            q.TableSelection = tableSelect.Text;
            q.FieldItemSelection = fieldSelect.Text;
        }

        private void ConfigurationFieldSelectComboxBox(ComboBox b)
        {
            List<string> fields = new List<string>();
            fields.Add(BuildTypeField);
            fields.Add(BuildXmlField);
            fields.Add(BuildCommandField);
            fields.Add(EmailField);
            fields.Add(EnlistmentField);
            fields.Add(PhoneSkuField);
            fields.Add(UserField);
            b.ItemsSource = fields;
        }

        #region Configure Combo Boxes

        private void query0ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.query0ItemSelectComboBox.Text = "";
        }
        private void query1ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.query1ItemSelectComboBox.Text = "";
        }
        private void query2ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.query2ItemSelectComboBox.Text = "";
        }
        private void query3ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.query3ItemSelectComboBox.Text = "";
        }

        private void query0FieldSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            query0FieldSelectComboBox.Text = query0FieldSelectComboBox.SelectedValue.ToString();
            ConfigureItemsSelection(query0FieldSelectComboBox.Text, query0ItemSelectComboBox);

        }

        private void query1FieldSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            query1FieldSelectComboBox.Text = query1FieldSelectComboBox.SelectedValue.ToString();

            ConfigureItemsSelection(query1FieldSelectComboBox.Text, query1ItemSelectComboBox);

        }

        private void query2FieldSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            query2FieldSelectComboBox.Text = query2FieldSelectComboBox.SelectedValue.ToString();

            ConfigureItemsSelection(query2FieldSelectComboBox.Text, query2ItemSelectComboBox);
        }

        private void query3FieldSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            query3FieldSelectComboBox.Text = query3FieldSelectComboBox.SelectedValue.ToString();

            ConfigureItemsSelection(query3FieldSelectComboBox.Text, query3ItemSelectComboBox);
        }

        private void ConfigureItemsSelection(string field, ComboBox b)
        {
            EnlistmentDC dc = this.viewModel.dc;
            switch (field)
            {
                case BuildTypeField:
                    {
                        var items = from c in dc.BuildTypes select c.Name;
                        b.ItemsSource = items;
                    }
                    break;
                case BuildXmlField:
                    {
                        var items = from c in dc.BuildXmls select c.FilePath;
                        b.ItemsSource = items;
                    }
                    break;
                case BuildCommandField:
                    {
                        var items = from c in dc.Commands select c.Command1;
                        b.ItemsSource = items;
                    }
                    break;
                case EmailField:
                    {
                        var items = from c in dc.Emails select c.EmailAccount;
                        b.ItemsSource = items;
                    }
                    break;
                case EnlistmentField:
                    {
                        var items = from c in dc.Enlistments select c.Path;
                        b.ItemsSource = items;
                    }
                    break;
                case PhoneSkuField:
                    {
                        var items = from c in dc.PhoneSKUs select c.Name;
                        b.ItemsSource = items;
                    }
                    break;
                case UserField:
                    {
                        var items = from c in dc.Users select c.LastName;
                        b.ItemsSource = items;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Query Processing


        private void queryButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<string> newFileContents = Enumerable.Empty<string>();
            IEnumerable<Configuration> cfg = Enumerable.Empty<Configuration>();
            if (query0ItemSelectComboBox.SelectedValue != null)
            {
                query0ItemSelectComboBox.Text = query0ItemSelectComboBox.SelectedValue.ToString();
                cfg = ExecuteQuery(cfg, query0FieldSelectComboBox.Text, query0ItemSelectComboBox.Text);
            }
            if (query1ItemSelectComboBox.SelectedValue != null)
            {
                query1ItemSelectComboBox.Text = query1ItemSelectComboBox.SelectedValue.ToString();
                cfg = ExecuteQuery(cfg, query1FieldSelectComboBox.Text, query1ItemSelectComboBox.Text);
            }
            if (query2ItemSelectComboBox.SelectedValue != null)
            {
                query2ItemSelectComboBox.Text = query2ItemSelectComboBox.SelectedValue.ToString();
                cfg = ExecuteQuery(cfg, query2FieldSelectComboBox.Text, query2ItemSelectComboBox.Text);
            }
            if (query3ItemSelectComboBox.SelectedValue != null)
            {
                query3ItemSelectComboBox.Text = query3ItemSelectComboBox.SelectedValue.ToString();
                cfg = ExecuteQuery(cfg, query3FieldSelectComboBox.Text, query3ItemSelectComboBox.Text);
            }

            Configuration[] configs = cfg.ToArray<Configuration>();
            this.configDataGridMan.ItemsSource = configs;
        }


        private IEnumerable<Configuration> ExecuteQuery(IEnumerable<Configuration> orig,
            string field,
            string item)
        {
            EnlistmentDC dc = this.viewModel.dc;
            IEnumerable<Configuration> source;
            if (orig.Count() == 0)
            {
                source = (from c in dc.Configurations select c).AsEnumerable<Configuration>();
            }
            else
            {
                source = orig;
            }
            IEnumerable<Configuration> cfg = null;
            switch (field)
            {
                case BuildTypeField:
                    {
                        cfg = from c in source
                              where c.BuildType.Name == item
                              select c;
                    }
                    break;
                case BuildXmlField:
                    {
                        cfg = from c in source
                              where c.BuildXml.FilePath == item
                              select c;
                    }
                    break;
                case BuildCommandField:
                    {
                        cfg = from c in source
                              where c.Command.Command1 == item
                              select c;
                    }
                    break;
                case EmailField:
                    {
                        cfg = from c in source
                              where c.Email.EmailAccount == item
                              select c;
                    }
                    break;
                case EnlistmentField:
                    {
                        cfg = from c in source
                              where c.Enlistment1.Path == item
                              select c;
                    }
                    break;
                case PhoneSkuField:
                    {
                        cfg = from c in source
                              where c.PhoneSKU.Name == item
                              select c;
                    }
                    break;
                case UserField:
                    {
                        cfg = from c in source
                              where c.User.LastName == item
                              select c;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return cfg;
        }


        #endregion



        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.mainWindow != null)
            {
                this.mainWindow.UseConfigurationSet = true;
                IEnumerable<Configuration> selectedItems = this.configDataGridMan.SelectedItems.OfType<Configuration>();
                if (selectedItems != null && selectedItems.Count() > 0)
                {
                    this.mainWindow.viewModel.SelectedOperationSet = selectedItems;
                }
                else
                {
                    this.mainWindow.viewModel.SelectedOperationSet = this.configDataGridMan.Items.OfType<Configuration>();
                }

                this.mainWindow.UpdateSelectedSet();
            }

            this.Close();

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.mainWindow != null)
            {
                this.mainWindow.UseConfigurationSet = false;
                this.mainWindow.viewModel.SelectedOperationSet = Enumerable.Empty<Configuration>();
            }
            this.Close();

        }
       

        internal void DeleteItem()
        {
            if (this.CurrentItemToEdit != null)
            {
                this.viewModel.dc.Configurations.DeleteOnSubmit(CurrentItemToEdit);
                this.viewModel.dc.SubmitChanges();
                this.queryButton_Click(null, null);
            }
        }

        public void DataGridPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                this.DeleteItem();
            }
        }

        protected void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.CurrentItemToEdit = e.AddedItems[0] as Configuration;
            }
            else
            {
                this.CurrentItemToEdit = null;
            }
            object source = e.Source;

        }

        protected void CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataRowView rowView = e.Row.Item as DataRowView;
            rowBeingEdited = rowView;

            foreach (DataGridCellInfo cellInfo in configDataGridMan.SelectedCells)
            {
                // this changes the cell's content not the data item behind it        
                DataGridCell gridCell = TryToFindGridCell(configDataGridMan, cellInfo);

                if (cellInfo.Column.Header.ToString() == DailyBuildHeader ||
                    cellInfo.Column.Header.ToString() == EnlistmentHeader ||
                    cellInfo.Column.Header.ToString() == PhoneSkuHeader   ||
                    cellInfo.Column.Header.ToString() == CommandHeader)
                {
                    if (gridCell != null)
                    {
                        TextBox tb = gridCell.Content as TextBox;
                        if (tb != null)
                        {
                            Configuration en;
                            en = (from c in this.viewModel.dc.Configurations
                                  where c.ID == CurrentItemToEdit.ID
                                  select c).First();
                            switch (cellInfo.Column.Header.ToString())
                            {
                                case DailyBuildHeader:
                                    en.DailyBuild = Convert.ToInt16(tb.Text);
                                    break;
                                case EnlistmentHeader:
                                    en.EnlistmentID = Convert.ToInt16(tb.Text);
                                    break;
                                case PhoneSkuHeader:
                                    en.PhoneSkuID = Convert.ToInt16(tb.Text);
                                    break;
                                case CommandHeader:
                                    en.CommandID = Convert.ToInt16(tb.Text);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public DataGridCell TryToFindGridCell(DataGrid grid, DataGridCellInfo cellInfo)
        {
            DataGridCell result = null;
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                }

            }
            return result;
        }

        public V GetVisualChild<V>(Visual parent) where V : Visual
        {
            V child = default(V);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as V;
                if (child == null)
                {
                    child = GetVisualChild<V>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }



        public DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = null; // GetRow(dataGrid, row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                // try to get the cell  but it may possibly be virtualized               
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }



        #region Build Set Function. May not be needed


        private void buildSetSelectChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        /// <summary>
        /// This will add the selected items to a set of items that can be added to later
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buildSetAddButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Configuration> selectedItems = this.configDataGridMan.SelectedItems.OfType<Configuration>();
            foreach (Configuration c in selectedItems)
            {
                this.userList.Add(c);
            }
        }

        private void buildSetShowButton_Click(object sender, RoutedEventArgs e)
        {
            this.configDataGridMan.ItemsSource = this.userList;
        }

        /// <summary>
        /// Clear the set of items 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buildSetDelButton_Click(object sender, RoutedEventArgs e)
        {
            userList.Clear();
            this.configDataGridMan.ItemsSource = null;
        }

        #endregion

        #region Test Functions




        private void resetDataContextButton_Click(object sender, RoutedEventArgs e)
        {
            Shared.ResetDataContext();
            EnlistmentDC dc = this.viewModel.dc;
            this.configDataGridMan.ItemsSource = this.viewModel.dc.Configurations;

        }


        private void addTestRowButton_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            short maxId = (short)(from s in dc.Configurations
                                  select s.ID).Max<short>();
            maxId += 1;
            this.testConfigItem = new Configuration
            {
                ID = maxId,
                UserID = 1,
                EmailID = 1,
                PhoneSkuID = 3,
                CommandID = 1,
                BuildXmlID = 1,
                EnlistmentID = 1,
                DailyBuild = 1
            };
            dc.Configurations.InsertOnSubmit(this.testConfigItem);
            dc.SubmitChanges();
            queryButton_Click(sender, e);

        }

        private void modifyTestRowButton_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            var query = (from s in dc.Configurations
                         where s.PhoneSkuID == 3
                         select s);
            var command = (from s in dc.Commands
                           where s.CommandID == 4
                           select s).First();

            foreach (Configuration c in query)
            {
                c.Command = command;
            }


            dc.SubmitChanges();
            queryButton_Click(sender, e);
        }


        private void deleteTestRowButton_Click(object sender, RoutedEventArgs e)
        {
            EnlistmentDC dc = this.viewModel.dc;
            var query = from s in dc.Configurations
                        where s.PhoneSkuID == 3
                        select s;
            dc.Configurations.DeleteAllOnSubmit(query);
            dc.SubmitChanges();
            queryButton_Click(sender, e);
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {

        }


        #endregion



    }
}
