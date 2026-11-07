
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;
using System.Collections.Generic;

namespace AQUALEDGERS.Services
{
    public static class GrowthService
    {
        public static void RecordGrowth(
            int pondId,
            DateTime recordDate,
            double averageWeight,
            string notes)
        {
            if (pondId <= 0)
                throw new ArgumentException(
                    "Please select a valid pond.");

            if (averageWeight <= 0)
                throw new ArgumentException(
                    "Average weight must be greater than zero.");

            using (SqliteConnection connection = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO GrowthRecords
                    (PondId, RecordDate, AverageWeight, Notes)
                    VALUES
                    (@PondId, @RecordDate, @AverageWeight, @Notes);
                ";

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.AddWithValue(
                        "@PondId", pondId);

                    command.Parameters.AddWithValue(
                        "@RecordDate",
                        recordDate.ToString("yyyy-MM-dd"));

                    command.Parameters.AddWithValue(
                        "@AverageWeight", averageWeight);

                    command.Parameters.AddWithValue(
                        "@Notes",
                        string.IsNullOrWhiteSpace(notes)
                            ? (object)DBNull.Value
                            : notes.Trim());

                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<string> GetGrowthHistory(int pondId)
        {
            if (pondId <= 0)
                throw new ArgumentException(
                    "Please select a valid pond.");

            List<string> records = new List<string>();

            using (SqliteConnection connection = Db.GetConnection())
            {
                string sql = @"
            SELECT RecordDate, AverageWeight, Notes
            FROM GrowthRecords
            WHERE PondId = @PondId
            ORDER BY RecordDate DESC, Id DESC;
        ";

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@PondId", pondId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string recordDate = reader.GetString(0);
                            double averageWeight = reader.GetDouble(1);

                            string notes = reader.IsDBNull(2)
                                ? ""
                                : reader.GetString(2);

                            string record =
                                $"{recordDate} | {averageWeight} | {notes}";

                            records.Add(record);
                        }
                    }
                }
            }

            return records;
        }

    }
}
