using Spectre.Console;
using AzureFinOpsCLI.Models;
using System.Linq;

namespace AzureFinOpsCLI.Presentation;

public static class ConsoleRenderer
{
    public static void PrintHeader()
    {
        AnsiConsole.Write(
            new FigletText("Azure FinOps CLI")
                .LeftJustified()
                .Color(Color.Blue));
                
        AnsiConsole.MarkupLine("[bold grey]Local-First Azure Cost and Resource Leak Detector[/]");
        AnsiConsole.WriteLine();
    }

    public static void PrintReport(AnalysisReport report)
    {
        AnsiConsole.MarkupLine($"[green]Scanned [bold]{report.ScannedResourceCount}[/] resources.[/]");
        AnsiConsole.WriteLine();

        if (report.ZombieResources.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold green]No zombie resources found. You are fully optimized![/]");
            return;
        }

        var table = new Table();
        table.AddColumn("[bold blue]Resource Name[/]");
        table.AddColumn("[bold blue]Type[/]");
        table.AddColumn("[bold blue]CPU %[/]");
        table.AddColumn("[bold blue]Cost/Hr[/]");
        table.AddColumn("[bold blue]Cost/Mo[/]");
        table.AddColumn("[bold blue]Recommendation[/]");

        foreach (var res in report.ZombieResources.OrderByDescending(r => r.EstimatedMonthlyCost))
        {
            string cpuColor = res.AverageCpuPercentage < 0.5 ? "red" : "yellow";
            
            table.AddRow(
                res.Name,
                res.ResourceType.Split('/').Last(),
                $"[{cpuColor}]{res.AverageCpuPercentage}%[/]",
                $"{res.EstimatedHourlyCost:F4} {report.Currency}",
                $"{res.EstimatedMonthlyCost:F2} {report.Currency}",
                $"[bold]{res.Recommendation}[/]"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        var panel = new Panel(
            new Markup(
                $"[bold red]Total Estimated Monthly Waste:[/] {report.TotalEstimatedMonthlyWaste:F2} {report.Currency}\n" +
                $"[bold red]Total Estimated Hourly Waste:[/] {report.TotalEstimatedHourlyWaste:F4} {report.Currency}"
            )
        );
        panel.Header = new PanelHeader("Summary");
        panel.Border = BoxBorder.Rounded;
        panel.Padding = new Padding(2, 2, 2, 2);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }
}
