
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class SalesService
    {
        public static void RecordSale(
            int harvestId,
            string buyerName,
            double quantityKg,
            double pricePerKg,
            DateTime saleDate,
            string notes)
        {
            if (harvestId <= 0)
                throw new ArgumentException(
                    "Please select a valid harvest.");

            if (quantityKg <= 0)
                throw new ArgumentException(
                    "Sale quantity must be greater than zero.");

            if (pricePerKg < 0)
                throw new ArgumentException(
                    "Price per kilogram cannot be negative.");

            double totalAmount = quantityKg * pricePerKg;

            using (SqliteConnection connection =
                Db.GetConnection())
            using (SqliteTransaction transaction =
                connection.BeginTransaction())
            {
                string harvestSql = @"
                    SELECT WeightKg, SoldKg
                    FROM Harvests
                    WHERE Id = @HarvestId;
                ";

                double harvestWeight;
                double soldKg;

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = harvestSql;

                    command.Parameters.AddWithValue(
                        "@HarvestId", harvestId);

                    using (SqliteDataReader reader =
                        command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException(
                                "The selected harvest was not found.");

                        harvestWeight = reader.GetDouble(0);
                        soldKg = reader.GetDouble(1);
                    }
                }

                double availableKg = harvestWeight - soldKg;

                if (quantityKg > availableKg)
                    throw new InvalidOperationException(
                        "Sale quantity exceeds available harvested fish.");

                string updateSql = @"
                    UPDATE Harvests
                    SET SoldKg = SoldKg + @QuantityKg
                    WHERE Id = @HarvestId
                      AND SoldKg + @QuantityKg <= WeightKg;
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = updateSql;

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@HarvestId", harvestId);

                    int rowsUpdated = command.ExecuteNonQuery();

                    if (rowsUpdated != 1)
                        throw new InvalidOperationException(
                            "The sale could not be completed. Please check the available harvest quantity.");
                }

                string insertSql = @"
                    INSERT INTO Sales
                    (
                        HarvestId,
                        BuyerName,
                        QuantityKg,
                        PricePerKg,
                        TotalAmount,
                        SaleDate,
                        Notes
                    )
                    VALUES
                    (
                        @HarvestId,
                        @BuyerName,
                        @QuantityKg,
                        @PricePerKg,
                        @TotalAmount,
                        @SaleDate,
                        @Notes
                    );
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = insertSql;

                    command.Parameters.AddWithValue(
                        "@HarvestId", harvestId);

                    command.Parameters.AddWithValue(
                        "@BuyerName",
                        string.IsNullOrWhiteSpace(buyerName)
                            ? (object)DBNull.Value
                            : buyerName.Trim());

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@PricePerKg", pricePerKg);

                    command.Parameters.AddWithValue(
                        "@TotalAmount", totalAmount);

                    command.Parameters.AddWithValue(
                        "@SaleDate",
                        saleDate.ToString("yyyy-MM-dd"));

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
