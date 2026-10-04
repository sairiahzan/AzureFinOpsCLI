namespace AzureFinOpsCLI.Models;

public class AppSettings
{
    public double ZombieCpuThresholdPercentage { get; set; } = 1.0;
    public int MetricsLookbackDays { get; set; } = 7;
    public string Currency { get; set; } = "USD";
}
