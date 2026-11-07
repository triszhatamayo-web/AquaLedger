
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace AQUALEDGERS.Services
{
    public class MarketplaceListing
    {
        public int Id { get; set; }
        public int FarmerId { get; set; }
        public int PondId { get; set; }
        public int HarvestId { get; set; }
        public string FishType { get; set; }
        public double QuantityKg { get; set; }
        public double PricePerKg { get; set; }
        public double ReservedKg { get; set; }
        public double SoldKg { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }

        public double AvailableKg
        {
            get
            {
                return QuantityKg - ReservedKg - SoldKg;
            }
        }
    }

    public static class MarketplaceService
    {
        public static int CreateListing(
            int farmerId,
            int pondId,
            int harvestId,
            string fishType,
            double quantityKg,
            double pricePerKg)
        {
            if (farmerId <= 0)
                throw new ArgumentException("Please select a valid farmer.");

            if (pondId <= 0)
                throw new ArgumentException("Please select a valid pond.");

            if (harvestId <= 0)
                throw new ArgumentException("Please select a valid harvest.");

            if (string.IsNullOrWhiteSpace(fishType))
                throw new ArgumentException("Fish type is required.");

            if (quantityKg <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (pricePerKg < 0)
                throw new ArgumentException("Price cannot be negative.");

            using (var connection = Data.Db.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                double harvestWeight;
                double harvestSoldKg;
                int actualPondId;
                string actualSpecies;

                // Verify that the harvest belongs to this farmer and pond.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT
                            h.WeightKg,
                            h.SoldKg,
                            h.PondId,
                            p.Species
                        FROM Harvests h
                        INNER JOIN Ponds p ON p.Id = h.PondId
                        INNER JOIN Farms f ON f.Id = p.FarmId
                        WHERE h.Id = @HarvestId
                          AND p.Id = @PondId
                          AND f.FarmerId = @FarmerId;
                    ";

                    command.Parameters.AddWithValue("@HarvestId", harvestId);
                    command.Parameters.AddWithValue("@PondId", pondId);
                    command.Parameters.AddWithValue("@FarmerId", farmerId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new InvalidOperationException(
                                "The selected harvest does not belong to this farmer and pond.");
                        }

                        harvestWeight = reader.GetDouble(0);
                        harvestSoldKg = reader.GetDouble(1);
                        actualPondId = reader.GetInt32(2);
                        actualSpecies = reader.GetString(3);
                    }
                }

                // Count quantities already listed from this harvest.
                double alreadyListedKg = 0;

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT COALESCE(SUM(QuantityKg), 0)
                        FROM MarketplaceListings
                        WHERE HarvestId = @HarvestId;
                    ";

                    command.Parameters.AddWithValue("@HarvestId", harvestId);

                    alreadyListedKg = Convert.ToDouble(
                        command.ExecuteScalar());
                }

                double remainingKg =
                    harvestWeight - harvestSoldKg - alreadyListedKg;

                if (quantityKg > remainingKg)
                {
                    throw new InvalidOperationException(
                        "The listing quantity exceeds the remaining unlisted harvest. " +
                        "Available to list: " + remainingKg.ToString("0.##") + " kg.");
                }

                if (!string.Equals(
                    fishType.Trim(),
                    actualSpecies,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "The fish type must match the selected pond's species.");
                }

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO MarketplaceListings
                        (
                            FarmerId,
                            PondId,
                            HarvestId,
                            FishType,
                            QuantityKg,
                            PricePerKg,
                            Status,
                            ReservedKg,
                            SoldKg
                        )
                        VALUES
                        (
                            @FarmerId,
                            @PondId,
                            @HarvestId,
                            @FishType,
                            @QuantityKg,
                            @PricePerKg,
                            'Available',
                            0,
                            0
                        );

                        SELECT last_insert_rowid();
                    ";

                    command.Parameters.AddWithValue("@FarmerId", farmerId);
                    command.Parameters.AddWithValue("@PondId", actualPondId);
                    command.Parameters.AddWithValue("@HarvestId", harvestId);
                    command.Parameters.AddWithValue("@FishType", actualSpecies);
                    command.Parameters.AddWithValue("@QuantityKg", quantityKg);
                    command.Parameters.AddWithValue("@PricePerKg", pricePerKg);

                    int listingId = Convert.ToInt32(
                        command.ExecuteScalar());

                    transaction.Commit();
                    return listingId;
                }
            }
        }

        public static List<MarketplaceListing> GetAvailableListings()
        {
            var listings = new List<MarketplaceListing>();

            using (var connection = Data.Db.GetConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        Id,
                        FarmerId,
                        PondId,
                        HarvestId,
                        FishType,
                        QuantityKg,
                        PricePerKg,
                        ReservedKg,
                        SoldKg,
                        Status,
                        CreatedAt
                    FROM MarketplaceListings
                    WHERE Status = 'Available'
                      AND QuantityKg > ReservedKg + SoldKg
                    ORDER BY CreatedAt DESC;
                ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listings.Add(new MarketplaceListing
                        {
                            Id = reader.GetInt32(0),
                            FarmerId = reader.GetInt32(1),
                            PondId = reader.GetInt32(2),
                            HarvestId = reader.GetInt32(3),
                            FishType = reader.GetString(4),
                            QuantityKg = reader.GetDouble(5),
                            PricePerKg = reader.GetDouble(6),
                            ReservedKg = reader.GetDouble(7),
                            SoldKg = reader.GetDouble(8),
                            Status = reader.GetString(9),
                            CreatedAt = reader.GetString(10)
                        });
                    }
                }
            }

            return listings;
        }
    }
}
