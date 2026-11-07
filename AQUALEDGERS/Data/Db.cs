
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace AQUALEDGERS.Data
{
    public static class Db
    {
        private static readonly string FolderPath =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "AquaLedger"
            );

        private static readonly string DbPath =
            Path.Combine(FolderPath, "AquaLedger.db");

        private static readonly string ConnectionString =
            $"Data Source={DbPath}";

        public static void Initialize()
        {
            Directory.CreateDirectory(FolderPath);

            using (var connection =
                new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (var pragmaCommand =
                    connection.CreateCommand())
                {
                    pragmaCommand.CommandText =
                        "PRAGMA foreign_keys = ON;";

                    pragmaCommand.ExecuteNonQuery();
                }

                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PinHash TEXT NOT NULL,
                        FullName TEXT NOT NULL,
                        Role TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
                    );

                    CREATE TABLE IF NOT EXISTS Farmers (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Phone TEXT,
                        Address TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Farms (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FarmerId INTEGER NOT NULL,
                        FarmName TEXT NOT NULL,
                        Address TEXT,
                        FOREIGN KEY (FarmerId) REFERENCES Farmers(Id)
                    );

                    CREATE TABLE IF NOT EXISTS Ponds (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FarmId INTEGER NOT NULL,
                        PondName TEXT NOT NULL,
                        Species TEXT NOT NULL,
                        InitialStock INTEGER NOT NULL DEFAULT 0,
                        StockingDate TEXT,
                        FOREIGN KEY (FarmId) REFERENCES Farms(Id)
                    );

                    CREATE TABLE IF NOT EXISTS MortalityRecords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PondId INTEGER NOT NULL,
                        Quantity INTEGER NOT NULL CHECK (Quantity > 0),
                        RecordDate TEXT NOT NULL,
                        Notes TEXT,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (PondId) REFERENCES Ponds(Id)
                    );

                    CREATE TABLE IF NOT EXISTS Harvests (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PondId INTEGER NOT NULL,
                        FishCount INTEGER NOT NULL CHECK (FishCount > 0),
                        WeightKg REAL NOT NULL CHECK (WeightKg > 0),
                        HarvestDate TEXT NOT NULL,
                        SoldKg REAL NOT NULL DEFAULT 0 CHECK (SoldKg >= 0),
                        Notes TEXT,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (PondId) REFERENCES Ponds(Id),
                        CHECK (SoldKg <= WeightKg)
                    );

                    CREATE TABLE IF NOT EXISTS FeedInventory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FeedType TEXT NOT NULL UNIQUE,
                        QuantityKg REAL NOT NULL DEFAULT 0 CHECK (QuantityKg >= 0),
                        UpdatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
                    );

                    CREATE TABLE IF NOT EXISTS FeedingRecords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PondId INTEGER NOT NULL,
                        FeedType TEXT NOT NULL,
                        QuantityKg REAL NOT NULL CHECK (QuantityKg > 0),
                        FeedingDate TEXT NOT NULL,
                        Notes TEXT,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (PondId) REFERENCES Ponds(Id)
                    );

                    CREATE TABLE IF NOT EXISTS TreatmentRecords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PondId INTEGER NOT NULL,
                        TreatmentName TEXT NOT NULL,
                        TreatmentDate TEXT NOT NULL,
                        Dosage TEXT,
                        Notes TEXT,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (PondId) REFERENCES Ponds(Id)
                    );

                    CREATE TABLE IF NOT EXISTS GrowthRecords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PondId INTEGER NOT NULL,
                        RecordDate TEXT NOT NULL,
                        AverageWeight REAL NOT NULL CHECK (AverageWeight > 0),
                        Notes TEXT,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (PondId) REFERENCES Ponds(Id)
                    );

                    CREATE TABLE IF NOT EXISTS StockVerificationRecords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PondId INTEGER NOT NULL,
                        PreviousStock INTEGER NOT NULL CHECK (PreviousStock >= 0),
                        VerifiedStock INTEGER NOT NULL CHECK (VerifiedStock >= 0),
                        Difference INTEGER NOT NULL,
                        RecordDate TEXT NOT NULL,
                        Notes TEXT,
                        RecordedBy TEXT,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (PondId) REFERENCES Ponds(Id)
                    );

                    CREATE TABLE IF NOT EXISTS Expenses (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ExpenseCategory TEXT NOT NULL,
                        Description TEXT NOT NULL,
                        Amount REAL NOT NULL CHECK (Amount > 0),
                        ExpenseDate TEXT NOT NULL,
                        FarmId INTEGER,
                        PondId INTEGER,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (FarmId) REFERENCES Farms(Id),
                        FOREIGN KEY (PondId) REFERENCES Ponds(Id)
                    );

                    CREATE TABLE IF NOT EXISTS Sales (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        HarvestId INTEGER NOT NULL,
                        BuyerName TEXT,
                        QuantityKg REAL NOT NULL CHECK (QuantityKg > 0),
                        PricePerKg REAL NOT NULL CHECK (PricePerKg >= 0),
                        TotalAmount REAL NOT NULL CHECK (TotalAmount >= 0),
                        SaleDate TEXT NOT NULL,
                        Notes TEXT,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (HarvestId) REFERENCES Harvests(Id)
                    );

                    CREATE TABLE IF NOT EXISTS FeedPurchases (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FeedType TEXT NOT NULL,
                        QuantityKg REAL NOT NULL CHECK (QuantityKg > 0),
                        CostPerKg REAL NOT NULL CHECK (CostPerKg >= 0),
                        TotalCost REAL NOT NULL CHECK (TotalCost >= 0),
                        PurchaseDate TEXT NOT NULL,
                        ExpenseId INTEGER,
                        Notes TEXT,
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (ExpenseId) REFERENCES Expenses(Id)
                    );

                    CREATE TABLE IF NOT EXISTS MarketplaceListings (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FarmerId INTEGER NOT NULL,
                        PondId INTEGER NOT NULL,
                        FishType TEXT NOT NULL,
                        QuantityKg REAL NOT NULL CHECK (QuantityKg > 0),
                        PricePerKg REAL NOT NULL CHECK (PricePerKg >= 0),
                        Status TEXT NOT NULL DEFAULT 'Available',
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (FarmerId) REFERENCES Farmers(Id),
                        FOREIGN KEY (PondId) REFERENCES Ponds(Id)
                    );

                    CREATE TABLE IF NOT EXISTS MarketplaceOrders (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ListingId INTEGER NOT NULL,
                        BuyerId INTEGER NOT NULL,
                        QuantityKg REAL NOT NULL CHECK (QuantityKg > 0),
                        TotalAmount REAL NOT NULL CHECK (TotalAmount >= 0),
                        Status TEXT NOT NULL DEFAULT 'Pending',
                        CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (ListingId) REFERENCES MarketplaceListings(Id),
                        FOREIGN KEY (BuyerId) REFERENCES Users(Id)
                    );
             
                    CREATE TABLE IF NOT EXISTS UserPreferences (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL UNIQUE,
                        Currency TEXT NOT NULL DEFAULT 'PHP',
                        DateFormat TEXT NOT NULL DEFAULT 'MM/dd/yyyy',
                        FOREIGN KEY (UserId) REFERENCES Users(Id)
                    );

                ";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }

            // Automatically update older marketplace tables.
            UpgradeMarketplaceSchema();
            UpgradeFarmersSchema();
        }

        public static SqliteConnection GetConnection()
        {
            var connection =
                new SqliteConnection(ConnectionString);

            connection.Open();

            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "PRAGMA foreign_keys = ON;";

                    command.ExecuteNonQuery();
                }

                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }


        public static void UpgradeMarketplaceSchema()
        {
            using (var connection = GetConnection())
            {
                bool hasReservedKg = false;
                bool hasSoldKg = false;
                bool hasHarvestId = false;

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "PRAGMA table_info(MarketplaceListings);";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader.GetString(1);

                            if (columnName == "ReservedKg")
                                hasReservedKg = true;

                            if (columnName == "SoldKg")
                                hasSoldKg = true;

                            if (columnName == "HarvestId")
                                hasHarvestId = true;
                        }

                    }
                }


                if (!hasReservedKg)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    ALTER TABLE MarketplaceListings
                    ADD COLUMN ReservedKg REAL NOT NULL DEFAULT 0;
                ";

                        command.ExecuteNonQuery();
                    }
                }

                if (!hasSoldKg)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    ALTER TABLE MarketplaceListings
                    ADD COLUMN SoldKg REAL NOT NULL DEFAULT 0;
                ";

                        command.ExecuteNonQuery();
                    }
                }

                if (!hasHarvestId)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    ALTER TABLE MarketplaceListings
                    ADD COLUMN HarvestId INTEGER
                    REFERENCES Harvests(Id);
                ";

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void UpgradeFarmersSchema()
        {
            using (var connection = GetConnection())
            {
                bool hasPhone = false;
                bool hasPhotoPath = false;

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "PRAGMA table_info(Farmers);";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader.GetString(1);

                            if (string.Equals(
                                columnName,
                                "Phone",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                hasPhone = true;
                            }

                            if (string.Equals(
                                columnName,
                                "PhotoPath",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                hasPhotoPath = true;
                            }
                        }
                    }
                }

                if (!hasPhone)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            "ALTER TABLE Farmers ADD COLUMN Phone TEXT;";

                        command.ExecuteNonQuery();
                    }
                }

                if (!hasPhotoPath)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            "ALTER TABLE Farmers ADD COLUMN PhotoPath TEXT;";

                        command.ExecuteNonQuery();
                    }
                }
            }
        }


    }
}
