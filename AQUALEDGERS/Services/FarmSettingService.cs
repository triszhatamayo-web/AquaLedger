
using System;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;

namespace AQUALEDGERS.Services
{
    public static class FarmSettingsService
    {
        public static bool UpdateFarm(
            int farmId,
            string farmName,
            string address,
            out string message)
        {
            farmName = farmName?.Trim() ?? "";
            address = address?.Trim() ?? "";

            if (farmId <= 0)
            {
                message = "Please select a valid farm.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(farmName))
            {
                message = "Farm name is required.";
                return false;
            }

            try
            {
                using (var connection = Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Farms
                        SET FarmName = @farmName,
                            Address = @address
                        WHERE Id = @farmId;";

                    command.Parameters.AddWithValue(
                        "@farmName", farmName);

                    command.Parameters.AddWithValue(
                        "@address", address);

                    command.Parameters.AddWithValue(
                        "@farmId", farmId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        message = "Farm could not be found.";
                        return false;
                    }
                }

                message = "Farm information updated successfully!";
                return true;
            }
            catch (Exception)
            {
                message = "Unable to update farm information.";
                return false;
            }
        }
    }
}
