using System.CommandLine;
using Spectre.Console;
using AzureFinOpsCLI.Services;
using AzureFinOpsCLI.Models;
using AzureFinOpsCLI.Presentation;
using System.Threading.Tasks;

namespace AzureFinOpsCLI.Commands;

public class AnalyzeCommand : Command
{
    private readonly ResourceScannerService _scannerService;

    public AnalyzeCommand(ResourceScannerService scannerService) 
        : base("analyze", "Scans Azure subscriptions for zombie resources")
    {
        _scannerService = scannerService;
        
        var thresholdOption = new Option<double>(
            "--cpu-threshold",
            () => 1.0,
            "CPU percentage threshold for identifying zombie resources");
            
        var lookbackOption = new Option<int>(
            "--lookback-days",
            () => 7,
            "Days to look back for metrics data");

        var currencyOption = new Option<string>(
            "--currency",
            () => "USD",
            "Currency code for pricing (e.g., USD, TRY, EUR)");

        AddOption(thresholdOption);
        AddOption(lookbackOption);
        AddOption(currencyOption);

        this.SetHandler(async (threshold, lookback, currency) =>
        {
            await ExecuteAsync(threshold, lookback, currency);
        }, thresholdOption, lookbackOption, currencyOption);
    }

    private async Task ExecuteAsync(double threshold, int lookback, string currency)
    {
        ConsoleRenderer.PrintHeader();

        var settings = new AppSettings 
        { 
            ZombieCpuThresholdPercentage = threshold, 
            MetricsLookbackDays = lookback,
            Currency = currency
        };

        AnalysisReport report = new AnalysisReport();
        report.Currency = currency;

        await AnsiConsole.Status()
            .StartAsync("Scanning Azure subscriptions and analyzing metrics...", async ctx =>
            {
                report = await _scannerService.ScanForZombiesAsync(settings);
            });

        ConsoleRenderer.PrintReport(report);
        
        AnsiConsole.MarkupLine("To take action on a specific resource, use the [bold cyan]action[/] command.");
    }
}
