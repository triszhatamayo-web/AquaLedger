
using System;
using System.Drawing;
using System.Windows.Forms;
using AQUALEDGERS.Services;

namespace AQUALEDGERS
{
    public partial class AddFarmerForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private Button btnSave;
        private Button btnClear;

        public AddFarmerForm()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "AquaLedger - Add Farmer";
            Size = new Size(650, 480);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            Label title = new Label();
            title.Text = "Register Farmer";
            title.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(30, 100, 75);
            title.Location = new Point(30, 25);
            title.AutoSize = true;
            Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "Enter the farmer's basic information.";
            subtitle.Location = new Point(33, 70);
            subtitle.AutoSize = true;
            Controls.Add(subtitle);

            AddLabel("Full Name:", 35, 130);
            txtFullName = AddTextBox(180, 125, 400);

            AddLabel("Phone Number:", 35, 190);
            txtPhone = AddTextBox(180, 185, 400);

            AddLabel("Address:", 35, 250);
            txtAddress = AddTextBox(180, 245, 400);

            btnSave = new Button();
            btnSave.Text = "Save Farmer";
            btnSave.Location = new Point(180, 325);
            btnSave.Size = new Size(150, 42);
            btnSave.BackColor = Color.FromArgb(30, 120, 85);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            btnClear = new Button();
            btnClear.Text = "Clear";
            btnClear.Location = new Point(345, 325);
            btnClear.Size = new Size(110, 42);
            btnClear.Click += BtnClear_Click;
            Controls.Add(btnClear);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            Controls.Add(label);
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            TextBox textBox = new TextBox();
            textBox.Location = new Point(x, y);
            textBox.Size = new Size(width, 30);
            Controls.Add(textBox);
            return textBox;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show(
                    "Please enter the farmer's full name.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            try
            {
                FarmerService.AddFarmer(fullName, phone, address);

                MessageBox.Show(
                    "Farmer registered successfully!",
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                txtFullName.Clear();
                txtPhone.Clear();
                txtAddress.Clear();
                txtFullName.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to save farmer:\n" + ex.Message,
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtFullName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtFullName.Focus();
        }

        private void AddFarmerForm_Load(object sender, EventArgs e)
        {

        }
    }
}
