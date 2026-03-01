namespace FolderCompare.Services;

using System.IO;
using FolderCompare.Models;

public class FolderComparer : IFolderComparer
{
    private static readonly TimeSpan TimeTolerance = TimeSpan.FromSeconds(2);

    public async Task<IReadOnlyList<ComparisonItem>> CompareAsync(
        string leftPath,
        string rightPath,
        IProgress<int>? progress,
        CancellationToken ct)
    {
        var leftEntries = EnumerateFileSystem(leftPath);
        var rightEntries = EnumerateFileSystem(rightPath);

        var allKeys = new SortedSet<string>(
            leftEntries.Keys.Union(rightEntries.Keys),
            StringComparer.OrdinalIgnoreCase);

        var results = new List<ComparisonItem>(allKeys.Count);
        int processed = 0;
        int total = allKeys.Count;

        foreach (var key in allKeys)
        {
            ct.ThrowIfCancellationRequested();

            bool inLeft = leftEntries.TryGetValue(key, out var leftInfo);
            bool inRight = rightEntries.TryGetValue(key, out var rightInfo);

            try
            {
                var item = BuildComparisonItem(key, inLeft, leftInfo, inRight, rightInfo, leftPath, rightPath);

                if (item.Status == ComparisonStatus.Modified)
                {
                    // Sizes match but times differ — hash to confirm
                    await ResolveByHashAsync(item, ct).ConfigureAwait(false);
                }

                results.Add(item);
            }
            catch (UnauthorizedAccessException) { }
            catch (PathTooLongException) { }
            catch (IOException) { }

            processed++;
            progress?.Report(total == 0 ? 100 : (int)((long)processed * 100 / total));
        }

        return results;
    }

    private static Dictionary<string, FileSystemInfo> EnumerateFileSystem(string rootPath)
    {
        var map = new Dictionary<string, FileSystemInfo>(StringComparer.OrdinalIgnoreCase);
        var root = new DirectoryInfo(rootPath);

        if (!root.Exists)
            return map;

        try
        {
            foreach (var entry in root.EnumerateFileSystemInfos("*", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.ReparsePoint
            }))
            {
                try
                {
                    // Skip symbolic links / reparse points to avoid cycles
                    if (entry.Attributes.HasFlag(FileAttributes.ReparsePoint))
                        continue;

                    string relativePath = Path.GetRelativePath(rootPath, entry.FullName);
                    map[relativePath] = entry;
                }
                catch (UnauthorizedAccessException) { }
                catch (PathTooLongException) { }
                catch (IOException) { }
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (PathTooLongException) { }
        catch (IOException) { }

        return map;
    }

    private static ComparisonItem BuildComparisonItem(
        string relativePath,
        bool inLeft, FileSystemInfo? leftInfo,
        bool inRight, FileSystemInfo? rightInfo,
        string leftRoot, string rightRoot)
    {
        bool isDirectory = (inLeft && leftInfo is DirectoryInfo) || (inRight && rightInfo is DirectoryInfo);

        var item = new ComparisonItem
        {
            Name = Path.GetFileName(relativePath),
            RelativePath = relativePath,
            IsDirectory = isDirectory,
            LeftFullPath = inLeft ? leftInfo!.FullName : null,
            RightFullPath = inRight ? rightInfo!.FullName : null
        };

        if (inLeft && leftInfo is FileInfo leftFile)
        {
            item.LeftSize = leftFile.Length;
            item.LeftModified = leftFile.LastWriteTimeUtc;
        }
        else if (inLeft && leftInfo is DirectoryInfo)
        {
            item.LeftModified = leftInfo.LastWriteTimeUtc;
        }

        if (inRight && rightInfo is FileInfo rightFile)
        {
            item.RightSize = rightFile.Length;
            item.RightModified = rightFile.LastWriteTimeUtc;
        }
        else if (inRight && rightInfo is DirectoryInfo)
        {
            item.RightModified = rightInfo.LastWriteTimeUtc;
        }

        if (inLeft && !inRight)
        {
            item.Status = ComparisonStatus.LeftOnly;
        }
        else if (!inLeft && inRight)
        {
            item.Status = ComparisonStatus.RightOnly;
        }
        else if (isDirectory)
        {
            item.Status = ComparisonStatus.Identical;
        }
        else
        {
            // Both are files
            if (item.LeftSize != item.RightSize)
            {
                item.Status = ComparisonStatus.DifferentSize;
            }
            else if (item.LeftModified.HasValue && item.RightModified.HasValue &&
                     (item.LeftModified.Value - item.RightModified.Value).Duration() <= TimeTolerance)
            {
                item.Status = ComparisonStatus.Identical;
            }
            else
            {
                // Sizes match but times differ — needs hashing (marked Modified as provisional)
                item.Status = ComparisonStatus.Modified;
            }
        }

        return item;
    }

    private static async Task ResolveByHashAsync(ComparisonItem item, CancellationToken ct)
    {
        if (item.LeftFullPath is null || item.RightFullPath is null)
            return;

        byte[] leftHash = await FileHasher.ComputeHashAsync(item.LeftFullPath, ct).ConfigureAwait(false);
        byte[] rightHash = await FileHasher.ComputeHashAsync(item.RightFullPath, ct).ConfigureAwait(false);

        item.Status = leftHash.AsSpan().SequenceEqual(rightHash)
            ? ComparisonStatus.Identical
            : ComparisonStatus.Modified;
    }
}
