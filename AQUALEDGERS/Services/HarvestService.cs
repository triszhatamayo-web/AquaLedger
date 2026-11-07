
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class HarvestService
    {
        public static void RecordHarvest(
            int pondId,
            int fishCount,
            double weightKg,
            DateTime harvestDate,
            string notes)
        {
            if (pondId <= 0)
                throw new ArgumentException(
                    "Please select a valid pond.");

            if (fishCount <= 0)
                throw new ArgumentException(
                    "Harvested fish count must be greater than zero.");

            if (weightKg <= 0)
                throw new ArgumentException(
                    "Harvest weight must be greater than zero.");

            using (SqliteConnection connection =
                Db.GetConnection())
            using (SqliteTransaction transaction =
                connection.BeginTransaction())
            {
                string stockSql = @"
                    SELECT
                        p.InitialStock
                        - COALESCE((
                            SELECT SUM(m.Quantity)
                            FROM MortalityRecords m
                            WHERE m.PondId = p.Id
                        ), 0)
                        - COALESCE((
                            SELECT SUM(h.FishCount)
                            FROM Harvests h
                            WHERE h.PondId = p.Id
                        ), 0)
                    FROM Ponds p
                    WHERE p.Id = @PondId;
                ";

                int currentStock;

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = stockSql;
                    command.Parameters.AddWithValue(
                        "@PondId", pondId);

                    object result = command.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                        throw new InvalidOperationException(
                            "The selected pond was not found.");

                    currentStock = Convert.ToInt32(result);
                }

                if (fishCount > currentStock)
                    throw new InvalidOperationException(
                        "Harvest quantity cannot exceed current fish stock.");

                string insertSql = @"
                    INSERT INTO Harvests
                    (PondId, FishCount, WeightKg,
                     HarvestDate, Notes)
                    VALUES
                    (@PondId, @FishCount, @WeightKg,
                     @HarvestDate, @Notes);
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = insertSql;

                    command.Parameters.AddWithValue(
                        "@PondId", pondId);

                    command.Parameters.AddWithValue(
                        "@FishCount", fishCount);

                    command.Parameters.AddWithValue(
                        "@WeightKg", weightKg);

                    command.Parameters.AddWithValue(
                        "@HarvestDate",
                        harvestDate.ToString("yyyy-MM-dd"));

                    command.Parameters.AddWithValue(
                        "@Notes",
                        string.IsNullOrWhiteSpace(notes)
                            ? (object)DBNull.Value
                            : notes.Trim());

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
    }
}
