using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using AzureFinOpsCLI.Core;
using AzureFinOpsCLI.Models;

namespace AzureFinOpsCLI.Services;

public class ResourceScannerService
{
    private readonly AzureAuthService _authService;
    private readonly MetricsService _metricsService;
    private readonly CostEstimatorService _costEstimator;

    public ResourceScannerService(
        AzureAuthService authService,
        MetricsService metricsService,
        CostEstimatorService costEstimator)
    {
        _authService = authService;
        _metricsService = metricsService;
        _costEstimator = costEstimator;
    }

    public async Task<AnalysisReport> ScanForZombiesAsync(AppSettings settings)
    {
        var report = new AnalysisReport();
        
        try
        {
            var armClient = await _authService.GetArmClientAsync();
            var subscriptions = armClient.GetSubscriptions();
            
            foreach (var sub in subscriptions)
            {
                var resourceGroups = sub.GetResourceGroups();
                foreach (var rg in resourceGroups)
                {
                    // For the sake of this local tool, we use GetGenericResources to list everything in the RG
                    var resources = rg.GetGenericResources();
                    
                    foreach (var resource in resources)
                    {
                        report.ScannedResourceCount++;
                        
                        var resourceType = resource.Data.ResourceType.ToString().ToLowerInvariant();
                        
                        // Target only specific expensive resources
                        if (resourceType == Constants.VmResourceType.ToLowerInvariant() ||
                            resourceType == Constants.SqlDbResourceType.ToLowerInvariant() ||
                            resourceType == Constants.PublicIpResourceType.ToLowerInvariant())
                        {
                            var cpu = await _metricsService.GetAverageCpuPercentageAsync(resource.Id, settings.MetricsLookbackDays);
                            
                            if (cpu < settings.ZombieCpuThresholdPercentage)
                            {
                                var costs = await _costEstimator.EstimateCostAsync(
                                    resourceType, 
                                    resource.Data.Location.Name, 
                                    resource.Data.Sku?.Name,
                                    settings.Currency);
                                
                                string recommendation = Constants.RecommendationReview;
                                if (resourceType == Constants.VmResourceType.ToLowerInvariant())
                                    recommendation = Constants.RecommendationSleep;
                                else if (resourceType == Constants.PublicIpResourceType.ToLowerInvariant())
                                    recommendation = Constants.RecommendationDelete;

                                report.ZombieResources.Add(new ZombieResource
                                {
                                    Id = resource.Id,
                                    Name = resource.Data.Name,
                                    ResourceGroupName = rg.Data.Name,
                                    SubscriptionId = sub.Data.SubscriptionId,
                                    ResourceType = resource.Data.ResourceType,
                                    AverageCpuPercentage = cpu,
                                    EstimatedMonthlyCost = costs.Monthly,
                                    EstimatedHourlyCost = costs.Hourly,
                                    Recommendation = recommendation
                                });
                                
                                report.TotalEstimatedMonthlyWaste += costs.Monthly;
                                report.TotalEstimatedHourlyWaste += costs.Hourly;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // For a CLI tool, we might want to log this to console later, or just swallow if one subscription fails.
            Console.WriteLine($"Error scanning resources: {ex.Message}");
        }

        return report;
    }
}
