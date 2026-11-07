using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public class StockVerificationRecord
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public int PreviousStock { get; set; }
        public int VerifiedStock { get; set; }
        public int Difference { get; set; }
        public string RecordDate { get; set; }
        public string Notes { get; set; }
        public string RecordedBy { get; set; }
    }

    public static class StockVerificationService
    {
        // RECORD STOCK VERIFICATION
        public static int RecordVerification(
            int pondId,
            int verifiedStock,
            DateTime recordDate,
            string notes,
            string recordedBy)
        {
            if (pondId <= 0)
                throw new ArgumentException(
                    "Please select a valid pond.");

            if (verifiedStock < 0)
                throw new ArgumentException(
                    "Verified stock cannot be negative.");

            using (SqliteConnection connection =
                Db.GetConnection())
            {
                int previousStock =
                    ProductionService.GetCurrentStock(pondId);

                int difference =
                    verifiedStock - previousStock;

                string sql = @"
                    INSERT INTO StockVerificationRecords
                    (
                        PondId,
                        PreviousStock,
                        VerifiedStock,
                        Difference,
                        RecordDate,
                        Notes,
                        RecordedBy
                    )
                    VALUES
                    (
                        @PondId,
                        @PreviousStock,
                        @VerifiedStock,
                        @Difference,
                        @RecordDate,
                        @Notes,
                        @RecordedBy
                    );

                    SELECT last_insert_rowid();
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.AddWithValue(
                        "@PondId", pondId);

                    command.Parameters.AddWithValue(
                        "@PreviousStock", previousStock);

                    command.Parameters.AddWithValue(
                        "@VerifiedStock", verifiedStock);

                    command.Parameters.AddWithValue(
                        "@Difference", difference);

                    command.Parameters.AddWithValue(
                        "@RecordDate",
                        recordDate.ToString("yyyy-MM-dd"));

                    command.Parameters.AddWithValue(
                        "@Notes",
                        string.IsNullOrWhiteSpace(notes)
                            ? (object)DBNull.Value
                            : notes.Trim());

                    command.Parameters.AddWithValue(
                        "@RecordedBy",
                        string.IsNullOrWhiteSpace(recordedBy)
                            ? (object)DBNull.Value
                            : recordedBy.Trim());

                    return Convert.ToInt32(
                        command.ExecuteScalar());
                }
            }
        }

        // GET STOCK VERIFICATION HISTORY
        public static List<StockVerificationRecord>
            GetVerificationHistory(int pondId)
        {
            if (pondId <= 0)
                throw new ArgumentException(
                    "Please select a valid pond.");

            var records =
                new List<StockVerificationRecord>();

            using (SqliteConnection connection =
                Db.GetConnection())
            using (SqliteCommand command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        Id,
                        PondId,
                        PreviousStock,
                        VerifiedStock,
                        Difference,
                        RecordDate,
                        Notes,
                        RecordedBy
                    FROM StockVerificationRecords
                    WHERE PondId = @PondId
                    ORDER BY RecordDate DESC, Id DESC;
                ";

                command.Parameters.AddWithValue(
                    "@PondId", pondId);

                using (SqliteDataReader reader =
                    command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        records.Add(
                            new StockVerificationRecord
                            {
                                Id = reader.GetInt32(0),
                                PondId = reader.GetInt32(1),
                                PreviousStock = reader.GetInt32(2),
                                VerifiedStock = reader.GetInt32(3),
                                Difference = reader.GetInt32(4),
                                RecordDate = reader.GetString(5),

                                Notes = reader.IsDBNull(6)
                                    ? ""
                                    : reader.GetString(6),

                                RecordedBy = reader.IsDBNull(7)
                                    ? ""
                                    : reader.GetString(7)
                            });
                    }
                }
            }

            return records;
        }
    }
}