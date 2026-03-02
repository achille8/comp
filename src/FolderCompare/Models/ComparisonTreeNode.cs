namespace FolderCompare.Models;

using System.Collections.ObjectModel;
using System.IO;

/// <summary>
/// Represents a node in the folder comparison tree structure.
/// Can be either a directory (with children) or a file (leaf node).
/// </summary>
public class ComparisonTreeNode
{
    /// <summary>Name of the file or folder.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Path relative to the root folder.</summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>Whether this node represents a directory.</summary>
    public bool IsDirectory { get; set; }

    /// <summary>File size (for files only).</summary>
    public long? Size { get; set; }

    /// <summary>Last modified date (for files only).</summary>
    public DateTime? Modified { get; set; }

    /// <summary>File size in left folder (for files only).</summary>
    public long? LeftSize { get; set; }

    /// <summary>File size in right folder (for files only).</summary>
    public long? RightSize { get; set; }

    /// <summary>Last modified date in left folder (for files only).</summary>
    public DateTime? LeftModified { get; set; }

    /// <summary>Last modified date in right folder (for files only).</summary>
    public DateTime? RightModified { get; set; }

    /// <summary>File extension in left folder (for files only).</summary>
    public string? LeftExtension { get; set; }

    /// <summary>File extension in right folder (for files only).</summary>
    public string? RightExtension { get; set; }

    /// <summary>Comparison status (for files only).</summary>
    public ComparisonStatus Status { get; set; }

    /// <summary>Full path to the file in the left folder (if exists).</summary>
    public string? LeftFullPath { get; set; }

    /// <summary>Full path to the file in the right folder (if exists).</summary>
    public string? RightFullPath { get; set; }

    /// <summary>Child nodes (for directories).</summary>
    public ObservableCollection<ComparisonTreeNode> Children { get; } = new();

    /// <summary>Parent node (null for root).</summary>
    public ComparisonTreeNode? Parent { get; set; }

    /// <summary>Depth in the tree (0 for root).</summary>
    public int Depth { get; set; }

    /// <summary>
    /// Builds a hierarchical tree structure from a flat list of ComparisonItem objects.
    /// </summary>
    /// <param name="items">Flat list of comparison items</param>
    /// <param name="isLeftTree">If true, builds tree for left folder; otherwise for right folder</param>
    /// <returns>Root node containing the entire tree structure</returns>
    public static ComparisonTreeNode BuildTree(IEnumerable<ComparisonItem> items, bool isLeftTree)
    {
        var root = new ComparisonTreeNode
        {
            Name = isLeftTree ? "Left Folder" : "Right Folder",
            RelativePath = "",
            IsDirectory = true,
            Parent = null
        };

        foreach (var item in items)
        {
            // Skip items that don't exist on this side
            if (isLeftTree && item.LeftFullPath == null) continue;
            if (!isLeftTree && item.RightFullPath == null) continue;

            var relativePath = isLeftTree ? item.RelativePath : item.RelativePath;
            
            // Split the path into parts
            var parts = relativePath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            
            var currentNode = root;
            var currentPath = "";
            
            // Build the path hierarchy
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                currentPath = i == 0 ? part : Path.Combine(currentPath, part);
                
                // Find or create the child node
                var child = currentNode.Children.FirstOrDefault(c => c.Name == part);
                if (child == null)
                {
                    child = new ComparisonTreeNode
                    {
                        Name = part,
                        RelativePath = currentPath,
                        IsDirectory = i < parts.Length - 1, // intermediate nodes are directories
                        Parent = currentNode
                    };
                    currentNode.Children.Add(child);
                }
                
                // If this is the last part (the item itself), update its properties from the item
                if (i == parts.Length - 1)
                {
                    child.IsDirectory = item.IsDirectory;
                    child.Size = isLeftTree ? item.LeftSize : item.RightSize;
                    child.Modified = isLeftTree ? item.LeftModified : item.RightModified;
                    child.Status = item.Status;
                    child.LeftFullPath = item.LeftFullPath;
                    child.RightFullPath = item.RightFullPath;
                    if (isLeftTree && item.LeftFullPath != null)
                    {
                        child.LeftExtension = Path.GetExtension(item.LeftFullPath).TrimStart('.').ToUpper();
                    }
                    if (!isLeftTree && item.RightFullPath != null)
                    {
                        child.RightExtension = Path.GetExtension(item.RightFullPath).TrimStart('.').ToUpper();
                    }
                }
                
                currentNode = child;
            }
        }

