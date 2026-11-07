
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class FeedService
    {
        public static void AddFeedStock(
            string feedType,
            double quantityKg,
            double costPerKg,
            DateTime purchaseDate,
            string notes)
        {
            if (string.IsNullOrWhiteSpace(feedType))
                throw new ArgumentException(
                    "Please enter the feed type.");

            if (quantityKg <= 0)
                throw new ArgumentException(
                    "Feed quantity must be greater than zero.");

            if (costPerKg < 0)
                throw new ArgumentException(
                    "Feed cost cannot be negative.");

            double totalCost = quantityKg * costPerKg;

            using (SqliteConnection connection =
                Db.GetConnection())
            using (SqliteTransaction transaction =
                connection.BeginTransaction())
            {
                string inventorySql = @"
                    INSERT INTO FeedInventory
                    (FeedType, QuantityKg, UpdatedAt)
                    VALUES
                    (@FeedType, @QuantityKg, CURRENT_TIMESTAMP)
                    ON CONFLICT(FeedType)
                    DO UPDATE SET
                        QuantityKg =
                            FeedInventory.QuantityKg
                            + excluded.QuantityKg,
                        UpdatedAt = CURRENT_TIMESTAMP;
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = inventorySql;

                    command.Parameters.AddWithValue(
                        "@FeedType", feedType.Trim());

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.ExecuteNonQuery();
                }

                string expenseSql = @"
                    INSERT INTO Expenses
                    (ExpenseCategory, Description, Amount,
                     ExpenseDate)
                    VALUES
                    ('Feed', @Description, @Amount, @ExpenseDate);
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = expenseSql;

                    command.Parameters.AddWithValue(
                        "@Description",
                        "Feed purchase: " + feedType.Trim()
                        + " (" + quantityKg + " kg)");

                    command.Parameters.AddWithValue(
                        "@Amount", totalCost);

                    command.Parameters.AddWithValue(
                        "@ExpenseDate",
                        purchaseDate.ToString("yyyy-MM-dd"));

                    command.ExecuteNonQuery();
                }

                string purchaseSql = @"
                    INSERT INTO FeedPurchases
                    (FeedType, QuantityKg, CostPerKg,
                     TotalCost, PurchaseDate, Notes)
                    VALUES
                    (@FeedType, @QuantityKg, @CostPerKg,
                     @TotalCost, @PurchaseDate, @Notes);
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = purchaseSql;

                    command.Parameters.AddWithValue(
                        "@FeedType", feedType.Trim());

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@CostPerKg", costPerKg);

                    command.Parameters.AddWithValue(
                        "@TotalCost", totalCost);

                    command.Parameters.AddWithValue(
                        "@PurchaseDate",
                        purchaseDate.ToString("yyyy-MM-dd"));

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

        public static void RecordFeeding(
            int pondId,
            string feedType,
            double quantityKg,
            DateTime feedingDate,
            string notes)
        {
            if (pondId <= 0)
                throw new ArgumentException(
                    "Please select a valid pond.");

            if (string.IsNullOrWhiteSpace(feedType))
                throw new ArgumentException(
                    "Please select a feed type.");

            if (quantityKg <= 0)
                throw new ArgumentException(
                    "Feeding quantity must be greater than zero.");

            using (SqliteConnection connection =
                Db.GetConnection())
            using (SqliteTransaction transaction =
                connection.BeginTransaction())
            {
                string stockSql = @"
                    SELECT QuantityKg
                    FROM FeedInventory
                    WHERE FeedType = @FeedType;
                ";

                double availableKg;

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = stockSql;

                    command.Parameters.AddWithValue(
                        "@FeedType", feedType.Trim());

                    object result = command.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                        throw new InvalidOperationException(
                            "No inventory exists for this feed type.");

                    availableKg = Convert.ToDouble(result);
                }

                if (quantityKg > availableKg)
                    throw new InvalidOperationException(
                        "Feeding quantity exceeds available feed stock.");

                string updateSql = @"
                    UPDATE FeedInventory
                    SET QuantityKg = QuantityKg - @QuantityKg,
                        UpdatedAt = CURRENT_TIMESTAMP
                    WHERE FeedType = @FeedType;
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = updateSql;

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@FeedType", feedType.Trim());

                    command.ExecuteNonQuery();
                }

                string recordSql = @"
                    INSERT INTO FeedingRecords
                    (PondId, FeedType, QuantityKg,
                     FeedingDate, Notes)
                    VALUES
                    (@PondId, @FeedType, @QuantityKg,
                     @FeedingDate, @Notes);
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = recordSql;

                    command.Parameters.AddWithValue(
                        "@PondId", pondId);

                    command.Parameters.AddWithValue(
                        "@FeedType", feedType.Trim());

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@FeedingDate",
                        feedingDate.ToString("yyyy-MM-dd"));

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
