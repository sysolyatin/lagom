using CommandLine;
using LagomCms;

Utils.ShowMessage("LagomCMS");
Utils.ShowMessage("https://github.com/sysolyatin/lagom");

try
{
    var parser = new Parser(c => c.HelpWriter = null);
    var parserResult = parser.ParseArguments<BuildOptions, DeployOptions>(args);
    parserResult
        .WithParsed<BuildOptions>(Operations.BuildSite)
        .WithParsed<DeployOptions>(Operations.DeploySite)
        .WithNotParsed(errs => Operations.DisplayHelp(parserResult, errs));
}
catch (Exception e)
{
    Utils.ShowMessage($"ERROR: {e.Message}", ConsoleColor.Red);
}
