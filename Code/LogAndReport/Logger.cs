using Serilog; // Namespace für die Serilog-Bibliothek, die für das Logging verwendet wird
using System.Reflection; // Namespace für die Arbeit mit Metadaten und Assembly-Informationen

namespace AMST
{
    internal static class Logger
    {
        // Statische Eigenschaft für das Logger-Objekt
        internal static ILogger Log { get; }

        // Statischer Konstruktor zur Initialisierung des Loggers
        static Logger()
        {
            // Bestimmen des Verzeichnisses der ausführenden Assembly
            string? assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyLocation != null)
            {
                // Festlegen des Pfads für das Log-Verzeichnis
                string logDirectory = Path.Combine(assemblyLocation.Replace("bin\\Debug\\net8.0", ""), "Log_Files");

                // Konfiguration des Loggers mit täglichem Rolling der Logdateien und Ausgabe in die Konsole
                Log = new LoggerConfiguration()
                    .WriteTo.File(
                        Path.Combine(logDirectory, "Log.txt"), // Dateiname im Format Log_yyyy_MM_dd.txt
                        rollingInterval: RollingInterval.Day
                    )
                    .WriteTo.Console()
                    .CreateLogger();
            }
            else
            {
                // Konfiguration des Loggers nur mit Konsolenausgabe, falls das Verzeichnis nicht bestimmt werden kann
                Log = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
                Log.Warning("[Program start without logfile.]");
            }
        }
    }
}
