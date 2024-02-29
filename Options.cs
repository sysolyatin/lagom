using CommandLine;

namespace LagomCms;

[Verb("build", HelpText = "Build Static Site")]
public class BuildOptions {
    [Option('u',"baseUrl", Required = false, HelpText = "Base url for site, " +
                                                            "(ex. https://site.com/)")]
    public string BaseUrl { get; set; }
    
    [Option('t',"title", Required = false, HelpText = "Site title")]
    public string SiteTitle { get; set; }
    
    [Option('d',"subtitle", Required = false, HelpText = "Site subtitle")]
    public string SiteSubTitle { get; set; }
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