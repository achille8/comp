using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FolderCompare.Models;
using FolderCompare.ViewModels;

namespace FolderCompare;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGridRow row && row.DataContext is ComparisonItem item)
        {
            // Don't open diff for directories
            if (item.IsDirectory)
                return;

            var viewModel = new FileCompareViewModel(item.LeftFullPath, item.RightFullPath, item.RelativePath);
            var window = new FileCompareWindow(viewModel)
            {
                Owner = this
            };
            window.Show();
        }
    }
}