        // Sort children: directories first, then files; both alphabetically
        SortChildren(root);
        
        return root;
    }

    /// <summary>
    /// Builds a unified hierarchical tree structure from a flat list of ComparisonItem objects.
    /// Each node contains both left and right folder information.
    /// </summary>
    /// <param name="items">Flat list of comparison items</param>
    /// <returns>Root node containing the entire unified tree structure</returns>
    public static ComparisonTreeNode BuildUnifiedTree(IEnumerable<ComparisonItem> items)
    {
        var root = new ComparisonTreeNode
        {
            Name = "Comparison",
            RelativePath = "",
            IsDirectory = true,
            Parent = null
        };

        foreach (var item in items)
        {
            var relativePath = item.RelativePath;
            
            // Split the path into parts
            var parts = relativePath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            
            var currentNode = root;
            var currentPath = "";
            
            // Build the path hierarchy
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                currentPath = i == 0 ? part : Path.Combine(currentPath, part);
                
                // Find or create the child node
                var child = currentNode.Children.FirstOrDefault(c => c.Name == part);
                if (child == null)
                {
                    child = new ComparisonTreeNode
                    {
                        Name = part,
                        RelativePath = currentPath,
                        IsDirectory = i < parts.Length - 1, // intermediate nodes are directories
                        Parent = currentNode,
                        LeftSize = null,
                        RightSize = null,
                        LeftModified = null,
                        RightModified = null
                    };
                    currentNode.Children.Add(child);
                }
                
                // If this is the last part (the item itself), update its properties from the item
                if (i == parts.Length - 1)
                {
                    child.IsDirectory = item.IsDirectory;
                    child.LeftSize = item.LeftSize;
                    child.RightSize = item.RightSize;
                    child.LeftModified = item.LeftModified;
                    child.RightModified = item.RightModified;
                    child.Status = item.Status;
                    child.LeftFullPath = item.LeftFullPath;
                    child.RightFullPath = item.RightFullPath;
                    child.LeftExtension = item.LeftFullPath != null ? Path.GetExtension(item.LeftFullPath).TrimStart('.').ToUpper() : null;
                    child.RightExtension = item.RightFullPath != null ? Path.GetExtension(item.RightFullPath).TrimStart('.').ToUpper() : null;
                    // For compatibility, set Size and Modified to left values (or right if left is null)
                    child.Size = item.LeftSize ?? item.RightSize;
                    child.Modified = item.LeftModified ?? item.RightModified;
                }
                
                currentNode = child;
            }
        }

        // Sort children: directories first, then files; both alphabetically
        SortChildren(root);
        
        return root;
    }

    /// <summary>
    /// Flattens the tree into a list with depth information for list view display.
    /// </summary>
    /// <param name="root">The root node of the tree</param>
    /// <returns>Flattened list of all nodes with depth set</returns>
    public static List<ComparisonTreeNode> FlattenTree(ComparisonTreeNode root)
    {
        var result = new List<ComparisonTreeNode>();
        FlattenNode(root, 0, result);
        return result;
    }

    private static void FlattenNode(ComparisonTreeNode node, int depth, List<ComparisonTreeNode> result)
    {
        // Create a copy with depth set (we don't want to modify the original tree structure)
        var nodeCopy = new ComparisonTreeNode
        {
            Name = node.Name,
            RelativePath = node.RelativePath,
            IsDirectory = node.IsDirectory,
            Size = node.Size,
            Modified = node.Modified,
            Status = node.Status,
            LeftFullPath = node.LeftFullPath,
            RightFullPath = node.RightFullPath,
            LeftExtension = node.LeftExtension,
            RightExtension = node.RightExtension,
            Depth = depth,
            Parent = null
        };

        result.Add(nodeCopy);

        // Recursively add children with incremented depth
        foreach (var child in node.Children)
        {
            FlattenNode(child, depth + 1, result);
        }
    }

    private static void SortChildren(ComparisonTreeNode node)
    {
        var sorted = node.Children.OrderBy(c => c.IsDirectory ? 0 : 1)
                                   .ThenBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
                                   .ToList();
        
        node.Children.Clear();
        foreach (var child in sorted)
        {
            node.Children.Add(child);
            SortChildren(child); // Recursively sort
        }
    }
}
