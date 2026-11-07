
using Microsoft.Data.Sqlite;
using System;

namespace AQUALEDGERS.Services
{
    public static class MarketplaceOrderService
    {
        // Create an order and reserve its quantity.
        public static int PlaceOrder(
            int listingId,
            int buyerId,
            double quantityKg)
        {
            if (listingId <= 0)
                throw new ArgumentException("Invalid listing.");

            if (buyerId <= 0)
                throw new ArgumentException("Invalid buyer.");

            if (quantityKg <= 0)
                throw new ArgumentException(
                    "Order quantity must be greater than zero.");

            using (var connection = Data.Db.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                // Confirm that the account is a buyer.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT COUNT(*)
                        FROM Users
                        WHERE Id = @BuyerId
                          AND Role = 'Buyer';
                    ";

                    command.Parameters.AddWithValue("@BuyerId", buyerId);

                    if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                        throw new InvalidOperationException(
                            "Only registered buyer accounts can place orders.");
                }

                double pricePerKg;

                // Read the listing price.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT PricePerKg
                        FROM MarketplaceListings
                        WHERE Id = @ListingId
                          AND Status = 'Available';
                    ";

                    command.Parameters.AddWithValue("@ListingId", listingId);

                    object result = command.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        throw new InvalidOperationException(
                            "This listing is unavailable.");

                    pricePerKg = Convert.ToDouble(result);
                }

                // Reserve stock only if enough unreserved quantity remains.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        UPDATE MarketplaceListings
                        SET ReservedKg = ReservedKg + @QuantityKg
                        WHERE Id = @ListingId
                          AND Status = 'Available'
                          AND QuantityKg - ReservedKg - SoldKg
                              >= @QuantityKg;
                    ";

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@ListingId", listingId);

                    if (command.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(
                            "Not enough fish are available. Please refresh the listing.");
                }

                double totalAmount = quantityKg * pricePerKg;

                int orderId;

                // Save the pending order.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO MarketplaceOrders
                        (
                            ListingId,
                            BuyerId,
                            QuantityKg,
                            TotalAmount,
                            Status
                        )
                        VALUES
                        (
                            @ListingId,
                            @BuyerId,
                            @QuantityKg,
                            @TotalAmount,
                            'Pending'
                        );

                        SELECT last_insert_rowid();
                    ";

                    command.Parameters.AddWithValue(
                        "@ListingId", listingId);

                    command.Parameters.AddWithValue(
                        "@BuyerId", buyerId);

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@TotalAmount", totalAmount);

                    orderId = Convert.ToInt32(command.ExecuteScalar());
                }

                transaction.Commit();

