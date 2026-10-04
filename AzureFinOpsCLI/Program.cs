using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AzureFinOpsCLI.Commands;
using AzureFinOpsCLI.Services;
using System.Threading.Tasks;

namespace AzureFinOpsCLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // 1. Setup Dependency Injection
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<AzureAuthService>();
                services.AddSingleton<MetricsService>();
                services.AddSingleton<CostEstimatorService>();
                services.AddSingleton<ResourceActionService>();
                services.AddSingleton<ResourceScannerService>();
                
                services.AddSingleton<AnalyzeCommand>();
                services.AddSingleton<ActionCommand>();
            })
            .Build();

        // 2. Setup System.CommandLine Root Command
        var rootCommand = new RootCommand("Local-First Azure Cost and Resource Leak Detector (FinOps CLI)");
        
        // Resolve commands from DI
        var analyzeCmd = host.Services.GetRequiredService<AnalyzeCommand>();
        var actionCmd = host.Services.GetRequiredService<ActionCommand>();

        rootCommand.AddCommand(analyzeCmd);
        rootCommand.AddCommand(actionCmd);

        // 3. Execute
        return await rootCommand.InvokeAsync(args);
    }
}
