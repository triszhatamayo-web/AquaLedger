
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public class ProductionSummary
    {
        public int PondId { get; set; }

        public int InitialStock { get; set; }

        public int CurrentStock { get; set; }

        public int TotalMortality { get; set; }

        public int TotalHarvested { get; set; }

        public double? LatestAverageWeight { get; set; }

        public string LatestGrowthDate { get; set; }
    }

    public static class ProductionSummaryService
    {
        public static ProductionSummary GetSummary(int pondId)
        {
            if (pondId <= 0)
                throw new ArgumentException(
                    "Please select a valid pond.");

            ProductionSummary summary = new ProductionSummary();

            using (SqliteConnection connection = Db.GetConnection())
            {
                string sql = @"
                    SELECT
                        InitialStock,
                        COALESCE((
                            SELECT SUM(Quantity)
                            FROM MortalityRecords
                            WHERE PondId = Ponds.Id
                        ), 0),
                        COALESCE((
                            SELECT SUM(FishCount)
                            FROM Harvests
                            WHERE PondId = Ponds.Id
                        ), 0)
                    FROM Ponds
                    WHERE Id = @PondId;
                ";

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.AddWithValue(
                        "@PondId", pondId);

                    using (SqliteDataReader reader =
                        command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException(
                                "The selected pond was not found.");

                        summary.PondId = pondId;
                        summary.InitialStock = reader.GetInt32(0);
                        summary.TotalMortality = reader.GetInt32(1);
                        summary.TotalHarvested = reader.GetInt32(2);
                    }
                }

                summary.CurrentStock =
                    ProductionService.GetCurrentStock(pondId);

                string growthSql = @"
                    SELECT RecordDate, AverageWeight
                    FROM GrowthRecords
                    WHERE PondId = @PondId
                    ORDER BY RecordDate DESC, Id DESC
                    LIMIT 1;
                ";

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = growthSql;

                    command.Parameters.AddWithValue(
                        "@PondId", pondId);

                    using (SqliteDataReader reader =
                        command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.LatestGrowthDate =
                                reader.GetString(0);

                            summary.LatestAverageWeight =
                                reader.GetDouble(1);
                        }
                    }
                }
            }

            return summary;
        }
    }
}
