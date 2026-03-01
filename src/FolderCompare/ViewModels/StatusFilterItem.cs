namespace FolderCompare.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using FolderCompare.Models;

public partial class StatusFilterItem : ObservableObject
{
    public ComparisonStatus Status { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _isChecked = true;

    [ObservableProperty]
    private int _count;

    public Action? FilterChanged { get; set; }

    public StatusFilterItem(ComparisonStatus status, string label)
    {
        Status = status;
        Label = label;
    }

    partial void OnIsCheckedChanged(bool value)
    {
        FilterChanged?.Invoke();
    }
}
