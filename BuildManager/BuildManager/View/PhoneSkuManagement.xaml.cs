using System.Windows;
using BuildManager.Data;
using BuildManager.ViewModel;
using System.Windows.Controls;

namespace BuildManager.View
{
    /// <summary>
    /// Interaction logic for PhoneSkuManagement.xaml
    /// </summary>
    public partial class PhoneSkuManagement : DbTableModifyBase
    {
        PhoneSkuManagementViewModel vm = new PhoneSkuManagementViewModel("PhoneSku");

        public PhoneSkuManagement(MainWindow window ) :base()
        {
            this.mainWindow = window;
            base.viewModel = vm;
            this.ColumnNameForEdit = "Name";
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
                this.vm.CurrentItemToEdit = e.AddedItems[0] as PhoneSKU;
            }
            else
            {
                this.vm.CurrentItemToEdit = null;
            }
            object source = e.Source;

        }
    }


}
