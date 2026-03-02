namespace FolderCompare.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderCompare.Models;
using FolderCompare.Services;

public partial class MainViewModel : ObservableObject
{
    private readonly IFolderComparer _comparer = new FolderComparer();
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private string _leftFolderPath = string.Empty;

    [ObservableProperty]
    private string _rightFolderPath = string.Empty;

    [ObservableProperty]
    private bool _isComparing;

    [ObservableProperty]
    private int _progressPercent;

    [ObservableProperty]
    private string _statusText = "Ready";

    public ObservableCollection<ComparisonItem> AllItems { get; } = new();

    public ICollectionView FilteredItems { get; }

    public ObservableCollection<StatusFilterItem> StatusFilters { get; }

    public ObservableCollection<ComparisonTreeNode> UnifiedFlatItems { get; } = new();

    public MainViewModel()
    {
        StatusFilters = new ObservableCollection<StatusFilterItem>
        {
            new(ComparisonStatus.Identical, "Identical"),
            new(ComparisonStatus.Modified, "Modified"),
            new(ComparisonStatus.LeftOnly, "Left Only"),
            new(ComparisonStatus.RightOnly, "Right Only"),
            new(ComparisonStatus.DifferentSize, "Different Size"),
        };

        FilteredItems = CollectionViewSource.GetDefaultView(AllItems);
        FilteredItems.Filter = FilterItem;

        foreach (var filter in StatusFilters)
        {
            filter.FilterChanged = () => FilteredItems.Refresh();
        }
    }

    partial void OnLeftFolderPathChanged(string value)
    {
        CompareCommand.NotifyCanExecuteChanged();
    }

    partial void OnRightFolderPathChanged(string value)
    {
        CompareCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsComparingChanged(bool value)
    {
        CompareCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void BrowseLeft()
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog();
        if (dialog.ShowDialog() == true)
        {
            LeftFolderPath = dialog.FolderName;
        }
    }

    [RelayCommand]
    private void BrowseRight()
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog();
        if (dialog.ShowDialog() == true)
        {
            RightFolderPath = dialog.FolderName;
        }
    }

         [RelayCommand(CanExecute = nameof(CanCompare))]
         private async Task Compare()
         {
             if (!Directory.Exists(LeftFolderPath) || !Directory.Exists(RightFolderPath))
             {
                 StatusText = "One or both folder paths do not exist.";
                 return;
             }

             IsComparing = true;
             AllItems.Clear();
             UnifiedFlatItems.Clear();
             _cts = new CancellationTokenSource();

             var progress = new Progress<int>(percent => ProgressPercent = percent);

             try
             {
                 var results = await _comparer.CompareAsync(LeftFolderPath, RightFolderPath, progress, _cts.Token);

                 foreach (var item in results)
                 {
                     AllItems.Add(item);
                 }

                 // Build unified tree and flatten for list view
                 var unifiedTree = ComparisonTreeNode.BuildUnifiedTree(results);
                 var flatList = ComparisonTreeNode.FlattenTree(unifiedTree);
                 foreach (var node in flatList)
                 {
                     UnifiedFlatItems.Add(node);
                 }

                 foreach (var filter in StatusFilters)
                 {
                     filter.Count = AllItems.Count(i => i.Status == filter.Status);
                 }

                 StatusText = $"Comparison complete: {AllItems.Count} items found.";
             }
             catch (OperationCanceledException)
             {
                 StatusText = "Comparison cancelled.";
             }
             catch (Exception ex)
             {
                 StatusText = $"Error: {ex.Message}";
             }
             finally
             {
                 IsComparing = false;
                 _cts?.Dispose();
                 _cts = null;
             }
         }

    private bool CanCompare() =>
        !IsComparing
        && !string.IsNullOrWhiteSpace(LeftFolderPath)
        && !string.IsNullOrWhiteSpace(RightFolderPath);

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        _cts?.Cancel();
    }

    private bool CanCancel() => IsComparing;

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var filter in StatusFilters)
        {
            filter.IsChecked = true;
        }
    }

    [RelayCommand]
    private void SelectNone()
    {
        foreach (var filter in StatusFilters)
        {
            filter.IsChecked = false;
        }
    }

    private bool FilterItem(object obj)
    {
        if (obj is ComparisonItem item)
        {
            var filter = StatusFilters.FirstOrDefault(f => f.Status == item.Status);
            return filter?.IsChecked ?? true;
        }

        return false;
    }
}
