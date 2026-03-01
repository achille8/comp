namespace FolderCompare.Models;

/// <summary>
/// Type of line in a diff comparison.
/// </summary>
public enum DiffLineType
{
    /// <summary>Line is the same in both files.</summary>
    Unchanged,

    /// <summary>Line exists only in the left file (deleted from right).</summary>
    Deleted,

    /// <summary>Line exists only in the right file (added to right).</summary>
    Added,

    /// <summary>Line was modified between left and right.</summary>
    Modified
}
