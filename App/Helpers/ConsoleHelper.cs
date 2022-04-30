using System;
using System.Collections.Generic;
using System.Reflection;
using Spectre.Console;

namespace App.Helpers
{
    public class ConsoleHelper : IConsoleHelper
    {
        public void RenderTitle(string text)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new FigletText(text).LeftAligned());
            AnsiConsole.WriteLine();
        }

        public void RenderInfos(ICollection<MethodInfo> methods)
        {
            var table = new Table()
                .BorderColor(Color.White)
                .Border(TableBorder.Square)
                .Title($"[yellow]{methods.Count} method(s) found[/]")
                .AddColumn(new TableColumn("[u]Name[/]").Centered());

            foreach (var method in methods)
            {
                table.AddRow(method.Name);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }

        public void RenderException(Exception exception)
        {
            const ExceptionFormats formats = ExceptionFormats.ShortenTypes
                                             | ExceptionFormats.ShortenPaths
                                             | ExceptionFormats.ShortenMethods;

            AnsiConsole.WriteLine();
            AnsiConsole.WriteException(exception, formats);
            AnsiConsole.WriteLine();
        }
    }
}