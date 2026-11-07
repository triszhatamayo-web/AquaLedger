
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }

    public static class ProfileService
    {
        public static UserProfile GetCurrentProfile()
        {
            int userId = AuthService.CurrentUserId;

            if (userId <= 0)
            {
                throw new InvalidOperationException(
                    "No user is currently logged in.");
            }

            using (var connection = Db.GetConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT Id, Username, FullName, Role, CreatedAt
                    FROM Users
                    WHERE Id = @userId
                    LIMIT 1;";

                command.Parameters.AddWithValue(
                    "@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "User profile could not be found.");
                    }

                    return new UserProfile
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        FullName = reader.GetString(2),
                        Role = reader.GetString(3),
                        CreatedAt = reader.GetString(4)
                    };
                }
            }
        }
    }
}
