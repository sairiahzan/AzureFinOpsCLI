using System.Collections.Generic;

namespace AzureFinOpsCLI.Models;

public class AnalysisReport
{
    public List<ZombieResource> ZombieResources { get; set; } = new();
    public decimal TotalEstimatedMonthlyWaste { get; set; }
    public decimal TotalEstimatedHourlyWaste { get; set; }
    public int ScannedResourceCount { get; set; }
    public string Currency { get; set; } = "USD";
}
