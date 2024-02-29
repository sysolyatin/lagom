namespace LagomCms;

public class CategoryInfo
{
    public string Title { get; }
    public string RawContent { get; }
    public string CategoryUrl { get; }

    public string HtmlFileName => Path.Combine(CategoryUrl, "index.html");

    private CategoryInfo(string title, string rawContent, string categoryUrl)
    {
        Title = title;
        RawContent = rawContent;
        CategoryUrl = categoryUrl;
    }

    public static CategoryInfo CreateFromFile(string filePath)
    {
        var fileContent = File.ReadAllText(filePath);
        
        var categoryUrl = Path.GetFileName(filePath).Split(".")[0];
        var title = categoryUrl;
        var rawContent = fileContent;
        
        var fileContentParts = fileContent
            .Replace("\r", "")
            .Split("\n----------\n\n");
        if (fileContentParts.Length != 2) return new CategoryInfo(title, rawContent, categoryUrl);
        rawContent = fileContentParts[1];
        var paramsDict = fileContentParts[0]
            .Split('\n')
            .Select(a => a.Replace('\r', ','))
            .Where(a => a.Contains(": "))
            .ToDictionary(a => a.Split(": ")[0], a => a.Split(": ")[1]);
        if (paramsDict.TryGetValue("title", out var titleFromParams))
        {
            title = titleFromParams;
        }

        return new CategoryInfo(title, rawContent, categoryUrl);
    }

    public static CategoryInfo CreateFromUrl(string url) => new(url, "", url);
}