
using System;
using System.IO;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class BackupService
    {
        public static bool BackupDatabase(
            string destinationPath,
            out string message)
        {
            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                message = "Please select a backup location.";
                return false;
            }

            try
            {
                string fullDestinationPath =
                    Path.GetFullPath(destinationPath);

                string folderPath =
                    Path.GetDirectoryName(fullDestinationPath);

                if (string.IsNullOrWhiteSpace(folderPath))
                {
                    message = "Invalid backup location.";
                    return false;
                }

                Directory.CreateDirectory(folderPath);

                using (var sourceConnection = Db.GetConnection())
                using (var destinationConnection =
                    new SqliteConnection(
                        $"Data Source={fullDestinationPath}"))
                {
                    destinationConnection.Open();

                    sourceConnection.BackupDatabase(
                        destinationConnection);
                }

                message = "Database backup completed successfully!";
                return true;
            }
            catch (Exception)
            {
                message =
                    "Unable to create database backup. Please try again.";

                return false;
            }
        }
    }
}
