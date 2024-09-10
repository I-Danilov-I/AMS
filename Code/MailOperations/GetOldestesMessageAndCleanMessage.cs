using Microsoft.Graph.Models;
using Microsoft.Graph;
using System.Text.RegularExpressions;

namespace AMST
{
    internal class GetOldestesMessageFromFolderAndCleanMessage
    {

        internal static async Task<Message?> GetAndClean(GraphServiceClient graphClient, string destinationParentFolderName)
        {
            try
            {
                Logger.Log.Information($"Get Oldest Message from '{destinationParentFolderName}'");
                var mainAllFolders = await graphClient.Users[GraphApiConnector.GetUserEmail()].MailFolders.GetAsync();
                var parentFolder = mainAllFolders?.Value?.FirstOrDefault(f => f.DisplayName == destinationParentFolderName);
                if (parentFolder != null)
                {
                    var messages = await graphClient.Users[GraphApiConnector.GetUserEmail()].MailFolders[parentFolder.Id].Messages
                        .GetAsync(requestConfig =>
                        {
                            requestConfig.Headers.Add("Prefer", "outlook.body-content-type=\"text\"");
                        });

                    var oldestMessage = messages?.Value?.OrderBy(m => m.ReceivedDateTime).FirstOrDefault();
                    if (oldestMessage != null && oldestMessage.Body != null && oldestMessage.Body.Content != null)
                    {
                        oldestMessage.Body.Content = RemoveHtmlLinks(oldestMessage.Body.Content);
                        oldestMessage.Body.Content = Regex.Unescape(oldestMessage.Body.Content);
                        Logger.Log.Information("Get oldest message successfully!");
                        return oldestMessage;
                    }
                    else
                    {
                        Logger.Log.Information($"No messages found: '{destinationParentFolderName}'");
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.Log.Information($"{ex.Message}");
                return null;
            }
        }

        internal static string RemoveHtmlLinks(string input)
        {
            string pattern = @"<[^>]+>";
            return Regex.Replace(input, pattern, "");
        }

        internal static void PrintEmailMessageToConsole(Message message)
        {
            if (message != null)
            {
                Logger.Log.Information("_______________________________________________________________________________________________________________");
                Logger.Log.Information($"Sender: {message?.From?.EmailAddress?.Address}");
                Logger.Log.Information("Date: " + $"{message?.ReceivedDateTime?.ToString("yyyy-MM-dd HH:mm:ss")}");
                Logger.Log.Information("Subject: " + $"{message?.Subject}");
                Logger.Log.Information("Attachments: " + $"{message?.HasAttachments}");
                Logger.Log.Information("Body:\n" + $"{message?.Body?.Content}");
                Logger.Log.Information("_______________________________________________________________________________________________________________");
            }
            else
            {
                Logger.Log.Information("No message available.");
            }
        }

    }
}
