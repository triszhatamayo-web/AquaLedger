
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace AQUALEDGERS.Services
{
    public class FarmRecord
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }
        public string FarmName { get; set; }
        public string Address { get; set; }
    }

    public static class FarmService
    {
        public static int AddFarm(
            int farmerId,
            string farmName,
            string address)
        {
            if (farmerId <= 0)
            {
                throw new ArgumentException(
                    "Please select a valid farmer.");
            }

            if (string.IsNullOrWhiteSpace(farmName))
            {
                throw new ArgumentException(
                    "Farm name is required.");
            }

            using (var connection =
                AQUALEDGERS.Data.Db.GetConnection())
            using (var command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Farms
                        (FarmerId, FarmName, Address)
                    SELECT
                        @FarmerId, @FarmName, @Address
                    WHERE EXISTS (
                        SELECT 1 FROM Farmers
                        WHERE Id = @FarmerId
                    );

                    SELECT changes();
                ";

                command.Parameters.AddWithValue(
                    "@FarmerId", farmerId);

                command.Parameters.AddWithValue(
                    "@FarmName", farmName.Trim());

                command.Parameters.AddWithValue(
                    "@Address",
                    string.IsNullOrWhiteSpace(address)
                        ? (object)DBNull.Value
                        : address.Trim());

                int inserted = Convert.ToInt32(
                    command.ExecuteScalar());

                if (inserted == 0)
                {
                    throw new ArgumentException(
                        "The selected farmer does not exist.");
                }

                command.CommandText =
                    "SELECT last_insert_rowid();";

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public static List<FarmRecord> GetFarmsByFarmer(
            int farmerId)
        {
            var farms = new List<FarmRecord>();

            using (var connection =
                AQUALEDGERS.Data.Db.GetConnection())
            using (var command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT Id, FarmerId, FarmName, Address
                    FROM Farms
                    WHERE FarmerId = @FarmerId
                    ORDER BY FarmName;
                ";

                command.Parameters.AddWithValue(
                    "@FarmerId", farmerId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        farms.Add(new FarmRecord
                        {
                            Id = reader.GetInt32(0),
                            FarmerId = reader.GetInt32(1),
                            FarmName = reader.GetString(2),
                            Address = reader.IsDBNull(3)
                                ? ""
                                : reader.GetString(3)
                        });
                    }
                }
            }

            return farms;
        }
    }
}
