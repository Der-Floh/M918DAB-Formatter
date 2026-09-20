namespace M918DAB_Formatter.Utils;

public static class LoadFileHelper
{
    public static void LoadFolderStructure(string? path, TreeView treeView, ulong maxItems = 10_000, Action<double>? progressCallback = null)
    {
        try
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                if (treeView.InvokeRequired)
                    treeView.Invoke(new Action(treeView.Nodes.Clear));
                else
                    treeView.Nodes.Clear();
                return;
            }

            var rootDir = new DirectoryInfo(path);
            var rootNode = new TreeNode(rootDir.Name) { Tag = rootDir };

            var totalItems = progressCallback is null ? 0 : CountItems(path!, maxItems);
            totalItems = Math.Min(totalItems, maxItems);
            ulong processedItems = 0;

            Populate(rootDir, rootNode, progressCallback, ref processedItems, totalItems, maxItems);
            ReportProgress(progressCallback, processedItems, totalItems);
            if (treeView.InvokeRequired)
            {
                treeView.Invoke(new Action(() =>
                {
                    treeView.BeginUpdate();
                    treeView.Nodes.Clear();
                    treeView.Nodes.Add(rootNode);
                    treeView.EndUpdate();
                }));
            }
            else
            {
                treeView.BeginUpdate();
                treeView.Nodes.Clear();
                treeView.Nodes.Add(rootNode);
                treeView.EndUpdate();
            }

            ExpandTreeToLevel(treeView, 1);
        }
        catch { }
    }

    private static void Populate(DirectoryInfo dirInfo, TreeNode parent, Action<double>? progressCallback, ref ulong processed, ulong total, ulong maxItems)
    {
        var updateStep = Math.Max(1, (ulong)(total * 0.004));

        IEnumerable<FileSystemInfo> entries = FATHelper.EnumerateInKenwoodOrder(dirInfo);

        foreach (var fsi in entries/*dirInfo.EnumerateFileSystemInfos()*/)
        {
            try
            {
                var child = new TreeNode(fsi.Name) { Tag = fsi };
                parent.Nodes.Add(child);

                if ((fsi.Attributes & FileAttributes.Directory) != 0)
                {
                    Populate((DirectoryInfo)fsi, child, progressCallback, ref processed, total, maxItems);
                }

                // progress every 256 items (tune to taste)
                if (progressCallback is not null && (++processed % updateStep) == 0)
                    ReportProgress(progressCallback, processed, total);

                if (processed >= maxItems)
                {
                    processed = total; // cap at 10,000 processed items
                    break;
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (PathTooLongException) { }
        }

        ReportProgress(progressCallback, processed, total);
    }

    private static void ExpandTreeToLevel(TreeView treeView, int level)
    {
        if (treeView.InvokeRequired)
        {
            treeView.Invoke(new Action(() => ExpandTreeToLevel(treeView, level)));
            return;
        }

        foreach (TreeNode node in treeView.Nodes)
        {
            ExpandNodeToLevel(node, 1, level);
        }
    }

    private static void ExpandNodeToLevel(TreeNode node, int currentLevel, int maxLevel)
    {
        if (currentLevel <= maxLevel)
        {
            node.Expand();

            foreach (TreeNode child in node.Nodes)
            {
                ExpandNodeToLevel(child, currentLevel + 1, maxLevel);
            }
        }
    }

    private static void ReportProgress(Action<double>? progressCallback, ulong processed, ulong total)
    {
        if (progressCallback is null || total == 0)
            return;
        progressCallback((double)processed / total);
    }

    private static ulong CountItems(string path, ulong maxItems)
    {
        ulong count = 0;
        var pending = new Stack<string>();
        pending.Push(path);

        while (pending.Count != 0)
        {
            var dir = pending.Pop();

            try
            {
                foreach (var fsi in new DirectoryInfo(dir).EnumerateFileSystemInfos())
                {
                    if ((fsi.Attributes & FileAttributes.Directory) != 0)
                        pending.Push(fsi.FullName);
                    count++;

                    if (count >= maxItems)
                        return count; // cap at maxItems
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (PathTooLongException) { }
        }

        return count;
    }
}
