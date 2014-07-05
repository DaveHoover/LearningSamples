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
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Data;

namespace BuildManager.View
{
    public class DbTableModifyBase  : Window 
    {

        protected MainWindow mainWindow = null;

        protected DataRowView rowBeingEdited = null;

        public string ColumnNameForEdit { get; set; }

        protected DataGrid datagrid { get; set; }

        public DbTableViewModelBase viewModel;


        public DbTableModifyBase () : base ()
        {

        }


        protected void OkButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected void CancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       

        protected void modButton_Clicked(object sender, RoutedEventArgs e)
        {
        }

        protected void delButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.viewModel.DeleteItem();
        }

      

        protected virtual void CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataRowView rowView = e.Row.Item as DataRowView;
            rowBeingEdited = rowView;

            foreach (DataGridCellInfo cellInfo in datagrid.SelectedCells)
            {
                if (cellInfo.Column.Header.ToString() == this.ColumnNameForEdit)
                {
                    // this changes the cell's content not the data item behind it        
                    DataGridCell gridCell = TryToFindGridCell(datagrid, cellInfo);
                    if (gridCell != null)
                    {
                        TextBox tb = gridCell.Content as TextBox;
                        if (tb != null)
                        {
                            this.viewModel.ModifyItem(tb.Text);
                        }
                    }
                }
            }
        }



        public void DataGridPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                this.viewModel.DeleteItem();
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


    }
}
