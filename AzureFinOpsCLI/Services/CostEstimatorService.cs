using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureFinOpsCLI.Services;

public class CostEstimatorService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public async Task<(decimal Monthly, decimal Hourly)> EstimateCostAsync(string resourceType, string region, string skuName, string currency = "USD")
    {
        decimal hourlyCost = 0;

        try
        {
            if (string.IsNullOrEmpty(region)) region = "westeurope";
            
            // Basic mapping to Azure Retail Prices API filters
            string serviceName = "";
            string skuFilter = "";

            if (resourceType.Contains("virtualmachines", StringComparison.OrdinalIgnoreCase))
            {
                serviceName = "Virtual Machines";
                skuName = string.IsNullOrEmpty(skuName) ? "Standard_B2s" : skuName; // fallback if sku is null
                skuFilter = $" and armSkuName eq '{skuName}'";
            }
            else if (resourceType.Contains("databases", StringComparison.OrdinalIgnoreCase))
            {
                serviceName = "SQL Database";
                skuName = string.IsNullOrEmpty(skuName) ? "Basic" : skuName;
                skuFilter = $" and skuName eq '{skuName}'";
            }
            else if (resourceType.Contains("publicipaddresses", StringComparison.OrdinalIgnoreCase))
            {
                serviceName = "Virtual Network";
                // E.g. Standard IPv4 Static Public IP
                skuFilter = " and skuName eq 'Standard IPv4 Static Public IP'";
            }
            
            if (!string.IsNullOrEmpty(serviceName))
            {
                // Construct OData filter query
                string query = $"currencyCode='{currency}'&$filter=armRegionName eq '{region}' and serviceName eq '{serviceName}' and priceType eq 'Consumption'{skuFilter}";
                string url = $"https://prices.azure.com/api/retail/prices?{query}";

                var response = await _httpClient.GetStringAsync(url);
                using var doc = JsonDocument.Parse(response);
                
                var items = doc.RootElement.GetProperty("Items");
                if (items.GetArrayLength() > 0)
                {
                    hourlyCost = items[0].GetProperty("retailPrice").GetDecimal();
                }
            }
        }
        catch
        {
            // Fallback if API fails (e.g. offline, rate limits, no match)
        }

        // If we failed to get a price from the API, use our static fallbacks
        if (hourlyCost == 0)
        {
            switch (resourceType.ToLowerInvariant())
            {
                case "microsoft.compute/virtualmachines":
                    hourlyCost = 75.0m / 730m;
                    break;
                case "microsoft.sql/servers/databases":
                    hourlyCost = 15.0m / 730m;
                    break;
                case "microsoft.network/publicipaddresses":
                    hourlyCost = 3.6m / 730m;
                    break;
                default:
                    hourlyCost = 10.0m / 730m;
                    break;
            }
        }

        decimal monthlyCost = hourlyCost * 730m;
        
        return (monthlyCost, hourlyCost);
    }
}
