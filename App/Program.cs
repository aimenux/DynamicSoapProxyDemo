using App.Commands;
using App.Helpers;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace App
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddTransient<IConsoleHelper, ConsoleHelper>()
                .AddTransient<InfosCommand>()
                .BuildServiceProvider();

            var app = new CommandLineApplication<MainCommand>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);

            return app.Execute(args);
        }
    }
}
