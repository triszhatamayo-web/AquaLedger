
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using AQUALEDGERS.Services;

namespace AQUALEDGERS
{
    public class AddFarmForm : Form
    {
        private ComboBox cmbFarmer;
        private TextBox txtFarmName;
        private TextBox txtAddress;
        private Button btnSave;
        private Button btnCancel;

        public AddFarmForm()
        {
            BuildForm();
            LoadFarmers();
        }

        private void BuildForm()
        {
            Text = "Add Farm";
            Size = new Size(460, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "Add New Farm",
                Font = new Font("Segoe UI", 16,
                    FontStyle.Bold),
                Location = new Point(25, 20),
                Size = new Size(380, 35)
            };

            Label lblFarmer = new Label
            {
                Text = "Select Farmer",
                Location = new Point(25, 75),
                Size = new Size(380, 22)
            };

            cmbFarmer = new ComboBox
            {
                Location = new Point(25, 100),
                Size = new Size(390, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            Label lblFarmName = new Label
            {
                Text = "Farm Name",
                Location = new Point(25, 145),
                Size = new Size(380, 22)
            };

            txtFarmName = new TextBox
            {
                Location = new Point(25, 170),
                Size = new Size(390, 30),
                Font = new Font("Segoe UI", 10)
            };

            Label lblAddress = new Label
            {
                Text = "Farm Address",
                Location = new Point(25, 215),
                Size = new Size(380, 22)
            };

            txtAddress = new TextBox
            {
                Location = new Point(25, 240),
                Size = new Size(390, 30),
                Font = new Font("Segoe UI", 10)
            };

            btnSave = new Button
            {
                Text = "Save Farm",
                Location = new Point(195, 310),
                Size = new Size(105, 38),
                BackColor = Color.FromArgb(35, 125, 88),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(310, 310),
                Size = new Size(105, 38),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(60, 60, 60),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (sender, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblFarmer, cmbFarmer,
                lblFarmName, txtFarmName,
                lblAddress, txtAddress,
                btnSave, btnCancel
            });
        }

        private void LoadFarmers()
        {
            try
            {
                List<FarmerRecord> farmers =
                    FarmerService.GetAllFarmers();

                cmbFarmer.DataSource = farmers;
                cmbFarmer.DisplayMember = "FullName";
                cmbFarmer.ValueMember = "Id";

                if (farmers.Count == 0)
                {
                    btnSave.Enabled = false;

                    MessageBox.Show(
                        "Please add a farmer before adding a farm.",
                        "No Farmers Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load farmers.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                btnSave.Enabled = false;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbFarmer.SelectedValue == null)
            {
                MessageBox.Show("Please select a farmer.");
                return;
            }

            if (!int.TryParse(
                cmbFarmer.SelectedValue.ToString(),
                out int farmerId))
            {
                MessageBox.Show("Please select a valid farmer.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFarmName.Text))
            {
                MessageBox.Show("Please enter a farm name.");
                txtFarmName.Focus();
                return;
            }

            try
            {
                FarmService.AddFarm(
                    farmerId,
                    txtFarmName.Text,
                    txtAddress.Text);

                MessageBox.Show(
                    "Farm added successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to save farm.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
