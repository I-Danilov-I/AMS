namespace AMST
{
    internal static class CleaningProgramFolder
    {
        internal static void CleanTempAttachmentsFolder()
        {
            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory().Replace("bin\\Debug\\net8.0", ""), "Code\\MailOperations\\TempAttachmentsFiles");
                Directory.CreateDirectory(folderPath);
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    File.Delete(file);
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message);
                throw;
            }
        }
    }
}
