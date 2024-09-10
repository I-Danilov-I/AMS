using Microsoft.Graph.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace AMST
{
    internal class FilterConvertJsonFromMessage
    {
        internal static bool FilterAndConvertToJsonFile(Message message, string jsonFileName)
        {
            try
            {

                Logger.Log.Information("Filter and Convert to JSON...");
                string path = Path.Combine(Directory.GetCurrentDirectory().Replace("bin\\Debug\\net8.0", ""), "Code\\JsonOperations\\TempJsonFiles", jsonFileName);
 
                string pattern = @"{[^{]*Salutation[^}]*}"; // Reguläre Ausdrucks für das Suchmuster
                if (message?.Body?.Content != null)
                {
                    var jsonMatch = Regex.Match(message.Body.Content, pattern, RegexOptions.IgnoreCase); // Suchen des Musters im Inhalt der Nachricht
                    if (jsonMatch.Success)
                    {

                        var bodyContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonMatch.ToString());
                        string jsonContent = JsonConvert.SerializeObject(bodyContent, Formatting.Indented);

                        File.WriteAllText(path, jsonContent);

                        Logger.Log.Information("Filter and Convert to JSON successfully!");
                        return true;
                    }
                    else
                    {
                        Logger.Log.Error("Pattern not found.");
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Filter and Convert to JSON: { ex.Message}");
                return false;
            }
        }
    }
}
