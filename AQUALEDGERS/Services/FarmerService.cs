
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace AQUALEDGERS.Services
{
    public class FarmerRecord
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PhotoPath { get; set; }
    }

    public static class FarmerService
    {
        public static int AddFarmer(
            string fullName,
            string phone,
            string address)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException(
                    "Farmer name is required.");
            }

            using (var connection =
                AQUALEDGERS.Data.Db.GetConnection())
            using (var command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Farmers
                        (FullName, Phone, Address)
                    VALUES
                        (@FullName, @Phone, @Address);
                ";

                command.Parameters.AddWithValue(
                    "@FullName", fullName.Trim());

                command.Parameters.AddWithValue(
                    "@Phone",
                    string.IsNullOrWhiteSpace(phone)
                        ? (object)DBNull.Value
                        : phone.Trim());

                command.Parameters.AddWithValue(
                    "@Address",
                    string.IsNullOrWhiteSpace(address)
                        ? (object)DBNull.Value
                        : address.Trim());

                command.ExecuteNonQuery();

                command.CommandText =
                    "SELECT last_insert_rowid();";

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public static List<FarmerRecord> GetAllFarmers()
        {
            var farmers = new List<FarmerRecord>();

            using (var connection =
                AQUALEDGERS.Data.Db.GetConnection())
            using (var command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT Id, FullName, Phone, Address, PhotoPath
                    FROM Farmers
                    ORDER BY Id DESC;
                ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        farmers.Add(new FarmerRecord
                        {
                            Id = reader.GetInt32(0),

                            FullName = reader.IsDBNull(1)
                                ? ""
                                : reader.GetString(1),

                            Phone = reader.IsDBNull(2)
                                ? ""
                                : reader.GetString(2),

                            Address = reader.IsDBNull(3)
                                ? ""
                                : reader.GetString(3),

                            PhotoPath = reader.IsDBNull(4)
                                ? null
                                : reader.GetString(4)
                        });
                    }
                }
            }

            return farmers;
        }
    }
}
