using System.CommandLine;
using Spectre.Console;
using AzureFinOpsCLI.Services;
using System.Threading.Tasks;

namespace AzureFinOpsCLI.Commands;

public class ActionCommand : Command
{
    private readonly ResourceActionService _actionService;

    public ActionCommand(ResourceActionService actionService) 
        : base("action", "Take action (sleep/delete) on a specific resource")
    {
        _actionService = actionService;

        var idOption = new Option<string>(
            "--id",
            "The Azure Resource ID to take action on")
            { IsRequired = true };

        var typeOption = new Option<string>(
            "--type",
            "The action to perform: 'sleep' or 'delete'")
            { IsRequired = true };

        AddOption(idOption);
        AddOption(typeOption);

        this.SetHandler(async (id, type) =>
        {
            await ExecuteAsync(id, type);
        }, idOption, typeOption);
    }

    private async Task ExecuteAsync(string resourceId, string actionType)
    {
        AnsiConsole.MarkupLine($"[bold yellow]Preparing to {actionType} resource...[/]");
        AnsiConsole.MarkupLine($"Resource ID: [grey]{resourceId}[/]");

        if (!AnsiConsole.Confirm($"Are you sure you want to [red]{actionType}[/] this resource?"))
        {
            AnsiConsole.MarkupLine("[yellow]Action cancelled.[/]");
            return;
        }

        bool success = false;
        
        await AnsiConsole.Status()
            .StartAsync($"Executing {actionType}...", async ctx =>
            {
                if (actionType.ToLowerInvariant() == "sleep")
                {
                    success = await _actionService.SleepResourceAsync(resourceId);
                }
                else if (actionType.ToLowerInvariant() == "delete")
                {
                    success = await _actionService.DeleteResourceAsync(resourceId);
                }
            });

        if (success)
        {
            AnsiConsole.MarkupLine($"[bold green]Successfully performed '{actionType}' on the resource![/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold red]Failed to perform '{actionType}'. The resource might not support this action or you might lack permissions.[/]");
        }
    }
}
