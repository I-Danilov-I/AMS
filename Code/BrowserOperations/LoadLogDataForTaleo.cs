using Microsoft.Extensions.Configuration;

namespace AMST
{
    internal static class LoadLogDataForTaleo
    {
        private static IConfigurationRoot? _taleoLogData;

        // Sensible Daten als private Felder
        private static string? _taleoLoginUsername;
        private static string? _taleoLoginPassword;
        private static string? _taleoStartsite;

        // Öffentliche Methoden zum Zugriff auf sensible Daten
        public static string? GetTaleoLoginUsername() => _taleoLoginUsername;
        public static string? GetTaleoLoginPassword() => _taleoLoginPassword;
        public static string? GetTaleoStartsite() => _taleoStartsite;

        public static void Load()
        {
            try
            {
                Logger.Log.Information("Configuration loaded...");

                string currentProjectPath = Directory.GetCurrentDirectory().Replace("bin\\Debug\\net8.0", "");
                string secretsFolderName = "Code\\Secrets";
                string secretsFileName = "secret.json";
                string fullName = Path.Combine(currentProjectPath, secretsFolderName, secretsFileName);

                _taleoLogData = new ConfigurationBuilder()
                    .SetBasePath(currentProjectPath)
                    .AddJsonFile(fullName)
                    .Build();

                // Laden der sensiblen Daten in private Felder
                _taleoLoginUsername = _taleoLogData["Taleo:TALEO_LOGIN_USERNAME"];
                _taleoLoginPassword = _taleoLogData["Taleo:TALEO_LOGIN_PASSWORT"];
                _taleoStartsite = _taleoLogData["Taleo:TALEO_LOGIN_STARTSITE"];

                Logger.Log.Information("Configuration loaded successfully.");
            }
            catch (FileNotFoundException ex)
            {
                Logger.Log.Fatal(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message);
                throw;
            }
        }
    }
}
