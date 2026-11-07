
using System;
using System.Drawing;
using System.Windows.Forms;
using AQUALEDGERS.Services;

namespace AQUALEDGERS
{
    public class MortalityForm : Form
    {
        private readonly int pondId;
        private readonly string pondName;
        private readonly string recordedBy;

        private NumericUpDown numQuantity;
        private DateTimePicker dtpDate;
        private TextBox txtNotes;

        public MortalityForm(
            int pondId,
            string pondName,
            string recordedBy)
        {
            this.pondId = pondId;
            this.pondName = pondName;
            this.recordedBy = recordedBy;

            InitializeForm();
        }

        private void InitializeForm()
        {
            Text = "Record Mortality";
            Size = new Size(500, 430);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            Label titleLabel = new Label();
            titleLabel.Text = "Record Mortality";
            titleLabel.Font = new Font(
                "Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(35, 70, 55);
            titleLabel.Location = new Point(25, 20);
            titleLabel.AutoSize = true;
            Controls.Add(titleLabel);

            Label pondLabel = new Label();
            pondLabel.Text = "Selected Pond: " + pondName;
            pondLabel.Font = new Font("Segoe UI", 10);
            pondLabel.Location = new Point(30, 75);
            pondLabel.AutoSize = true;
            Controls.Add(pondLabel);

            Label quantityLabel = new Label();
            quantityLabel.Text = "Mortality Quantity:";
            quantityLabel.Location = new Point(30, 120);
            quantityLabel.AutoSize = true;
            Controls.Add(quantityLabel);

            numQuantity = new NumericUpDown();
            numQuantity.Minimum = 1;
            numQuantity.Maximum = 1000000;
            numQuantity.Location = new Point(180, 115);
            numQuantity.Width = 220;
            Controls.Add(numQuantity);

            Label dateLabel = new Label();
            dateLabel.Text = "Mortality Date:";
            dateLabel.Location = new Point(30, 165);
            dateLabel.AutoSize = true;
            Controls.Add(dateLabel);

            dtpDate = new DateTimePicker();
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Value = DateTime.Today;
            dtpDate.Location = new Point(180, 160);
            dtpDate.Width = 220;
            Controls.Add(dtpDate);

            Label notesLabel = new Label();
            notesLabel.Text = "Reason / Notes:";
            notesLabel.Location = new Point(30, 210);
            notesLabel.AutoSize = true;
            Controls.Add(notesLabel);

            txtNotes = new TextBox();
            txtNotes.Multiline = true;
            txtNotes.Location = new Point(180, 205);
            txtNotes.Size = new Size(220, 70);
            Controls.Add(txtNotes);

            Button btnSave = new Button();
            btnSave.Text = "SAVE MORTALITY";
            btnSave.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(35, 70, 55);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Size = new Size(220, 40);
            btnSave.Location = new Point(180, 300);
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            Button btnCancel = new Button();
            btnCancel.Text = "CANCEL";
            btnCancel.Size = new Size(220, 35);
            btnCancel.Location = new Point(180, 350);
            btnCancel.Click += (sender, e) =>
            {
                Close();
            };
            Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                MortalityService.RecordMortality(
                    pondId,
                    (int)numQuantity.Value,
                    dtpDate.Value.Date,
                    txtNotes.Text.Trim());

                MessageBox.Show(
                    "Mortality record saved successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Mortality Recording Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}
