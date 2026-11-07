
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class ProductionService
    {
        public static int GetCurrentStock(int pondId)
        {
            if (pondId <= 0)
            {
                throw new ArgumentException(
                    "Please select a valid pond.");
            }

            using (SqliteConnection connection =
                Db.GetConnection())
            {
                string sql = @"
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

                using (SqliteCommand command =
                    connection.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.AddWithValue(
                        "@PondId", pondId);

                    object result = command.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "The selected pond was not found.");
                    }

                    return Convert.ToInt32(result);
                }
            }
        }
    }
}
