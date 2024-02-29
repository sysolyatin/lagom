namespace LagomCms;

public class Page
{
    public string Title { get; }
    public string Category { get; }
    public string PageUrl { get; }
    public string Date { get; }
    public string RawContent { get; }
    public bool IsPost { get; }

    public string HtmlFileName => PageUrl == "index"
        ? "index.html"
        : IsPost
            ? Path.Combine(Category, PageUrl, "index.html")
            : Path.Combine(PageUrl, "index.html");
    public string RelativeLinkPrefix => PageUrl == "index" ? "" : IsPost ? "../../" : "../";

    public Page(string filePath)
    {
        var fileName = Path.GetFileName(filePath).Split(".")[0];
        var fileContent = File.ReadAllText(filePath);
        IsPost = false;
        Date = string.Empty;
        PageUrl = fileName;
        
        var dateInFileName = fileName.Length > 12 ? fileName[..10] : "-";
        
        if (DateTime.TryParse(dateInFileName, out var date))
        {
            IsPost = true;
            Date = date.ToString("yyyy-MM-dd");
            PageUrl = PageUrl[11..];
            fileContent = fileContent.Replace("(../static/", $"({RelativeLinkPrefix}static/");
        }
        
        Title = PageUrl;
        Category = IsPost ? "posts" : string.Empty;
        RawContent = fileContent;
        
        var fileContentParts = fileContent
            .Replace("\r", "")
            .Split("\n----------\n\n");
        
        if (fileContentParts.Length != 2) return;
        RawContent = fileContentParts[1];
        var paramsDict = fileContentParts[0]
            .Split('\n')
            .Select(a => a.Replace('\r', ','))
            .Where(a => a.Contains(": "))
            .ToDictionary(a => a.Split(": ")[0], a => a.Split(": ")[1]);
        if (paramsDict.TryGetValue("title", out var title))
        {
            Title = title;
        }
        if (paramsDict.TryGetValue("category", out var category))
        {
            Category = IsPost ? category : string.Empty;
        }
    }
}