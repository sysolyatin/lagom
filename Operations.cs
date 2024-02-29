using CommandLine;
using CommandLine.Text;
using FluentFTP;

namespace LagomCms;

public static class Operations
{
    public static void BuildSite(BuildOptions options)
    {
        Console.WriteLine(options.BaseUrl);
        const string tplFileName = "template.html";
        const string siteDir = "_site";
        const string pagesDir = "pages";
        const string staticDir = "static";
        const string categoriesDir = "categories";
        
        if (!File.Exists(tplFileName))
        {
            Utils.ShowMessage($"Template file {tplFileName} not found", ConsoleColor.Red);
        }

        if (!Directory.Exists(pagesDir))
        {
            Utils.ShowMessage($"Directory {pagesDir} not found", ConsoleColor.Red);
        }
    
        if (!Directory.Exists(categoriesDir))
        {
            Utils.ShowMessage($"Directory {categoriesDir} not found", ConsoleColor.Red);
        }
    
        if (Directory.Exists(siteDir)) Directory.Delete(siteDir, true);
        Directory.CreateDirectory(siteDir);
        Directory.CreateDirectory(Path.Combine(siteDir, staticDir));

        if (Directory.Exists(staticDir))
        {
            Utils.CopyFilesRecursively(staticDir, Path.Combine(siteDir, staticDir));
        }

        var categoriesInfo = Directory.GetFiles(categoriesDir)
            .Where(Utils.IsSourcePage)
            .Select(CategoryInfo.CreateFromFile)
            .ToList();
        
        var htmlPageCreator = new HtmlPageCreator(tplFileName, siteDir, categoriesInfo, options);

        Directory.GetFiles(pagesDir)
            .Where(Utils.IsSourcePage)
            .Select(a => new Page(a))
            .ToList()
            .ForEach(htmlPageCreator.CreatePage);
    
        htmlPageCreator.CreateCategoryPages();
        htmlPageCreator.CreateRssFeed(options.SiteTitle, options.SiteSubTitle);
    
        Utils.ShowMessage("Site successfully created", ConsoleColor.Green);
    }

    public static void DeploySite(DeployOptions options)
    {
        const string buildDir = "_site";
        if (!Directory.Exists(buildDir))
        {
            Utils.ShowMessage("Build directory not found", ConsoleColor.Red);
            return;
        }
        
        using var ftp = new FtpClient(options.FtpServer, options.FtpUser, options.FtpPassword, options.FtpPort);
        ftp.Connect();
        ftp.EmptyDirectory($"/{options.FtpFolder}/");
        ftp.UploadDirectory(buildDir, $"/{options.FtpFolder}/");
        
        Utils.ShowMessage("Site successfully uploaded to server", ConsoleColor.Green);
    }
    
    public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
    {
        HelpText? helpText;
        if (errs.IsVersion())
        {
            helpText = HelpText.AutoBuild(result);
        }
        else
        {
            helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "";
                h.Copyright = "";
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(helpText);
    }
}