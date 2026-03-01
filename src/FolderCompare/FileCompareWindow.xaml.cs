using System.Windows;
using FolderCompare.ViewModels;

namespace FolderCompare;

public partial class FileCompareWindow : Window
{
    public FileCompareWindow(FileCompareViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
