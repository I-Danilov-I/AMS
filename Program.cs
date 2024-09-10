using System.Reflection; // Ermöglicht das Lokalisieren des aktuellen Programmverzeichnisses
using Topshelf; // Bibliothek zur Erstellung von Windows-Diensten

namespace AMST
{
    class Program
    {
        static void Main()
        {
            // Konfiguration des Topshelf-Dienstes
            HostFactory.Run(x =>
            {
                x.Service<Program>(s =>
                {
                    s.ConstructUsing(name => new Program()); // Konstruktion des Dienstes
                    s.WhenStarted(tc => tc.Start()); // Aktion beim Start des Dienstes
                    s.WhenStopped(tc => tc.Stop()); // Aktion beim Stoppen des Dienstes
                });
                x.RunAsNetworkService(); // Verwende ein anderes Konto mit mehr Berechtigungen

                x.SetDescription("Automatisierten Verarbeitung von Bewerberdaten für das interne Bewerbermanagement-System Taleo."); // Beschreibung des Dienstes
                x.SetDisplayName("Aplicant Manangment Service Taleo"); // Anzeigename des Dienstes
                x.SetServiceName("AMST"); // Dienstname
            });
        }

        // Methode, die beim Start des Dienstes ausgeführt wird
        public async void Start()
        {
            try
            {
                await Task.Delay(1000);
                string? programDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (programDirectory == null) { Logger.Log.Information("Program directory not find."); throw new Exception(); };

                // Loop wird gestartet
                RunAutomationLoop.RunAutomation();
            }

            catch (Exception ex)
            {
                Logger.Log.Error("Service starting error: ", ex);
            }
        }

        // Methode, die beim Stoppen des Dienstes ausgeführt wird
        public void Stop()
        {
            try
            {
                Logger.Log.Information("On Stop");
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Service stopping error: ", ex);
            }
        }
    }
}

