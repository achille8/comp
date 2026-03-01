using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FolderCompare.Models;
using FolderCompare.Services;

namespace FolderCompare.ViewModels;

public partial class FileCompareViewModel : ObservableObject
{
    public string WindowTitle { get; }
    public string LeftFilePath { get; }
    public string RightFilePath { get; }
    public ObservableCollection<DiffLine> DiffLines { get; }
    public string SummaryText { get; }

    public FileCompareViewModel(string? leftPath, string? rightPath, string relativePath)
    {
        LeftFilePath = leftPath ?? string.Empty;
        RightFilePath = rightPath ?? string.Empty;
        WindowTitle = "File Compare \u2014 " + relativePath;

        var lines = FileDiffer.ComputeDiffFromFiles(leftPath, rightPath);
        DiffLines = new ObservableCollection<DiffLine>(lines);

        int additions = 0, deletions = 0, modifications = 0;
        foreach (var line in lines)
        {
            switch (line.Type)
            {
                case DiffLineType.Added: additions++; break;
                case DiffLineType.Deleted: deletions++; break;
                case DiffLineType.Modified: modifications++; break;
            }
        }

        var parts = new List<string>();
        if (additions > 0) parts.Add($"{additions} addition{(additions == 1 ? "" : "s")}");
        if (deletions > 0) parts.Add($"{deletions} deletion{(deletions == 1 ? "" : "s")}");
        if (modifications > 0) parts.Add($"{modifications} modification{(modifications == 1 ? "" : "s")}");

        SummaryText = parts.Count > 0 ? string.Join(", ", parts) : "No differences";
    }
}
