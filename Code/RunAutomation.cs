using Microsoft.Graph;
using Microsoft.Graph.Models;
using OpenQA.Selenium.Edge;
using System.Runtime.InteropServices;

namespace AMST
{
    internal class RunAutomationLoop
    {
        private const string INBOX = "Inbox";
        private const string ARCHIVE_INVALID = "Archive invalid";
        private const string ARCHIVE_PROCESSED = "Archive processed";
        private const string ARCHIVE_UNPROCESSED = "Archive unprocessed";

        private static readonly string[] _allowedExtensions = [".docx", ".pdf", ".xml", ".doc", ".wpd", ".txt", ".rtf", ".html", ".htm", ".xls", ".xlsx", ".odt"];
        private static EdgeDriver? _edgeDriver;
        private static bool _retryMailsFromArhiveUnprozessed = true;

        internal static async void RunAutomation()
        {
            try
            {
                while (true)
                {
                    Guid uniLoopID = Guid.NewGuid(); // Einzigartige ID
                    Logger.Log.Information("\n\n\n\n");
                    Logger.Log.Information($"ID: [{uniLoopID}]");
                    Logger.Log.Information($"[Automation loop Start]----------------------------------------------------------------------");
                    RessourcenCleaning.WebDriverClean(_edgeDriver!);
                    CleaningProgramFolder.CleanTempAttachmentsFolder();

                    using (_edgeDriver = InitEdgeWebDriver.Init())
                    using (GraphServiceClient graphServiceClient = GraphApiConnector.ConnectToGraphApi())
                    {
                        await GetSpecificFoldersFromUser.GetFolders(graphServiceClient, GraphApiConnector.GetUserEmail());

                        Message? message = await GetOldestesMessageFromFolderAndCleanMessage.GetAndClean(graphServiceClient, INBOX);
                        if (message == null)
                        {
                            if (_retryMailsFromArhiveUnprozessed == true)
                            {
                                Logger.Log.Information("Move all Messages from Archive Unprozessed in Inbox...");
                                while (true)
                                {
                                    Message? messageFromUnprozessed = await GetOldestesMessageFromFolderAndCleanMessage.GetAndClean(graphServiceClient, ARCHIVE_UNPROCESSED);
                                    if ( messageFromUnprozessed == null) { break; }
                                    await MoveMessageToFolder.MoveTo(graphServiceClient, messageFromUnprozessed!, INBOX);
                                }

                                _retryMailsFromArhiveUnprozessed = false;
                                RessourcenCleaning.WebDriverClean(_edgeDriver!);
                                continue;
                            }
                            else
                            {
                                Logger.Log.Information("Continuing loop in 3h...");
                                _retryMailsFromArhiveUnprozessed = true;
                                await Task.Delay(1000 * 60 * 180);
                                RessourcenCleaning.WebDriverClean(_edgeDriver!);
                                continue;
                            }
                        }

                        if (message != null && message.Id != null)
                        {
                            string jsonFileName = Path.Combine("tempJsonFile" + ".json");
                            if (FilterConvertJsonFromMessage.FilterAndConvertToJsonFile(message, jsonFileName) == true)
                            {
                                await DownloadAttachmentsFromMessage.DownloadAttachments(graphServiceClient, message.Id);
                            }
                            else
                            {
                                await MoveMessageToFolder.MoveTo(graphServiceClient, message, ARCHIVE_INVALID);
                                RessourcenCleaning.WebDriverClean(_edgeDriver!);
                                continue;
                            }
                        }

                        try
                        {
                            LoadLogDataForTaleo.Load();
                            TaleoOperations.GetDataForTaleo();
                            TaleoOperations.LoginToTaleo(_edgeDriver);
                            TaleoOperations.NavigationToCandidate(_edgeDriver);

                            if (TaleoOperations.CheckDuplicateAndCanModifyCandidate(_edgeDriver) == false)
                            {
                                Logger.Log.Information("Program restart in 180 sec...");
                                await Task.Delay(1000 * 180);
                                RessourcenCleaning.WebDriverClean(_edgeDriver!);
                                continue;
                            }

                            TaleoOperations.InputCandidateData(_edgeDriver);
                            TaleoOperations.CheckSuccessMessage(_edgeDriver);

                            if (message?.HasAttachments == true) { TaleoOperations.UploadAttachments(_edgeDriver, _allowedExtensions); }

                            TaleoOperations.SendEmailReply(_edgeDriver);

                            if (message != null) { await MoveMessageToFolder.MoveTo(graphServiceClient, message, ARCHIVE_PROCESSED); }

                            TaleoOperations.Logout(_edgeDriver);
                        }
                        catch (Exception ex)
                        {
                            TakeScreenshot.CaptureScreenToFile(_edgeDriver, uniLoopID);
                            Logger.Log.Error(ex, "An error occurred during the Taleo operations.");
                            if (message != null) { await MoveMessageToFolder.MoveTo(graphServiceClient, message, ARCHIVE_UNPROCESSED); }
                        }
                        finally
                        {
                            Logger.Log.Information("Finally");
                            RessourcenCleaning.WebDriverClean(_edgeDriver!);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "An error occurred in the automation loop.");
                RessourcenCleaning.WebDriverClean(_edgeDriver!);
            }
        }
    }
}
