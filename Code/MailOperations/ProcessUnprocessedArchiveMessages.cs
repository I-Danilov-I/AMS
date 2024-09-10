using AMST;
using Microsoft.Graph.Models;
using Microsoft.Graph;
using System;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AMST
{
    internal class DailyTaskScheduler
    {
        private const string INBOX = "Inbox";
        private const string ARCHIVE_INVALID = "Archive invalid";
        private const string ARCHIVE_PROCESSED = "Archive processed";
        private const string ARCHIVE_UNPROCESSED = "Archive unprocessed";

        private static Timer? _timer;


        public static void ScheduleDailyTask(Func<Task> task, TimeSpan runTime)
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan timeToGo = runTime - currentTime;
            if (timeToGo < TimeSpan.Zero)
            {
                timeToGo = timeToGo.Add(new TimeSpan(24, 0, 0)); // Add 24 hours if the time has already passed today
            }

            _timer = new Timer(async x =>
            {
                await task.Invoke();
                ScheduleDailyTask(task, runTime); // Reschedule for the next day
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }



        private static async Task ProcessUnprocessedArchiveMessages(GraphServiceClient graphServiceClient)
        {
            Logger.Log.Information("Move all Messages from Archive Unprocessed in Inbox...");
            while (true)
            {
                Message? messageFromUnprocessed = await GetOldestesMessageFromFolderAndCleanMessage.GetAndClean(graphServiceClient, ARCHIVE_UNPROCESSED);
                if (messageFromUnprocessed == null) { break; }
                await MoveMessageToFolder.MoveTo(graphServiceClient, messageFromUnprocessed!, INBOX);
            }
        }

    }
}
