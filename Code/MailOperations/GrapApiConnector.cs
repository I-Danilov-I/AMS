using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Azure.Identity;

namespace AMST
{
    internal static class GraphApiConnector
    {
        private static IConfigurationRoot? _configGraphApi;
        private static string? _tenantId => _configGraphApi?["GraphMail:TenantId"];
        private static string? _clientId => _configGraphApi?["GraphMail:ClientId"];
        private static string? _clientSecret => _configGraphApi?["GraphMail:ClientSecret"];
        private static string? _userEmail => _configGraphApi?["GraphMail:GraphUserEmail"];

        private static void LoadGraphApiConfiguration()
        {
            try
            {
                Logger.Log.Information("Loading configuration for Graph API...");
                string currentProjectPath = Directory.GetCurrentDirectory().Replace("bin\\Debug\\net8.0", "");
                string secretsFolderPath = "Code\\Secrets";
                string secretsFileName = "secret.json";
                string fullPath = Path.Combine(secretsFolderPath, secretsFileName);

                _configGraphApi = new ConfigurationBuilder()
                    .SetBasePath(currentProjectPath)
                    .AddJsonFile(fullPath)
                    .Build();

                if (_configGraphApi == null)
                {
                    throw new ArgumentNullException(nameof(_configGraphApi), "Configuration could not be loaded.");
                }

                Logger.Log.Information("Configuration for Graph API loaded successfully.");
            }
            catch (ArgumentNullException ex)
            {
                Logger.Log.Error(ex, "Error loading configuration for Graph API: Configuration is null.");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "Error loading configuration for Graph API.");
                throw;
            }
        }

        internal static GraphServiceClient ConnectToGraphApi()
        {
            try
            {
                LoadGraphApiConfiguration();
                Logger.Log.Information("Connecting to Graph API...");
                var credentials = new ClientSecretCredential(
                    _tenantId, // Liest die TenantId aus der Konfigurationsdatei.
                    _clientId, // Liest die ClientId aus der Konfigurationsdatei.
                    _clientSecret, // Liest das ClientSecret aus der Konfigurationsdatei.
                    new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });
                Logger.Log.Information("Connected to Graph API successfully.");

                return new GraphServiceClient(credentials);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message);
                throw;
            }
        }

        internal static string GetUserEmail()
        {
            if (!string.IsNullOrEmpty(_userEmail))
            {
                return _userEmail;
            }
            throw new ArgumentNullException(nameof(_userEmail), "UserEmail is null or empty.");
        }

    }
}
