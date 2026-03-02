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

    private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListViewItem item && item.DataContext is ComparisonTreeNode node)
        {
            // Don't open diff for directories
            if (node.IsDirectory)
                return;

            // Ensure we have both paths
            if (string.IsNullOrEmpty(node.LeftFullPath) || string.IsNullOrEmpty(node.RightFullPath))
                return;

            var viewModel = new FileCompareViewModel(node.LeftFullPath, node.RightFullPath, node.RelativePath);
            var window = new FileCompareWindow(viewModel)
            {
                Owner = this
            };
            window.Show();
        }
    }
}