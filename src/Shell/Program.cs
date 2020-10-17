using Migrator;
using System;
using System.IO;

namespace Shell
{
    class Program
    {
        //TODO refactor sequential programming style
        //TODO Input validation on ReadLines()
        static void Main(string[] args)
        {
            //Welcome text
            Console.WriteLine("Beat Saber Save Migrator");
            Console.WriteLine("=================================================================================================");
            Console.WriteLine("This tool migrates your existing local beat saber highscores to version 1.12.1.");
            Console.WriteLine("There are two files, that are mandatory to complete this task:");
            Console.WriteLine("LocalLeaderboards.dat - which, who wouldn't have guessed, contains your local leaderboards");
            Console.WriteLine("SongHashData.dat - which contains some hashes that are used in Beat Saber versions prior to v1.12.1");
            Console.WriteLine("");
            Console.WriteLine("This program uses the default locations of this files - if you somehow use another location");
            Console.WriteLine("than the default, please input it when prompted.");

            //Choose input files
            //TODO refactor to prevent copy pasta coding
            string appDataPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)).FullName; // is there a better way to retrieve appdata or locallow folder?
            string localLowPath = Path.Combine(appDataPath, @"LocalLow\Hyperbolic Magnetism\Beat Saber\");

            Console.WriteLine("Select the LocalLeaderboards.dat or keep empty to use default.");
            string localLeaderboardPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(localLeaderboardPath))
            {
                localLeaderboardPath = Path.Combine(localLowPath, "LocalLeaderboards.dat");
            }

            if (!File.Exists(localLeaderboardPath))
            {
                Console.WriteLine("File does not exist. Please try again.");

                Environment.Exit(0); //TODO Handle retry
            }

            Console.WriteLine("Select the SongHashData.dat or keep empty to use default.");
            string songHashDataPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(songHashDataPath))
            {
                songHashDataPath = Path.Combine(localLowPath, "SongHashData.dat");
            }

            if (!File.Exists(songHashDataPath))
            {
                Console.WriteLine("File does not exist. Please try again.");

                Environment.Exit(0); //TODO Handle retry
            }

            //Do backup?
            {
                Console.WriteLine("Do a backup before migration? [y/n] (Default: y)");
                string result = Console.ReadLine();
                bool doBackup = result.ToLower().Equals("y") || string.IsNullOrWhiteSpace(result); //Do backup per default

                if (doBackup)
                {
                    string defaultBackupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Beat Saber Save Migrator");
                    Console.WriteLine(@$"Choose a backup directory: (Default: {defaultBackupDirectory})");
                    string backupPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(backupPath))
                    {
                        backupPath = defaultBackupDirectory;
                    }

                    Directory.CreateDirectory(backupPath);

                    //Also, create a directory with the timestamp of the backup
                    backupPath = Path.Combine(backupPath, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                    Directory.CreateDirectory(backupPath);

                    //Copy-Paste files to backup
                    File.Copy(localLeaderboardPath, Path.Combine(backupPath, Path.GetFileName(localLeaderboardPath)));
                    File.Copy(songHashDataPath, Path.Combine(backupPath, Path.GetFileName(songHashDataPath)));
                }
            }

            //Migrate - this is where the magic happens
            SaveMigrator saveMigrator = new SaveMigrator();
            saveMigrator.Up(localLeaderboardPath, songHashDataPath);
        }
    }
}
