using CommandLine;
using SimpleStaticSiteGenerator;

Utils.ShowMessage("                  ,,__");
Utils.ShowMessage("        ..  ..   / o._)                   .---.");
Utils.ShowMessage(@"       /--'/--\  \-'||        .----.    .'     '.");
Utils.ShowMessage(@"      /        \_/ / |      .'      '..'         '-.");
Utils.ShowMessage(@"    .'\  \__\  __.'.'     .' ");
Utils.ShowMessage(@"      )\ |  )\ |      _.'SIMPLE STATIC SITE GENERATOR");
Utils.ShowMessage(@"     // \\ // \\");
Utils.ShowMessage(@"    ||_  \\|_  \\_");
Utils.ShowMessage("    '--' '--'' '--'");

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
