using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace AMST
{
    internal static class TakeScreenshot
    {
        internal static int counter = 0; 

        internal static void CaptureScreenToFile(EdgeDriver driver, Guid id)
        {
            try
            {
                Logger.Log.Information("Take screenshot...");
                string currentProjectPath = Directory.GetCurrentDirectory().Replace("bin\\Debug\\net8.0", "");
                string fileName = $"[ID_{id}]";       
                string fullPath = Path.Combine(currentProjectPath, "Log_Files", fileName + ".png");

                Screenshot screenshot = driver.GetScreenshot();
                screenshot.SaveAsFile(fullPath);
                Logger.Log.Information("Screenshot save succefully!");
            }
            catch(Exception ex)
            {
                Logger.Log.Warning($"Take screenshot failed:  {ex.Message}");
            }
        }
    }
}
