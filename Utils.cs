namespace LagomCms;

public static class Utils
{
    public static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
    
    public static bool IsSourcePage(string path)
    {
        var fileName = Path.GetFileName(path);
        if (fileName.Contains(' ') || fileName.Split('.').Length != 2)
        {
            return false;
        }
        var sourcePageFileExtensions = new[] { ".md", ".markdown" };
        var fileExtension = Path.GetExtension(path);
        return sourcePageFileExtensions.Any(s => fileExtension == s);
    }
    
    public static void ShowMessage(string text, ConsoleColor color = ConsoleColor.Yellow)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
    }
    
}