
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public class ExpenseRecord
    {
        public int Id { get; set; }
        public string ExpenseCategory { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string ExpenseDate { get; set; }
    }

    public static class ExpenseService
    {
        public static void RecordExpense(
            string category,
            string description,
            double amount,
            DateTime expenseDate,
            int? farmId,
            int? pondId)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException(
                    "Please select an expense category.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(
                    "Please enter an expense description.");

            if (amount <= 0)
                throw new ArgumentException(
                    "Expense amount must be greater than zero.");

            if (string.Equals(
                category.Trim(),
                "Feed",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Record feed purchases through Feed → Add Feed Stock.");
            }

            using (SqliteConnection connection =
                Db.GetConnection())
            using (SqliteCommand command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Expenses
                    (
                        ExpenseCategory,
                        Description,
                        Amount,
                        ExpenseDate,
                        FarmId,
                        PondId
                    )
                    VALUES
                    (
                        @Category,
                        @Description,
                        @Amount,
                        @ExpenseDate,
                        @FarmId,
                        @PondId
                    );
                ";

                command.Parameters.AddWithValue(
                    "@Category", category.Trim());

                command.Parameters.AddWithValue(
                    "@Description", description.Trim());

                command.Parameters.AddWithValue(
                    "@Amount", amount);

                command.Parameters.AddWithValue(
                    "@ExpenseDate",
                    expenseDate.ToString("yyyy-MM-dd"));

                command.Parameters.AddWithValue(
                    "@FarmId",
                    farmId.HasValue
                        ? (object)farmId.Value
                        : DBNull.Value);

                command.Parameters.AddWithValue(
                    "@PondId",
                    pondId.HasValue
                        ? (object)pondId.Value
                        : DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public static List<ExpenseRecord> GetAllExpenses()
        {
            var expenses = new List<ExpenseRecord>();

            using (SqliteConnection connection =
                Db.GetConnection())
            using (SqliteCommand command =
                connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        Id,
                        ExpenseCategory,
                        Description,
                        Amount,
                        ExpenseDate
                    FROM Expenses
                    ORDER BY ExpenseDate DESC, Id DESC;
                ";

                using (SqliteDataReader reader =
                    command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        expenses.Add(new ExpenseRecord
                        {
                            Id = reader.GetInt32(0),
                            ExpenseCategory = reader.GetString(1),
                            Description = reader.GetString(2),
                            Amount = reader.GetDouble(3),
                            ExpenseDate = reader.GetString(4)
                        });
                    }
                }
            }

            return expenses;
        }
    }
}
