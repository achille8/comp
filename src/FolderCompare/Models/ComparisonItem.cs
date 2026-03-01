namespace FolderCompare.Models;

/// <summary>
/// Represents a single file or folder entry resulting from a folder comparison.
/// </summary>
public class ComparisonItem
{
    /// <summary>File or folder name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Path relative to the compared root.</summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>Whether this item represents a directory.</summary>
    public bool IsDirectory { get; set; }

    /// <summary>Comparison result status.</summary>
    public ComparisonStatus Status { get; set; }

    /// <summary>File size in the left directory, if present.</summary>
    public long? LeftSize { get; set; }

    /// <summary>File size in the right directory, if present.</summary>
    public long? RightSize { get; set; }

    /// <summary>Last modified date in the left directory, if present.</summary>
    public DateTime? LeftModified { get; set; }

    /// <summary>Last modified date in the right directory, if present.</summary>
    public DateTime? RightModified { get; set; }

    /// <summary>Full path to the file in the left directory, if present.</summary>
    public string? LeftFullPath { get; set; }

    /// <summary>Full path to the file in the right directory, if present.</summary>
    public string? RightFullPath { get; set; }
}
