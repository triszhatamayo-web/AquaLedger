
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class AuthService

    {
        public static int CurrentUserId { get; private set; } = 0;
        public static string CurrentUserRole { get; private set; } = "";
        public static void CreateDefaultUser()
        {
            using (var connection = Db.GetConnection())
            {
                using (var checkCommand = connection.CreateCommand())
                {
                    checkCommand.CommandText =
                        "SELECT COUNT(*) FROM Users;";

                    long count = (long)checkCommand.ExecuteScalar();

                    if (count > 0)
                        return;
                }

                string pinHash =
                    BCrypt.Net.BCrypt.HashPassword("1234");

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Users
                            (Username, PinHash, FullName, Role)
                        VALUES
                            (@username, @pinHash, @fullName, @role);";

                    command.Parameters.AddWithValue(
                        "@username", "coop");

                    command.Parameters.AddWithValue(
                        "@pinHash", pinHash);

                    command.Parameters.AddWithValue(
                        "@fullName", "AquaLedger User");

                    command.Parameters.AddWithValue(
                        "@role", "Admin");

                    command.ExecuteNonQuery();
                }
            }
        }


        public static bool ValidateLogin(
            string username,
            string pin,
            string accountType)
        {
            using (var connection = Db.GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                SELECT Id, PinHash
                FROM Users
                WHERE Username = @username
                  AND Role = @role
                LIMIT 1;";

                    command.Parameters.AddWithValue(
                        "@username", username.Trim());

                    string role = accountType == "Buyer"
                        ? "Buyer"
                        : "Admin";

                    command.Parameters.AddWithValue("@role", role);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            CurrentUserId = 0;
                            CurrentUserRole = "";
                            return false;
                        }

                        int userId = reader.GetInt32(0);
                        string storedHash = reader.GetString(1);

                        if (!BCrypt.Net.BCrypt.Verify(pin, storedHash))
                        {
                            CurrentUserId = 0;
                            CurrentUserRole = "";
                            return false;
                        }

                        CurrentUserId = userId;
                        CurrentUserRole = role;
                        return true;
                    }
                }
            }
        }


        public static bool RegisterBuyer(
            string fullName,
            string username,
            string pin,
            out string message)
        {
            fullName = fullName.Trim();
            username = username.Trim();

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pin))
            {
                message = "Please complete all fields.";
                return false;
            }

            if (username.Length > 50)
            {
                message =
                    "Username cannot exceed 50 characters.";

                return false;
            }

            if (pin.Length < 4 || pin.Length > 6 ||
                !System.Text.RegularExpressions.Regex.IsMatch(
                    pin, @"^\d+$"))
            {
                message = "PIN must contain 4 to 6 digits.";
                return false;
            }

            string pinHash =
                BCrypt.Net.BCrypt.HashPassword(pin);

            try
            {
                using (var connection = Db.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO Users
                                (Username, PinHash, FullName, Role)
                            VALUES
                                (@username, @pinHash, @fullName, 'Buyer');";

                        command.Parameters.AddWithValue(
                            "@username", username);

                        command.Parameters.AddWithValue(
                            "@pinHash", pinHash);

                        command.Parameters.AddWithValue(
                            "@fullName", fullName);

                        command.ExecuteNonQuery();
                    }
                }

                message = "Buyer account created successfully!";
                return true;
            }
            catch (SqliteException ex)
                when (ex.SqliteErrorCode == 19)
            {
                message =
                    "That username is already taken. Please choose another.";

                return false;
            }
            catch (Exception)
            {
                message =
                    "Unable to create the account. Please try again.";

                return false;
            }
        }
    }
}
