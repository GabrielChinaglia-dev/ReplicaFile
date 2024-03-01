using System;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: SyncFolders <source_folder> <replica_folder> <interval_in_seconds>");
            return;
        }

        string sourceFolder = args[0];
        string replicaFolder = args[1];
        int intervalSeconds = int.Parse(args[2]);

        string logFilePath = "sync_log.txt";

        Console.WriteLine($"Syncing folders '{sourceFolder}' to '{replicaFolder}' every {intervalSeconds} seconds...");

        // Perform initial synchronization
        SynchronizeFolders(sourceFolder, replicaFolder, logFilePath);

        // Set up a timer to periodically synchronize folders
        Timer timer = new Timer(_ =>
        {
            SynchronizeFolders(sourceFolder, replicaFolder, logFilePath);
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalSeconds));

        Console.WriteLine("Press 'q' to quit.");
        while (Console.ReadKey().Key != ConsoleKey.Q) ;
    }

    static void SynchronizeFolders(string sourceFolder, string replicaFolder, string logFilePath)
    {
        try
        {
            using (StreamWriter logFile = File.AppendText(logFilePath))
            {
                logFile.WriteLine($"[{DateTime.Now}] Synchronization started.");

                // Ensure replica folder exists
                if (!Directory.Exists(replicaFolder))
                    Directory.CreateDirectory(replicaFolder);

                // Synchronize files
                foreach (string file in Directory.GetFiles(sourceFolder))
                {
                    string destFile = Path.Combine(replicaFolder, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                    logFile.WriteLine($"[{DateTime.Now}] Copied file: {file} to {destFile}");
                }

                // Delete extra files in replica folder
                foreach (string file in Directory.GetFiles(replicaFolder))
                {
                    string sourceFile = Path.Combine(sourceFolder, Path.GetFileName(file));
                    if (!File.Exists(sourceFile))
                    {
                        File.Delete(file);
                        logFile.WriteLine($"[{DateTime.Now}] Deleted file: {file}");
                    }
                }

                logFile.WriteLine($"[{DateTime.Now}] Synchronization complete.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during synchronization: {ex.Message}");
        }
    }
}
// To test the code, open the "Properties" in the Solution Explorer
// Go to the "Debug" tab
// In the "Comand Line Arguments" pass the Source file path, the Replica file path and the interval of time, for example 60 for 60 seconds