using System;
using System.ComponentModel.DataAnnotations;
using App.Extensions;
using App.Helpers;
using Lib.Helpers;
using McMaster.Extensions.CommandLineUtils;

namespace App.Commands
{
    [Command(Name = "Infos", FullName = "Get soap service infos", Description = "Get soap service infos.")]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    [HelpOption]
    public class InfosCommand : AbstractCommand
    {
        public InfosCommand(IConsoleHelper consoleHelper) : base(consoleHelper)
        {
        }

        [Required]
        [Option("-u|--url", "Url", CommandOptionType.SingleValue)]
        public string Url { get; set; }

        protected override void Execute(CommandLineApplication app)
        {
            if (!Url.EndsWith("?wsdl", StringComparison.OrdinalIgnoreCase))
            {
                Url = $"{Url}?wsdl";
            }

            var inspector = new ServiceInspector(Url);
            ConsoleHelper.RenderInfos(inspector.MethodInfos);
        }

        protected override bool HasValidOptions()
        {
            return !string.IsNullOrWhiteSpace(Url) && Uri.TryCreate(Url, UriKind.Absolute, out var _);
        }

        protected static string GetVersion() => typeof(InfosCommand).GetVersion();
    }
}