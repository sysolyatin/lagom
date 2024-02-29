using Markdig;

namespace LagomCms;

public class HtmlPageCreator
{
    private readonly string _tpl;
    private readonly string _siteDir;
    private readonly List<CategoryInfo> _categoriesInfo;
    private readonly Dictionary<string, List<Page>> _categoryDictionary;
    private readonly MarkdownPipeline _mdPipeline;
    private readonly string _siteBaseUrl;
    private readonly string _siteTitle;
    private readonly string _siteSubTitle;
    private readonly DateTime _buildDateTime;

    public HtmlPageCreator(string tplPath, string siteDir, List<CategoryInfo> categoriesInfo, BuildOptions buildOptions)
    {
        _buildDateTime = DateTime.Now;
        _tpl = File.ReadAllText(tplPath);
        _siteDir = siteDir;
        _categoriesInfo = categoriesInfo;
        _siteBaseUrl = buildOptions.RssHostName;
        if (!string.IsNullOrEmpty(_siteBaseUrl) && _siteBaseUrl.Length > 1 && _siteBaseUrl.Last() != '/')
        {
            _siteBaseUrl += "/";
        }
        _siteTitle = buildOptions.RssSiteTitle;
        _siteSubTitle = buildOptions.RssSiteSubTitle;
        
        _categoryDictionary = new Dictionary<string, List<Page>>();
        _mdPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }

    public void CreatePage(Page page)
    {
        if (_categoriesInfo.Any(a => a.CategoryUrl == page.PageUrl))
        {
            Utils.ShowMessage($"{page.PageUrl} is a category, skip {page.HtmlFileName}");
        }
        Utils.ShowMessage($"Create {page.HtmlFileName}", ConsoleColor.Magenta);
        if (page.IsPost)
        {
            if (_categoryDictionary.TryGetValue(page.Category, out var pagesInThisCategory))
            {
                pagesInThisCategory.Add(page);
            }
            if (!_categoryDictionary.ContainsKey(page.Category))
            {
                _categoryDictionary.Add(page.Category, new List<Page>{page});
            }
        }

        var html = FillTemplate(
            page.Title, 
            Markdown.ToHtml(page.RawContent, _mdPipeline), 
            page.RelativeLinkPrefix,
            _buildDateTime,
            page.Date);

        var htmlFilePath = Path.Combine(_siteDir, page.HtmlFileName);
        var htmlFileDirectory = Path.GetDirectoryName(htmlFilePath);

        if (htmlFileDirectory != null)
        {
            Directory.CreateDirectory(htmlFileDirectory);
        }
        
        File.WriteAllText(htmlFilePath, html);
    }

    public void CreateCategoryPages()
    {
        foreach (var categoryDictItem in _categoryDictionary)
        {
            var categoryUrl = categoryDictItem.Key;
            Utils.ShowMessage($"Create {categoryUrl} category page", ConsoleColor.Magenta);
            var categoryInfo = _categoriesInfo.FirstOrDefault(a => a.CategoryUrl == categoryUrl) 
                               ?? CategoryInfo.CreateFromUrl(categoryUrl);
            var htmlContent = $"{Markdown.ToHtml(categoryInfo.RawContent, _mdPipeline)}<ul>\n" 
                           + string.Join("\n", categoryDictItem.Value
                .OrderByDescending(a => a.Date)
                .Select(p => $"\t<li><time datetime=\"{p.Date}T00:00:00\" title=\"{p.Date}\">{p.Date}</time>" +
                             $"<a href=\"{p.PageUrl}\">{p.Title}</a></li>")) + "</ul>\n";
            
            var html = FillTemplate(categoryInfo.Title, htmlContent, "../", _buildDateTime);
            var htmlFilePath = Path.Combine(_siteDir, categoryInfo.HtmlFileName);
            File.WriteAllText(htmlFilePath, html);
        }
    }

    public void CreateRssFeed(string rssTitle, string rssSubTitle)
    {
        Utils.ShowMessage("Create feed.xml", ConsoleColor.Magenta);
        if (string.IsNullOrEmpty(rssTitle))
        {
            rssTitle = "RSS Feed";
        }
        if (string.IsNullOrEmpty(rssSubTitle))
        {
            rssSubTitle = string.Empty;
        }
        if (string.IsNullOrEmpty(_siteBaseUrl))
        {
            Utils.ShowMessage("Base hostname is empty, skip creating RSS feed");
            return;
        }

        var rss = $"<feed xmlns=\"http://www.w3.org/2005/Atom\">" +
                  $"<script/>" +
                  $"<generator uri=\"https://github.com/sysolyatin/sssg\" version=\"1.0.0\">" +
                  $"Simple Static Site Generator</generator>" +
                  $"<link href=\"{_siteBaseUrl}feed.xml\" rel=\"self\" type=\"application/atom+xml\"/>" +
                  $"<link href=\"{_siteBaseUrl}\" rel=\"alternate\" type=\"text/html\"/>" +
                  $"<updated>{DateTime.Now:s}+00:00</updated>" +
                  $"<id>{_siteBaseUrl}feed.xml</id>" +
                  $"<title type=\"html\">{rssTitle}</title><subtitle>{rssSubTitle}</subtitle>";

        rss += string.Join("", _categoryDictionary
            .SelectMany(a => a.Value)
            .Select(p => $"<entry><title type=\"html\">{p.Title}</title>" +
                         $"<link href=\"{_siteBaseUrl}{p.Category}/{p.PageUrl}/\" " +
                         $"rel=\"alternate\" type=\"text/html\" title=\"{p.Title}\"/>" +
                         $"<published>{p.Date}T00:00:00+00:00</published>" +
                         $"<updated>{p.Date}T00:00:00+00:00</updated>" +
                         $"<id>{_siteBaseUrl}{p.Category}/{p.PageUrl}/</id>" +
                         $"<content type=\"html\" xml:base=\"{_siteBaseUrl}{p.Category}/{p.PageUrl}/\">" +
                         $"{Markdown.ToHtml(p.RawContent, _mdPipeline)}</content>" +
                         $"<author><name/></author>" +
                         $"<summary type=\"html\">{Markdown.ToHtml(p.RawContent, _mdPipeline)}</summary></entry>"));
        rss += "</feed>";
        File.WriteAllText(Path.Combine(_siteDir, "feed.xml"), rss);
    }
    
    private string FillTemplate(string title, string content, string relativeLinkPrefix, 
        DateTime buildDate, string date="")
    {
        return _tpl
            .Replace("{{site_title}}", _siteTitle)
            .Replace("{{site_subtitle}}", _siteSubTitle)
            .Replace("{{base_url}}", _siteBaseUrl)
            .Replace("{{page_title}}", title)
            .Replace("{{page_date}}", date)
            .Replace("{{page_content}}", content)
            .Replace("{{relative_link_prefix}}", relativeLinkPrefix)
            .Replace("{{last_build}}", buildDate.ToString("dd.MM.yyyy HH:mm"))
            .Replace("{{timestamp}}", ((DateTimeOffset)buildDate).ToUnixTimeSeconds().ToString())
            .Replace("{{year}}", DateTime.Now.Year.ToString());
    }
}