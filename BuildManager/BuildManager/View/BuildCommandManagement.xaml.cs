using System.Windows;
using BuildManager.Data;
using BuildManager.ViewModel;
using System.Windows.Controls;
using System.Data;

namespace BuildManager.View
{
    /// <summary>
    /// Interaction logic for BuildCommandManagement.xaml
    /// </summary>
    public partial class BuildCommandManagement : DbTableModifyBase
    {
        BuildCommandManagementViewModel vm = new BuildCommandManagementViewModel("Build Commands");

        private string CommandColumn = "Command";

        public BuildCommandManagement(MainWindow window) : base()
        {
            this.mainWindow = window;
            base.viewModel = vm;
            this.ColumnNameForEdit = "CommandLine";
            InitializeComponent();
            this.DataContext = this.viewModel;
            this.datagrid = this.dataGrid1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.viewModel.QueryAllDbTable();
            this.dataGrid1.ItemsSource = this.vm.DbTable;
        }

        protected void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        protected void addButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.viewModel.AddItem();
            this.dataGrid1.ItemsSource = this.vm.DbTable;
        }

        protected void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.vm.CurrentItemToEdit = e.AddedItems[0] as Command;
            }
            else
            {
                this.vm.CurrentItemToEdit = null;
            }
            object source = e.Source;

        }

        protected override void CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataRowView rowView = e.Row.Item as DataRowView;
            rowBeingEdited = rowView;

            foreach (DataGridCellInfo cellInfo in datagrid.SelectedCells)
            {
                // this changes the cell's content not the data item behind it        
                DataGridCell gridCell = TryToFindGridCell(datagrid, cellInfo);

                if (cellInfo.Column.Header.ToString() == this.ColumnNameForEdit ||
                    cellInfo.Column.Header.ToString() == CommandColumn)
                {
                    if (gridCell != null)
                    {
                        TextBox tb = gridCell.Content as TextBox;
                        if (tb != null)
                        {
                            if (cellInfo.Column.Header.ToString() == this.ColumnNameForEdit)
                            { 
                            this.viewModel.ModifyItem(tb.Text);
                            }
                            if ( cellInfo.Column.Header.ToString() == CommandColumn)
                            {
                                this.vm.ModifyItemCommand(tb.Text);
                            }
                        }
                    }
                }
            }
        }


    }
}
