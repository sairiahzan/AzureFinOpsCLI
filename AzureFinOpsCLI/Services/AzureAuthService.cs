using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.ResourceManager;

namespace AzureFinOpsCLI.Services;

public class AzureAuthService
{
    private ArmClient? _armClient;

    public async Task<ArmClient> GetArmClientAsync()
    {
        if (_armClient != null)
        {
            return _armClient;
        }

        try
        {
            // DefaultAzureCredential tries various auth methods: Env Vars, Managed Identity, Visual Studio, Azure CLI, Azure PowerShell, etc.
            // "Local-first" means it will typically pick up the Azure CLI credential (`az login`) if developer is logged in.
            var credential = new DefaultAzureCredential();
            _armClient = new ArmClient(credential);
            
            // Just a test to see if auth actually works (fetches default subscription)
            await _armClient.GetDefaultSubscriptionAsync();
            
            return _armClient;
        }
        catch (Exception ex)
        {
            throw new Exception($"Azure Authentication failed. Please ensure you are logged in via 'az login'. Error: {ex.Message}");
        }
    }
}
