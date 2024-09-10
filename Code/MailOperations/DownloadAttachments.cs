using Microsoft.Graph.Models;
using Microsoft.Graph;
using System.Reflection;

namespace AMST
{
    internal class DownloadAttachmentsFromMessage
    {
        private static readonly string[] _allowedExtensions = [".docx", ".pdf", ".xml", ".doc", ".wpd", ".txt", ".rtf", ".html", ".htm", ".xls", ".xlsx", ".odt"];

        internal static async Task DownloadAttachments(GraphServiceClient client, string messageId)
        {
            Logger.Log.Information("Downloading attachments from Message...");

            string? ProgramDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!.Replace("bin\\Debug\\net8.0", "");
            if (ProgramDirectory == null) { Logger.Log.Information("Program directory not find."); throw new Exception(); };
            string folderPathForTempAttachments = Path.Combine(ProgramDirectory, "Code\\MailOperations\\TempAttachmentsFiles\\");


            if (client == null)
            {
                Logger.Log.Error("GraphServiceClient is null.");
                return;
            }
            if (string.IsNullOrEmpty(messageId))
            {
                Logger.Log.Error("Message ID is null or empty.");
                return;
            }
            if (string.IsNullOrEmpty(folderPathForTempAttachments))
            {
                Logger.Log.Error("Folder path is null or empty.");
                return;
            }
            if (_allowedExtensions == null || _allowedExtensions.Length == 0)
            {
                Logger.Log.Error("Allowed extensions array is null or empty.");
                return;
            }

            try
            {
                var attachmentsPage = await client.Users[GraphApiConnector.GetUserEmail()].Messages[messageId].Attachments.GetAsync();
                if (attachmentsPage?.Value == null || attachmentsPage.Value.Count == 0)
                {
                    Logger.Log.Warning("No attachments found for the message.");
                    return;
                }

                foreach (var attachment in attachmentsPage.Value)
                {
                    if (attachment is FileAttachment fileAttachment)
                    {
                        var fileName = fileAttachment.Name;
                        if (fileName == null)
                        {
                            Logger.Log.Error("File name is null.");
                            continue;
                        }

                        var lastDotIndex = fileName.LastIndexOf('.');
                        var cleanFileName = lastDotIndex > 0 ? string.Concat(fileName[..lastDotIndex].Replace('.', '_'), fileName.AsSpan(lastDotIndex)) : fileName;
                        var fileExtension = Path.GetExtension(cleanFileName);

                        if (!string.IsNullOrEmpty(fileExtension) &&
                            _allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase) &&
                            cleanFileName.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            var filePath = Path.Combine(folderPathForTempAttachments, cleanFileName);
                            try
                            {
                                if (fileAttachment.ContentBytes == null)
                                {
                                    Logger.Log.Error($"File content is null for '{cleanFileName}'.");
                                    continue;
                                }

                                File.WriteAllBytes(filePath, fileAttachment.ContentBytes);
                                Logger.Log.Information($"Downloading attachments from Message succesfully: '{cleanFileName}'");
                            }
                            catch (Exception ex)
                            {
                                Logger.Log.Error($"{ex.Message}");
                            }
                        }
                        else
                        {
                            Logger.Log.Warning($"Download ignored: '{cleanFileName}'");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error retrieving attachments: {ex.Message}");
            }
        }
    }
}
