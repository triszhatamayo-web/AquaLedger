
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public class ReportSummary
    {
        public int TotalFarmers { get; set; }
        public int TotalFarms { get; set; }
        public int TotalPonds { get; set; }
        public int TotalFishStock { get; set; }

        public double TotalFeedPurchasedKg { get; set; }
        public double TotalFeedUsedKg { get; set; }

        public int TotalHarvestedFish { get; set; }
        public double TotalHarvestWeightKg { get; set; }

        public double TotalSales { get; set; }
        public double TotalExpenses { get; set; }
    }

    public static class ReportService
    {
        public static ReportSummary GetSummary(
            DateTime startDate,
            DateTime endDate,
            int? farmerId = null,
            int? farmId = null)
        {
            if (startDate.Date > endDate.Date)
                throw new ArgumentException(
                    "Start date cannot be later than end date.");

            ReportSummary report = new ReportSummary();

            using (SqliteConnection connection = Db.GetConnection())
            {
                string farmFilter = @"
                    (@FarmerId IS NULL OR f.FarmerId = @FarmerId)
                    AND (@FarmId IS NULL OR f.Id = @FarmId)";

                // TOTAL FARMERS
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COUNT(DISTINCT fr.Id)
                        FROM Farmers fr
                        INNER JOIN Farms f ON f.FarmerId = fr.Id
                        WHERE " + farmFilter + ";";

                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalFarmers =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                // TOTAL FARMS
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COUNT(*)
                        FROM Farms f
                        WHERE " + farmFilter + ";";

                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalFarms =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                // TOTAL PONDS
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COUNT(*)
                        FROM Ponds p
                        INNER JOIN Farms f ON f.Id = p.FarmId
                        WHERE " + farmFilter + ";";

                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalPonds =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                // CURRENT FISH STOCK
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(SUM(
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
                        ), 0)
                        FROM Ponds p
                        INNER JOIN Farms f ON f.Id = p.FarmId
                        WHERE " + farmFilter + ";";

                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalFishStock =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                // FEED PURCHASED
                // Shared cooperative inventory: not farm-filtered.
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(SUM(QuantityKg), 0)
                        FROM FeedPurchases
                        WHERE PurchaseDate >= @StartDate
                        AND PurchaseDate <= @EndDate;";

                    AddDateParameters(command, startDate, endDate);
                    report.TotalFeedPurchasedKg =
                        Convert.ToDouble(command.ExecuteScalar());
                }

                // FEED USED
                // Filtered through the ponds belonging to the selected farm.
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(SUM(fr.QuantityKg), 0)
                        FROM FeedingRecords fr
                        INNER JOIN Ponds p ON p.Id = fr.PondId
                        INNER JOIN Farms f ON f.Id = p.FarmId
                        WHERE fr.FeedingDate >= @StartDate
                        AND fr.FeedingDate <= @EndDate
                        AND " + farmFilter + ";";

                    AddDateParameters(command, startDate, endDate);
                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalFeedUsedKg =
                        Convert.ToDouble(command.ExecuteScalar());
                }

                // HARVESTED FISH
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(SUM(h.FishCount), 0)
                        FROM Harvests h
                        INNER JOIN Ponds p ON p.Id = h.PondId
                        INNER JOIN Farms f ON f.Id = p.FarmId
                        WHERE h.HarvestDate >= @StartDate
                        AND h.HarvestDate <= @EndDate
                        AND " + farmFilter + ";";

                    AddDateParameters(command, startDate, endDate);
                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalHarvestedFish =
                        Convert.ToInt32(command.ExecuteScalar());
                }

                // HARVEST WEIGHT
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(SUM(h.WeightKg), 0)
                        FROM Harvests h
                        INNER JOIN Ponds p ON p.Id = h.PondId
                        INNER JOIN Farms f ON f.Id = p.FarmId
                        WHERE h.HarvestDate >= @StartDate
                        AND h.HarvestDate <= @EndDate
                        AND " + farmFilter + ";";

                    AddDateParameters(command, startDate, endDate);
                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalHarvestWeightKg =
                        Convert.ToDouble(command.ExecuteScalar());
                }

                // SALES
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(SUM(s.TotalAmount), 0)
                        FROM Sales s
                        INNER JOIN Harvests h ON h.Id = s.HarvestId
                        INNER JOIN Ponds p ON p.Id = h.PondId
                        INNER JOIN Farms f ON f.Id = p.FarmId
                        WHERE s.SaleDate >= @StartDate
                        AND s.SaleDate <= @EndDate
                        AND " + farmFilter + ";";

                    AddDateParameters(command, startDate, endDate);
                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalSales =
                        Convert.ToDouble(command.ExecuteScalar());
                }

                // EXPENSES
                // Farm-specific expenses are filtered by their FarmId.
                // Shared cooperative expenses are included only in
                // the overall cooperative report.
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(SUM(e.Amount), 0)
                        FROM Expenses e
                        LEFT JOIN Farms f ON f.Id = e.FarmId
                        WHERE e.ExpenseDate >= @StartDate
                        AND e.ExpenseDate <= @EndDate
                        AND (
                            (@FarmerId IS NULL AND @FarmId IS NULL)
                            OR (
                                e.FarmId IS NOT NULL
                                AND " + farmFilter + @"
                            )
                        );";

                    AddDateParameters(command, startDate, endDate);
                    AddFilterParameters(command, farmerId, farmId);
                    report.TotalExpenses =
                        Convert.ToDouble(command.ExecuteScalar());
                }
            }

            return report;
        }

        private static void AddDateParameters(
            SqliteCommand command,
            DateTime startDate,
            DateTime endDate)
        {
            command.Parameters.AddWithValue(
                "@StartDate",
                startDate.ToString("yyyy-MM-dd"));

            command.Parameters.AddWithValue(
                "@EndDate",
                endDate.ToString("yyyy-MM-dd"));
        }

        private static void AddFilterParameters(
            SqliteCommand command,
            int? farmerId,
            int? farmId)
        {
            command.Parameters.AddWithValue(
                "@FarmerId",
                farmerId.HasValue ? (object)farmerId.Value : DBNull.Value);

            command.Parameters.AddWithValue(
                "@FarmId",
                farmId.HasValue ? (object)farmId.Value : DBNull.Value);
        }
    }
}
