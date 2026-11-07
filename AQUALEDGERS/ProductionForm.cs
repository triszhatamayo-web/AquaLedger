
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using AQUALEDGERS.Data;
using AQUALEDGERS.Services;


namespace AQUALEDGERS
{
    public partial class ProductionForm : Form
    {
        public ProductionForm()
        {
            Text = "AquaLedger - Production Tests";
            Size = new Size(560, 320);
            StartPosition = FormStartPosition.CenterScreen;

            Label resultLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12),
                Text = "Choose a test below."
            };

            Button stockButton = new Button
            {
                Text = "TEST CURRENT STOCK",
                Dock = DockStyle.Top,
                Height = 45
            };

            Button mortalityButton = new Button
            {
                Text = "TEST INVALID MORTALITY",
                Dock = DockStyle.Bottom,
                Height = 45
            };

            Button summaryButton = new Button
            {
                Text = "TEST PRODUCTION SUMMARY",
                Dock = DockStyle.Bottom,
                Height = 45
            };

            Button harvestButton = new Button
            {
                Text = "TEST INVALID HARVEST",
                Dock = DockStyle.Bottom,
                Height = 45
            };

            stockButton.Click += (sender, e) =>
            {
                try
                {
                    int stock =
                        ProductionService.GetCurrentStock(2);

                    resultLabel.Text =
                        $"Current Stock: {stock} fish\n" +
                        (stock == 3700
                            ? "PASS"
                            : "CHECK RESULT");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Stock Test Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };

            mortalityButton.Click += (sender, e) =>
            {
                try
                {
                    MortalityService.RecordMortality(
                        2,
                        3701,
                        DateTime.Today,
                        "Invalid mortality validation test");

                    resultLabel.Text =
                        "FAIL: Invalid mortality was accepted.";
                }
                catch (InvalidOperationException ex)
                {
                    resultLabel.Text =
                        "PASS: Invalid mortality rejected.\n" +
                        ex.Message;
                }
                catch (Exception ex)
                {
                    resultLabel.Text =
                        "CHECK RESULT:\n" + ex.Message;
                }
            };

            harvestButton.Click += (sender, e) =>
            {
                try
                {
                    HarvestService.RecordHarvest(
                        2,
                        3701,
                        100,
                        DateTime.Today,
                        "Invalid harvest validation test");

                    resultLabel.Text =
                        "FAIL: Invalid harvest was accepted.";
                }
                catch (InvalidOperationException ex)
                {
                    resultLabel.Text =
                        "PASS: Invalid harvest rejected.\n" +
                        ex.Message;
                }
                catch (Exception ex)
                {
                    resultLabel.Text =
                        "CHECK RESULT:\n" + ex.Message;
                }
            };

            summaryButton.Click += (sender, e) =>
            {
                try
                {
                    ProductionSummary summary =
                        ProductionSummaryService.GetSummary(2);

                    resultLabel.Text =
                        $"Initial Stock: {summary.InitialStock}\n" +
                        $"Mortality: {summary.TotalMortality}\n" +
                        $"Harvested: {summary.TotalHarvested}\n" +
                        $"Current Stock: {summary.CurrentStock}\n" +
                        $"Latest Weight: " +
                        $"{summary.LatestAverageWeight?.ToString() ?? "No record"}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.ToString(),
                        "Production Summary Test Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };

            Controls.Add(summaryButton);
            Controls.Add(harvestButton);

            Controls.Add(resultLabel);
            Controls.Add(mortalityButton);
            Controls.Add(stockButton);
        }
    }
}
