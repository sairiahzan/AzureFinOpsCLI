using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Network;

namespace AzureFinOpsCLI.Services;

public class ResourceActionService
{
    private readonly AzureAuthService _authService;

    public ResourceActionService(AzureAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> SleepResourceAsync(string resourceId)
    {
        try
        {
            var client = await _authService.GetArmClientAsync();
            var id = new ResourceIdentifier(resourceId);
            
            if (id.ResourceType.ToString().Equals("Microsoft.Compute/virtualMachines", StringComparison.OrdinalIgnoreCase))
            {
                // To sleep a VM, we deallocate it
                var vmResource = client.GetVirtualMachineResource(id);
                // Note: Get virtual machine might require you to actually fetch it first, but ArmClient allows getting a resource client by ID
                await vmResource.DeallocateAsync(Azure.WaitUntil.Completed);
                return true;
            }
            
            // Other resources might not have a "sleep" concept, only scale down or delete.
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteResourceAsync(string resourceId)
    {
        try
        {
            var client = await _authService.GetArmClientAsync();
            var id = new ResourceIdentifier(resourceId);

            var genericResource = client.GetGenericResource(id);
            await genericResource.DeleteAsync(Azure.WaitUntil.Completed);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}
