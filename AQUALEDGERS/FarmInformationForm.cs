using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AQUALEDGERS.Data;
using Microsoft.Data.Sqlite;

namespace AQUALEDGERS
{
    public partial class FarmInformationForm : Form
    {
        private int selectedFarmId = 0;
        public FarmInformationForm()
        {
            InitializeComponent();
        }
        private void LoadFarms()
        {
            using (var connection = Db.GetConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT
                Farms.Id,
                Farms.FarmName,
                Farmers.FullName,
                Farms.Address
            FROM Farms
            INNER JOIN Farmers
                ON Farms.FarmerId = Farmers.Id
            ORDER BY Farms.FarmName;";

                using (var reader = command.ExecuteReader())
                {
                    var table = new System.Data.DataTable();

                    table.Load(reader);

                    dgvFarms.DataSource = table;
                }
            }

            dgvFarms.Columns["Id"].Visible = false;
            dgvFarms.Columns["FarmName"].HeaderText = "Farm Name";
            dgvFarms.Columns["FullName"].HeaderText = "Farmer Name";
            dgvFarms.Columns["Address"].HeaderText = "Address";
        }

        private void FarmInformationForm_Load(object sender, EventArgs e)
        {
            LoadFarms();
        }

        private void btnSelectFarm_Click(object sender, EventArgs e)
        {
            if (dgvFarms.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Please select a farm first.",
                    "Farm Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            DataGridViewRow selectedRow = dgvFarms.SelectedRows[0];

            selectedFarmId = Convert.ToInt32(selectedRow.Cells["Id"].Value);

            string farmName = selectedRow.Cells["FarmName"].Value.ToString();

            MessageBox.Show(
                "Selected Farm: " + farmName,
                "Farm Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnEditFarm_Click(object sender, EventArgs e)
        {
            if (selectedFarmId == 0)
            {
                MessageBox.Show(
                    "Please select a farm first using the Select Farm button.",
                    "Farm Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            DataGridViewRow selectedRow = dgvFarms.SelectedRows[0];

            string farmName = selectedRow.Cells["FarmName"].Value.ToString();

            string address = selectedRow.Cells["Address"].Value?.ToString() ?? "";

            using (EditFarmForm form = new EditFarmForm(
                selectedFarmId,
                farmName,
                address))
            {
                form.ShowDialog();
            }
        }
    }
}
