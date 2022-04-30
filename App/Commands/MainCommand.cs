using App.Extensions;
using App.Helpers;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands
{
    [Command(Name = "SoapCli", FullName = "Generate a dynamic proxy for soap web services", Description = "Generate a dynamic proxy for soap web services.")]
    [Subcommand(typeof(InfosCommand))]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    public class MainCommand : AbstractCommand
    {
        public MainCommand(IConsoleHelper consoleHelper) : base(consoleHelper)
        {
        }

        protected override void Execute(CommandLineApplication app)
        {
            const string title = "SoapCli";
            ConsoleHelper.RenderTitle(title);
            app.ShowHelp();
        }

        protected static string GetVersion() => typeof(MainCommand).GetVersion();
    }
}