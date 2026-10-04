namespace AzureFinOpsCLI.Models;

public class ZombieResource
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ResourceGroupName { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string SubscriptionId { get; set; } = string.Empty;
    public double AverageCpuPercentage { get; set; }
    public decimal EstimatedMonthlyCost { get; set; }
    public decimal EstimatedHourlyCost { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}
