using System;
using Microsoft.Data.Sqlite;
using BCrypt.Net;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class PinService
    {
        public static bool ChangePin(
            string currentPin,
            string newPin,
            string confirmPin,
            out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(currentPin) ||
                string.IsNullOrWhiteSpace(newPin) ||
                string.IsNullOrWhiteSpace(confirmPin))
            {
                message = "Please fill in all PIN fields.";
                return false;
            }

            if (newPin != confirmPin)
            {
                message = "The new PINs do not match.";
                return false;
            }

            if (newPin.Length < 4 || newPin.Length > 6)
            {
                message = "The new PIN must contain 4 to 6 digits.";
                return false;
            }

            foreach (char digit in newPin)
            {
                if (!char.IsDigit(digit))
                {
                    message = "The PIN must contain numbers only.";
                    return false;
                }
            }

            int userId = AuthService.CurrentUserId;

            using (SqliteConnection connection = Db.GetConnection())
            {
                connection.Open();

                string query = "SELECT PinHash FROM Users WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);

                    object result = command.ExecuteScalar();

                    if (result == null)
                    {
                        message = "User account not found.";
                        return false;
                    }

                    string storedHash = result.ToString();

                    if (!BCrypt.Net.BCrypt.Verify(currentPin, storedHash))
                    {
                        message = "Current PIN is incorrect.";
                        return false;
                    }
                }

                string newHash = BCrypt.Net.BCrypt.HashPassword(newPin);

                string updateQuery =
                    "UPDATE Users SET PinHash = @PinHash WHERE Id = @Id";

                using (SqliteCommand command =
                    new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@PinHash", newHash);
                    command.Parameters.AddWithValue("@Id", userId);

                    command.ExecuteNonQuery();
                }
            }

            message = "PIN changed successfully.";
            return true;
        }
    }
}