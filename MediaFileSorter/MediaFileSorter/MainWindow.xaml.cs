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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaFileSorter.ViewModel;
using System.Diagnostics;
using MediaFileSorter.View;

namespace MediaFileSorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.sourceListBox.ItemsSource = this.viewModel.SourceFileList;
            this.targetListBox.ItemsSource = this.viewModel.TargetFileList;
            this.viewModel.RescanButton(false);
        }

        private void EnableUIControls(bool enable)
        {
            this.Cursor = enable ? Cursors.Arrow : Cursors.Wait;
            this.applyRecursivelyCheckBox.IsEnabled = enable;
            this.corretFilesForNumericalSortcheckBox.IsEnabled = enable;
            this.clearSelectionButton.IsEnabled = enable;
            this.rescanButton.IsEnabled = enable;
            this.updateNamesButton.IsEnabled = enable;
            this.createNumericalGroupBox.IsEnabled = enable;
            this.searchReplaceGroupBox.IsEnabled = enable;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void menuFileExit_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuHelpAbout_Clicked(object sender, RoutedEventArgs e)
        {
            MediaFileSorterAboutBox b = new MediaFileSorterAboutBox();
            b.ShowDialog();
        }



        private void rescanButton_Click(object sender, RoutedEventArgs e)
        {
            this.EnableUIControls(false);
            this.viewModel.RescanButton(false);
            this.EnableUIControls(true);
        }

        private void updateNamesButton_Click(object sender, RoutedEventArgs e)
        {
            this.EnableUIControls(false);
            this.viewModel.UpdateNames(false);
            this.EnableUIControls(true);
        }

        private void createNumericalListButton_Click(object sender, RoutedEventArgs e)
        {
            this.EnableUIControls(false);
            this.viewModel.CreateNumericalList(false);
            this.EnableUIControls(true);
        }

        private void searchUpdatebutton_Click(object sender, RoutedEventArgs e)
        {
            this.EnableUIControls(false);
            this.viewModel.SearchUpdate(false);
            this.EnableUIControls(true);
        }

        private void OnSourceFolderGotFocus(object sender, RoutedEventArgs e)
        {
            this.viewModel.SelectFolder(true);
        }

        private void OnTargetFolderGotFocus(object sender, RoutedEventArgs e)
        {
            this.viewModel.SelectFolder(false);
        }

        private void OnSourceItemsSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = this.sourceListBox.SelectedItems.Count;
            Debug.WriteLine("Selected item count = " + count.ToString());
            this.viewModel.selectedFileList.Clear();
            foreach ( string s in this.sourceListBox.SelectedItems)
            {
                this.viewModel.selectedFileList.Add(s);
            }
            this.viewModel.SelectedItemsFromUI = this.viewModel.selectedFileList.Count > 0 ? true : false;
        }

        private void clearSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            this.sourceListBox.SelectedItems.Clear();
            this.viewModel.SelectedItemsFromUI = false;
            this.viewModel.selectedFileList.Clear();
        }
    }
}

