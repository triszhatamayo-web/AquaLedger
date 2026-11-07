
using System;
using System.Collections.Generic;

namespace AQUALEDGERS.Services
{
    public class PondRecord
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public string PondName { get; set; }
        public string Species { get; set; }
        public int InitialStock { get; set; }
        public string StockingDate { get; set; }
    }

    public static class PondService
    {
        public static int AddPond(
            int farmId,
            string pondName,
            string species,
            int initialStock,
            string stockingDate)
        {
            if (farmId <= 0)
                throw new ArgumentException(
                    "Please select a valid farm.");

            if (string.IsNullOrWhiteSpace(pondName))
                throw new ArgumentException(
                    "Pond name is required.");

            if (string.IsNullOrWhiteSpace(species))
                throw new ArgumentException(
                    "Fish species is required.");

            if (initialStock < 0)
                throw new ArgumentException(
                    "Initial stock cannot be negative.");

            DateTime parsedDate;

            if (!string.IsNullOrWhiteSpace(stockingDate) &&
                !DateTime.TryParse(
                    stockingDate, out parsedDate))
            {
                throw new ArgumentException(
                    "Please enter a valid stocking date.");
            }

            using (var connection =
                AQUALEDGERS.Data.Db.GetConnection())
            using (var command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Ponds
                        (FarmId, PondName, Species,
                         InitialStock, StockingDate)
                    SELECT
                        @FarmId, @PondName, @Species,
                        @InitialStock, @StockingDate
                    WHERE EXISTS (
                        SELECT 1 FROM Farms
                        WHERE Id = @FarmId
                    );

                    SELECT changes();
                ";

                command.Parameters.AddWithValue(
                    "@FarmId", farmId);

                command.Parameters.AddWithValue(
                    "@PondName", pondName.Trim());

                command.Parameters.AddWithValue(
                    "@Species", species.Trim());

                command.Parameters.AddWithValue(
                    "@InitialStock", initialStock);

                command.Parameters.AddWithValue(
                    "@StockingDate",
                    string.IsNullOrWhiteSpace(stockingDate)
                        ? (object)DBNull.Value
                        : DateTime.Parse(stockingDate).ToString("yyyy-MM-dd"));

                int inserted = Convert.ToInt32(
                    command.ExecuteScalar());

                if (inserted == 0)
                {
                    throw new ArgumentException(
                        "The selected farm does not exist.");
                }

                command.CommandText =
                    "SELECT last_insert_rowid();";

                return Convert.ToInt32(
                    command.ExecuteScalar());
            }
        }

        public static List<PondRecord> GetPondsByFarm(
            int farmId)
        {
            var ponds = new List<PondRecord>();

            using (var connection =
                AQUALEDGERS.Data.Db.GetConnection())
            using (var command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT Id, FarmId, PondName, Species,
                           InitialStock, StockingDate
                    FROM Ponds
                    WHERE FarmId = @FarmId
                    ORDER BY PondName;
                ";

                command.Parameters.AddWithValue(
                    "@FarmId", farmId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ponds.Add(new PondRecord
                        {
                            Id = reader.GetInt32(0),
                            FarmId = reader.GetInt32(1),
                            PondName = reader.GetString(2),
                            Species = reader.GetString(3),
                            InitialStock = reader.GetInt32(4),
                            StockingDate = reader.IsDBNull(5)
                                ? ""
                                : reader.GetString(5)
                        });
                    }
                }
            }

            return ponds;
        }
    }
}
