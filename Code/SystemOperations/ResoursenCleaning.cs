using OpenQA.Selenium.Edge;

namespace AMST
{
    internal static class RessourcenCleaning
    {
        internal static void WebDriverClean(EdgeDriver edgeDriver)
        {
            if (edgeDriver != null)
            {
                Logger.Log.Information("Edge driver Quit...");
                edgeDriver?.Quit();
                edgeDriver?.Dispose();
                if (edgeDriver != null)
                {
                    Logger.Log.Information("Edge prozesses kill...");
                    var processes = System.Diagnostics.Process.GetProcessesByName("msedge");
                    foreach (var process in processes)
                    {
                        process.Kill();
                    }
                    Logger.Log.Information("Edge driver kill prozesses succesfully!");
                }

            }
            else
            {
                Logger.Log.Information("Edge driver not active.");
                return;
            }
            Thread.Sleep(1000*10);
        }
    }
}
