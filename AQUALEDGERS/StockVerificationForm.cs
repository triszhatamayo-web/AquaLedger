
using System;
using System.Drawing;
using System.Windows.Forms;
using AQUALEDGERS.Services;

namespace AQUALEDGERS
{
    public class StockVerificationForm : Form
    {
        private readonly int pondId;
        private readonly string pondName;
        private readonly string recordedBy;

        private NumericUpDown numVerifiedStock;
        private DateTimePicker dateVerification;
        private TextBox txtNotes;
        private DataGridView dgvHistory;

        public StockVerificationForm(
            int pondId,
            string pondName,
            string recordedBy)
        {
            this.pondId = pondId;
            this.pondName = pondName;
            this.recordedBy = recordedBy;

            BuildForm();
            LoadHistory();
        }

        private void BuildForm()
        {
            Text = "Stock Verification - " + pondName;
            Size = new Size(850, 650);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label lblTitle = new Label
            {
                Text = "Stock Verification",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 70, 55),
                Location = new Point(25, 20),
                AutoSize = true
            };

            Label lblPond = new Label
            {
                Text = "Pond: " + pondName,
                Location = new Point(28, 65),
                AutoSize = true
            };

            Label lblStock = new Label
            {
                Text = "Verified Fish Count",
                Location = new Point(28, 110),
                AutoSize = true
            };

            numVerifiedStock = new NumericUpDown
            {
                Location = new Point(28, 135),
                Width = 200,
                Minimum = 0,
                Maximum = 1000000
            };

            Label lblDate = new Label
            {
                Text = "Verification Date",
                Location = new Point(250, 110),
                AutoSize = true
            };

            dateVerification = new DateTimePicker
            {
                Location = new Point(250, 135),
                Width = 200,
                Format = DateTimePickerFormat.Short
            };

            Label lblNotes = new Label
            {
                Text = "Notes",
                Location = new Point(475, 110),
                AutoSize = true
            };

            txtNotes = new TextBox
            {
                Location = new Point(475, 135),
                Width = 300,
                Height = 60,
                Multiline = true
            };

            Button btnSave = new Button
            {
                Text = "SAVE VERIFICATION",
                Location = new Point(28, 210),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(35, 125, 88),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            Label lblHistory = new Label
            {
                Text = "Verification History",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(28, 275),
                AutoSize = true
            };

            dgvHistory = new DataGridView
            {
                Location = new Point(28, 315),
                Size = new Size(770, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            dgvHistory.Columns.Add("RecordDate", "Date");
            dgvHistory.Columns.Add("PreviousStock", "Previous Stock");
            dgvHistory.Columns.Add("VerifiedStock", "Verified Stock");
            dgvHistory.Columns.Add("Difference", "Difference");
            dgvHistory.Columns.Add("Notes", "Notes");
            dgvHistory.Columns.Add("RecordedBy", "Recorded By");

            Controls.Add(lblTitle);
            Controls.Add(lblPond);
            Controls.Add(lblStock);
            Controls.Add(numVerifiedStock);
            Controls.Add(lblDate);
            Controls.Add(dateVerification);
            Controls.Add(lblNotes);
            Controls.Add(txtNotes);
            Controls.Add(btnSave);
            Controls.Add(lblHistory);
            Controls.Add(dgvHistory);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int verifiedStock =
                    Convert.ToInt32(numVerifiedStock.Value);

                int recordId =
                    StockVerificationService.RecordVerification(
                        pondId,
                        verifiedStock,
                        dateVerification.Value,
                        txtNotes.Text,
                        recordedBy);

                MessageBox.Show(
                    "Stock verification saved successfully!\n\n"
                    + "Record ID: " + recordId,
                    "Stock Verification",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                txtNotes.Clear();

                LoadHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to save stock verification.\n\n"
                    + ex.Message,
                    "Stock Verification Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadHistory()
        {
            dgvHistory.Rows.Clear();

            try
            {
                var records =
                    StockVerificationService
                    .GetVerificationHistory(pondId);

                foreach (var record in records)
                {
                    dgvHistory.Rows.Add(
                        record.RecordDate,
                        record.PreviousStock,
                        record.VerifiedStock,
                        record.Difference,
                        record.Notes,
                        record.RecordedBy);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load verification history.\n\n"
                    + ex.Message,
                    "History Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
