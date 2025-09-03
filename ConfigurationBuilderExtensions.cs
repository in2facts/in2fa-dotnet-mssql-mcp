using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace mssqlMCP;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder TryAddJsonFromAzureKeyVault(this IConfigurationBuilder configurationBuilder,
        bool isDevelopment)
    {
        var configurations = configurationBuilder.Build();

        var keyVaultEndpoint = configurations["Azure:KeyVault:VaultUri"];
        var appsettingsSecretName = configurations["Azure:KeyVault:AppsettingsSecretName"];

        if (!string.IsNullOrWhiteSpace(keyVaultEndpoint) &&
            !string.IsNullOrWhiteSpace(appsettingsSecretName) &&
            !isDevelopment)
        {
            var secretClient = new SecretClient(new Uri(keyVaultEndpoint), new DefaultAzureCredential());

            var secretValue = secretClient.GetSecret(appsettingsSecretName);

            configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(secretValue.Value.Value)));
        }

        return configurationBuilder;
    }
}