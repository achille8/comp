namespace FolderCompare.Models;

/// <summary>
/// Represents a single line in the diff view.
/// </summary>
public class DiffLine
{
    /// <summary>1-based line number in left file, null if line doesn't exist on left.</summary>
    public int? LeftLineNumber { get; init; }

    /// <summary>1-based line number in right file, null if line doesn't exist on right.</summary>
    public int? RightLineNumber { get; init; }

    /// <summary>Text content from left file (empty string if missing).</summary>
    public string LeftText { get; init; } = string.Empty;

    /// <summary>Text content from right file (empty string if missing).</summary>
    public string RightText { get; init; } = string.Empty;

    /// <summary>The type of difference.</summary>
    public DiffLineType Type { get; init; }
}
