namespace FolderCompare.Models;

/// <summary>
/// Represents the result of comparing a file or folder between left and right directories.
/// </summary>
public enum ComparisonStatus
{
    /// <summary>Files are identical in content and metadata.</summary>
    Identical,

    /// <summary>Files exist on both sides but have different content.</summary>
    Modified,

    /// <summary>Item exists only in the left (source) directory.</summary>
    LeftOnly,

    /// <summary>Item exists only in the right (target) directory.</summary>
    RightOnly,

    /// <summary>Files exist on both sides but differ in size.</summary>
    DifferentSize
}
