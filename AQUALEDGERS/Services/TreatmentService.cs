
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class TreatmentService
    {
        public static void RecordTreatment(
            int pondId,
            string treatmentName,
            DateTime treatmentDate,
            string dosage,
            string notes)
        {
            if (pondId <= 0)
                throw new ArgumentException(
                    "Please select a valid pond.");

            if (string.IsNullOrWhiteSpace(treatmentName))
                throw new ArgumentException(
                    "Please enter the treatment name.");

            using (SqliteConnection connection =
                Db.GetConnection())
            using (SqliteCommand command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO TreatmentRecords
                    (
                        PondId,
                        TreatmentName,
                        TreatmentDate,
                        Dosage,
                        Notes
                    )
                    VALUES
                    (
                        @PondId,
                        @TreatmentName,
                        @TreatmentDate,
                        @Dosage,
                        @Notes
                    );
                ";

                command.Parameters.AddWithValue(
                    "@PondId", pondId);

                command.Parameters.AddWithValue(
                    "@TreatmentName", treatmentName.Trim());

                command.Parameters.AddWithValue(
                    "@TreatmentDate",
                    treatmentDate.ToString("yyyy-MM-dd"));

                command.Parameters.AddWithValue(
                    "@Dosage",
                    string.IsNullOrWhiteSpace(dosage)
                        ? (object)DBNull.Value
                        : dosage.Trim());

                command.Parameters.AddWithValue(
                    "@Notes",
                    string.IsNullOrWhiteSpace(notes)
                        ? (object)DBNull.Value
                        : notes.Trim());

                command.ExecuteNonQuery();
            }
        }
    }
}
