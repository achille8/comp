namespace FolderCompare.Services;

using FolderCompare.Models;

public interface IFolderComparer
{
    Task<IReadOnlyList<ComparisonItem>> CompareAsync(
        string leftPath,
        string rightPath,
        IProgress<int>? progress,
        CancellationToken ct);
}
