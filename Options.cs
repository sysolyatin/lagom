using CommandLine;

namespace LagomCms;

[Verb("build", HelpText = "Build Static Site")]
public class BuildOptions {
    [Option('h',"rssHostName", Required = false, HelpText = "Host name for use in RSS feed, " +
                                                            "(ex. https://site.com/)")]
    public string RssHostName { get; set; }
    
    [Option('t',"title", Required = false, HelpText = "Site title for RSS feed")]
    public string RssSiteTitle { get; set; }
    
    [Option('d',"subtitle", Required = false, HelpText = "Site subtitle for RSS feed")]
    public string RssSiteSubTitle { get; set; }
}

[Verb("deploy", HelpText = "Deploy site to FTP")]
public class DeployOptions {
    [Option('s',"server", Required = true, HelpText = "FTP server")]
    public string FtpServer { get; set; }
    
    [Option('p',"port", Default = 21, Required = true, HelpText = "FTP port")]
    public int FtpPort { get; set; }
    
    [Option('f',"folder", Required = true, HelpText = "FTP folder")]
    public string FtpFolder { get; set; }
    
    [Option('u',"user", Required = true, HelpText = "FTP user")]
    public string FtpUser { get; set; }
    
    [Option('w',"password", Required = true, HelpText = "FTP Password")]
    public string FtpPassword { get; set; }
}