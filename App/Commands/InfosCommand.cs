using System.ComponentModel.DataAnnotations;
using App.Helpers;
using Lib.Services;
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
            var credentials = new SoapServiceCredentials(Url, Username, Password);
            var service = SoapService.BuildSoapService(credentials);
            ConsoleHelper.RenderInfos(service.GetMethodInfos());
        }

        protected override bool HasValidOptions()
        {
            if (!SoapServiceCredentials.IsValidUrl(Url))
            {
                return false;
            }

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

        protected static string GetVersion() => GetVersion(typeof(InfosCommand));
    }
}