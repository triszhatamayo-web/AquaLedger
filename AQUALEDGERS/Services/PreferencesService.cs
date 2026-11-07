
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public class UserPreferences
    {
        public string Currency { get; set; } = "PHP";
        public string DateFormat { get; set; } = "MM/dd/yyyy";
    }

    public static class PreferencesService
    {
        public static UserPreferences GetPreferences()
        {
            int userId = AuthService.CurrentUserId;

            if (userId <= 0)
                throw new InvalidOperationException(
                    "No user is currently logged in.");

            using (var connection = Db.GetConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT OR IGNORE INTO UserPreferences (UserId)
                    VALUES (@userId);

                    SELECT Currency, DateFormat
                    FROM UserPreferences
                    WHERE UserId = @userId;";

                command.Parameters.AddWithValue(
                    "@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new InvalidOperationException(
                            "Preferences could not be loaded.");

                    return new UserPreferences
                    {
                        Currency = reader.GetString(0),
                        DateFormat = reader.GetString(1)
                    };
                }
            }
        }

        public static bool SavePreferences(
            string currency,
            string dateFormat,
            out string message)
        {
            int userId = AuthService.CurrentUserId;

            if (userId <= 0)
            {
                message = "No user is currently logged in.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(currency) ||
                string.IsNullOrWhiteSpace(dateFormat))
            {
                message = "Please select your preferences.";
                return false;
            }

            try
            {
                using (var connection = Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO UserPreferences
                            (UserId, Currency, DateFormat)
                        VALUES
                            (@userId, @currency, @dateFormat)
                        ON CONFLICT(UserId)
                        DO UPDATE SET
                            Currency = excluded.Currency,
                            DateFormat = excluded.DateFormat;";

                    command.Parameters.AddWithValue(
                        "@userId", userId);

                    command.Parameters.AddWithValue(
                        "@currency", currency);

                    command.Parameters.AddWithValue(
                        "@dateFormat", dateFormat);

                    command.ExecuteNonQuery();
                }

                message = "Preferences saved successfully!";
                return true;
            }
            catch (Exception)
            {
                message = "Unable to save preferences.";
                return false;
            }
        }
    }
}
