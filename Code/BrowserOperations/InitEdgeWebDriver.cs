using OpenQA.Selenium.Edge;

namespace AMST
{
    internal class InitEdgeWebDriver
    {
        internal static EdgeDriver Init()
        {
            try
            {
                Logger.Log.Information("EdgeDriver initialisation...");

                var service = EdgeDriverService.CreateDefaultService();
                
                var options = new EdgeOptions
                {
                    BinaryLocation = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
                };
                options.AddArgument("window-size=1920,1080");
                options.AddArgument("--log-level=3");
                options.AddArgument("headless");
                options.AddArgument("start-maximized");
                options.AddArgument("disable-gpu");
                options.AddArgument("no-sandbox");
                options.AddArgument("disable-dev-shm-usage");
                options.AddArgument("disable-application-cache");
                options.AddArgument("disk-cache-size=0");
                options.AddArgument("media-cache-size=0");
                options.AddArgument("disable-extensions");
                options.AddArgument("disable-background-networking");
                options.AddArgument("disable-sync");
                options.AddArgument("disable-translate");

                EdgeDriver edgeWebDriver = new(service, options);

                Logger.Log.Information("EdgeDriver initialisation success!");
                return edgeWebDriver;
            }

            catch (Exception ex)
            {
                throw new Exception($"Init Webdrive: {ex.Message}");
            }
        }
    }
}
