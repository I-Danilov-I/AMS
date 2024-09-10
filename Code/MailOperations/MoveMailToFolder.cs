using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.Messages.Item.Move;

namespace AMST
{
    internal static class MoveMessageToFolder
    {
        internal static async Task MoveTo(GraphServiceClient graphClient, Message oldestMessage, string parentFolderName)
        {
            try
            {
                Logger.Log.Information("Moving message to folder...");
                if (graphClient == null)
                {
                    Logger.Log.Error("GraphServiceClient is null.");
                    return;
                }

                if (oldestMessage?.From?.EmailAddress?.Address == null)
                {
                    Logger.Log.Error("The 'From' address of the message is null.");
                    return;
                }

                if (string.IsNullOrEmpty(parentFolderName))
                {
                    Logger.Log.Error("Parent folder name is null or empty.");
                    return;
                }

                var parentFolders = await graphClient.Users[GraphApiConnector.GetUserEmail()].MailFolders.GetAsync();

                if (parentFolders?.Value == null)
                {
                    Logger.Log.Error("No mail folders found for the user.");
                    return;
                }

                var parentFolder = parentFolders.Value.FirstOrDefault(folder =>
                    folder?.DisplayName != null && folder.DisplayName.Equals(parentFolderName, StringComparison.OrdinalIgnoreCase));

                if (parentFolder == null)
                {
                    Logger.Log.Error($"Parent folder not found: '{parentFolderName}'");
                    return;
                }


                var requestBody = new MovePostRequestBody
                {
                    DestinationId = parentFolder.Id,
                };

                var result = await graphClient.Users[GraphApiConnector.GetUserEmail()].Messages[oldestMessage.Id].Move.PostAsync(requestBody);
                Logger.Log.Information($"Message moved to folder '{parentFolderName}' Successfully!");
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"{ex.Message}");
            }
        }
    }
}
