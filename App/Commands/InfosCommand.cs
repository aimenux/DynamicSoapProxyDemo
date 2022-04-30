using System;
using System.ComponentModel.DataAnnotations;
using App.Helpers;
using Lib.Helpers;
using Lib.Models;
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
        [Option("--url", "Url", CommandOptionType.SingleValue)]
        public string Url { get; set; }

        [Option("-u|--user", "User", CommandOptionType.SingleValue)]
        public string Username { get; set; }

        [Option("-p|--pass", "Password", CommandOptionType.SingleValue)]
        public string Password { get; set; }

        protected override void Execute(CommandLineApplication app)
        {
            if (!Url.EndsWith("?wsdl", StringComparison.OrdinalIgnoreCase))
            {
                Url = $"{Url}?wsdl";
            }

            ServiceCredentials credentials = null;

            if (AreValidCredentials())
            {
                credentials = new ServiceCredentials
                {
                    Username = Username,
                    Password = Password
                };
            }

            var inspector = new ServiceInspector(Url, credentials);
            ConsoleHelper.RenderInfos(inspector.MethodInfos);
        }

        protected override bool HasValidOptions()
        {
            return !string.IsNullOrWhiteSpace(Url)
                   && Uri.TryCreate(Url, UriKind.Absolute, out var _)
                   && AreValidCredentials();
        }

        protected static string GetVersion() => GetVersion(typeof(InfosCommand));

        private bool AreValidCredentials()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                return string.IsNullOrWhiteSpace(Password);
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                return string.IsNullOrWhiteSpace(Username);
            }

            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}