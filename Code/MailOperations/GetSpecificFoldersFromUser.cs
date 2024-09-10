using Microsoft.Graph.Models;
using Microsoft.Graph;

namespace AMST
{
    internal class GetSpecificFoldersFromUser
    {
        internal const string INBOX = "Inbox";
        internal const string ARCHIVE_INVALID = "Archive invalid";
        internal const string ARCHIVE_PROCESSED = "Archive processed";
        internal const string ARCHIVE_UNPROCESSED = "Archive unprocessed";

        internal static async Task<List<MailFolder>?> GetFolders(GraphServiceClient graphClient, string userEmail)
        {
            List<MailFolder> allFolders = [];
            try
            {
                Logger.Log.Information($"Retrieving all folders from Email: {userEmail}");
                var getAllFolders = await graphClient.Users[userEmail].MailFolders.GetAsync();
                if (getAllFolders != null && getAllFolders.Value != null)
                {
                    allFolders.AddRange(getAllFolders.Value);
                    List<string> folderInfos = []; // Liste zum Speichern der Ordnerinformationen

                    foreach (var folder in allFolders)
                    {
                        var detailedFolder = await graphClient.Users[userEmail].MailFolders[folder.Id].GetAsync();

                        if (detailedFolder != null && (folder.DisplayName == INBOX || folder.DisplayName == ARCHIVE_PROCESSED || folder.DisplayName == ARCHIVE_UNPROCESSED || folder.DisplayName == ARCHIVE_INVALID))
                        {
                            folderInfos.Add($"{folder.DisplayName} [{detailedFolder?.TotalItemCount}]\t");
                        }
                    }

                    Logger.Log.Information(string.Join(" | ", folderInfos)); // Ausgabe aller Ordnerinformationen in einer Zeile
                }
                return allFolders;
            }
            catch (Exception ex)
            {
                Logger.Log.Information(ex.Message);
                return null;
            }
        }
    }
}