                return orderId;
            }
        }

        // Cancel a pending order and release its reserved quantity.
        public static void CancelOrder(int orderId)
        {
            ChangePendingOrder(orderId, "Cancelled");
        }

        // Reject a pending order and release its reserved quantity.
        public static void RejectOrder(int orderId)
        {
            ChangePendingOrder(orderId, "Rejected");
        }

        private static void ChangePendingOrder(
            int orderId,
            string newStatus)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order.");

            using (var connection = Data.Db.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                int listingId;
                double quantityKg;

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT ListingId, QuantityKg
                        FROM MarketplaceOrders
                        WHERE Id = @OrderId
                          AND Status = 'Pending';
                    ";

                    command.Parameters.AddWithValue("@OrderId", orderId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException(
                                "The order does not exist or is no longer pending.");

                        listingId = reader.GetInt32(0);
                        quantityKg = reader.GetDouble(1);
                    }
                }

                // Release the reserved quantity.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        UPDATE MarketplaceListings
                        SET ReservedKg = ReservedKg - @QuantityKg,
                            Status = 'Available'
                        WHERE Id = @ListingId
                          AND ReservedKg >= @QuantityKg;
                    ";

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@ListingId", listingId);

                    if (command.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(
                            "Could not release the reserved quantity.");
                }

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        UPDATE MarketplaceOrders
                        SET Status = @Status
                        WHERE Id = @OrderId
                          AND Status = 'Pending';
                    ";

                    command.Parameters.AddWithValue("@Status", newStatus);
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    if (command.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(
                            "The order status could not be updated.");
                }

                transaction.Commit();
            }
        }

        // Complete delivery and record the marketplace sale.
        public static void MarkOrderDelivered(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order.");

            using (var connection = Data.Db.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                int listingId;
                int buyerId;
                int harvestId;
                double quantityKg;
                double pricePerKg;
                string buyerName;

                // Get order and its linked harvest.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT
                            o.ListingId,
                            o.BuyerId,
                            l.HarvestId,
                            o.QuantityKg,
                            l.PricePerKg,
                            u.FullName
                        FROM MarketplaceOrders o
                        INNER JOIN MarketplaceListings l
                            ON l.Id = o.ListingId
                        INNER JOIN Users u
                            ON u.Id = o.BuyerId
                        WHERE o.Id = @OrderId
                          AND o.Status = 'Pending';
                    ";

                    command.Parameters.AddWithValue("@OrderId", orderId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new InvalidOperationException(
                                "The order does not exist or is no longer pending.");

                        if (reader.IsDBNull(2))
                            throw new InvalidOperationException(
                                "This listing is not linked to a harvest.");

                        listingId = reader.GetInt32(0);
                        buyerId = reader.GetInt32(1);
                        harvestId = reader.GetInt32(2);
                        quantityKg = reader.GetDouble(3);
                        pricePerKg = reader.GetDouble(4);
                        buyerName = reader.GetString(5);
                    }
                }

                // Confirm the reservation still exists.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        UPDATE MarketplaceListings
                        SET ReservedKg = ReservedKg - @QuantityKg,
                            SoldKg = SoldKg + @QuantityKg,
                            Status = CASE
                                WHEN QuantityKg - SoldKg - @QuantityKg <= 0
                                    THEN 'Sold'
                                ELSE 'Available'
                            END
                        WHERE Id = @ListingId
                          AND ReservedKg >= @QuantityKg
                          AND QuantityKg - SoldKg >= @QuantityKg;
                    ";

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@ListingId", listingId);

                    if (command.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(
                            "The listing no longer has enough reserved stock.");
                }

                // Update the original harvest's sold quantity.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        UPDATE Harvests
                        SET SoldKg = SoldKg + @QuantityKg
                        WHERE Id = @HarvestId
                          AND WeightKg - SoldKg >= @QuantityKg;
                    ";

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@HarvestId", harvestId);

                    if (command.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(
                            "The harvest does not have enough unsold fish.");
                }

                // Record the sale in the existing Sales table.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
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

                    command.Parameters.AddWithValue(
                        "@HarvestId", harvestId);

                    command.Parameters.AddWithValue(
                        "@BuyerName", buyerName);

                    command.Parameters.AddWithValue(
                        "@QuantityKg", quantityKg);

                    command.Parameters.AddWithValue(
                        "@PricePerKg", pricePerKg);

                    command.Parameters.AddWithValue(
                        "@TotalAmount", quantityKg * pricePerKg);

                    command.Parameters.AddWithValue(
                        "@SaleDate", DateTime.Now.ToString("yyyy-MM-dd"));

                    command.Parameters.AddWithValue(
                        "@Notes", "Marketplace order #" + orderId);

                    command.ExecuteNonQuery();
                }

                // Mark the order delivered only after all updates succeed.
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        UPDATE MarketplaceOrders
                        SET Status = 'Delivered'
                        WHERE Id = @OrderId
                          AND Status = 'Pending';
                    ";

                    command.Parameters.AddWithValue("@OrderId", orderId);

                    if (command.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(
                            "The order status could not be updated.");
                }

                transaction.Commit();
            }
        }
    }
}
