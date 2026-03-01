using System.IO;
using FolderCompare.Models;

namespace FolderCompare.Services;

/// <summary>
/// Computes a line-by-line diff between two files using a simple LCS (Longest Common Subsequence) algorithm.
/// </summary>
public static class FileDiffer
{
    /// <summary>
    /// Maximum number of lines per file for the full LCS algorithm.
    /// Files larger than this use a simpler line-by-line fallback.
    /// </summary>
    private const int MaxLcsLines = 10_000;

    /// <summary>
    /// Computes a diff between two arrays of lines.
    /// </summary>
    /// <param name="leftLines">Lines from the left file.</param>
    /// <param name="rightLines">Lines from the right file.</param>
    /// <returns>A list of <see cref="DiffLine"/> entries representing the diff.</returns>
    public static List<DiffLine> ComputeDiff(string[] leftLines, string[] rightLines)
    {
        ArgumentNullException.ThrowIfNull(leftLines);
        ArgumentNullException.ThrowIfNull(rightLines);

        if (leftLines.Length > MaxLcsLines || rightLines.Length > MaxLcsLines)
        {
            return ComputeSimpleDiff(leftLines, rightLines);
        }

        return ComputeLcsDiff(leftLines, rightLines);
    }

    /// <summary>
    /// Reads both files and computes a diff. Handles null paths and missing files gracefully.
    /// </summary>
    /// <param name="leftPath">Path to the left file, or null if it doesn't exist.</param>
    /// <param name="rightPath">Path to the right file, or null if it doesn't exist.</param>
    /// <returns>A list of <see cref="DiffLine"/> entries representing the diff.</returns>
    public static List<DiffLine> ComputeDiffFromFiles(string? leftPath, string? rightPath)
    {
        var leftLines = ReadFileLines(leftPath);
        var rightLines = ReadFileLines(rightPath);

        return ComputeDiff(leftLines, rightLines);
    }

    /// <summary>
    /// Reads all lines from a file, returning an empty array if the path is null or the file doesn't exist.
    /// </summary>
    private static string[] ReadFileLines(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return [];
        }

        try
        {
            return File.Exists(path) ? File.ReadAllLines(path) : [];
        }
        catch (IOException)
        {
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }
    }

    /// <summary>
    /// Computes a diff using the standard LCS dynamic-programming approach.
    /// </summary>
    private static List<DiffLine> ComputeLcsDiff(string[] leftLines, string[] rightLines)
    {
        int m = leftLines.Length;
        int n = rightLines.Length;

        // Build the LCS length table.
        var dp = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (leftLines[i - 1] == rightLines[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        // Back-track through the DP table to produce the diff.
        var result = new List<DiffLine>();
        int li = m;
        int ri = n;

        // We build the result in reverse, then reverse at the end.
        while (li > 0 || ri > 0)
        {
            if (li > 0 && ri > 0 && leftLines[li - 1] == rightLines[ri - 1])
            {
                // Unchanged — line is in both files.
                result.Add(new DiffLine
                {
                    LeftLineNumber = li,
                    RightLineNumber = ri,
                    LeftText = leftLines[li - 1],
                    RightText = rightLines[ri - 1],
                    Type = DiffLineType.Unchanged
                });
                li--;
                ri--;
            }
            else if (ri > 0 && (li == 0 || dp[li, ri - 1] >= dp[li - 1, ri]))
            {
                // Added — line exists only in right.
                result.Add(new DiffLine
                {
                    LeftLineNumber = null,
                    RightLineNumber = ri,
                    LeftText = string.Empty,
                    RightText = rightLines[ri - 1],
                    Type = DiffLineType.Added
                });
                ri--;
            }
            else
            {
                // Deleted — line exists only in left.
                result.Add(new DiffLine
                {
                    LeftLineNumber = li,
                    RightLineNumber = null,
                    LeftText = leftLines[li - 1],
                    RightText = string.Empty,
                    Type = DiffLineType.Deleted
                });
                li--;
            }
        }

        result.Reverse();
        return result;
    }

    /// <summary>
    /// Simple line-by-line comparison fallback for files exceeding <see cref="MaxLcsLines"/>.
    /// Compares corresponding lines and marks extras as Added or Deleted.
    /// </summary>
    private static List<DiffLine> ComputeSimpleDiff(string[] leftLines, string[] rightLines)
    {
        var result = new List<DiffLine>();
        int commonLength = Math.Min(leftLines.Length, rightLines.Length);

        // Compare corresponding lines.
        for (int i = 0; i < commonLength; i++)
        {
            if (leftLines[i] == rightLines[i])
            {
                result.Add(new DiffLine
                {
                    LeftLineNumber = i + 1,
                    RightLineNumber = i + 1,
                    LeftText = leftLines[i],
                    RightText = rightLines[i],
                    Type = DiffLineType.Unchanged
                });
            }
            else
            {
                result.Add(new DiffLine
                {
                    LeftLineNumber = i + 1,
                    RightLineNumber = i + 1,
                    LeftText = leftLines[i],
                    RightText = rightLines[i],
                    Type = DiffLineType.Modified
                });
            }
        }

        // Extra lines only in left → Deleted.
        for (int i = commonLength; i < leftLines.Length; i++)
        {
            result.Add(new DiffLine
            {
                LeftLineNumber = i + 1,
                RightLineNumber = null,
                LeftText = leftLines[i],
                RightText = string.Empty,
                Type = DiffLineType.Deleted
            });
        }

        // Extra lines only in right → Added.
        for (int i = commonLength; i < rightLines.Length; i++)
        {
            result.Add(new DiffLine
            {
                LeftLineNumber = null,
                RightLineNumber = i + 1,
                LeftText = string.Empty,
                RightText = rightLines[i],
                Type = DiffLineType.Added
            });
        }

        return result;
    }
}
