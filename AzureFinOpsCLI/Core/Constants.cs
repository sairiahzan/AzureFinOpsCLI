namespace AzureFinOpsCLI.Core;

public static class Constants
{
    public const string VmResourceType = "Microsoft.Compute/virtualMachines";
    public const string SqlDbResourceType = "Microsoft.Sql/servers/databases";
    public const string PublicIpResourceType = "Microsoft.Network/publicIPAddresses";
    
    public const string RecommendationSleep = "Stop (Deallocate) Resource";
    public const string RecommendationDelete = "Delete Resource";
    public const string RecommendationReview = "Review Manually";
}
