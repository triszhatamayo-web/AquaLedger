using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AQUALEDGERS.Services;

namespace AQUALEDGERS
{
    public partial class EditFarmForm : Form
    {
        private int farmId;

        public EditFarmForm(int farmId, string farmName, string address)
        {
            InitializeComponent();

            this.farmId = farmId;

            txtFarmName.Text = farmName;
            txtFarmAddress.Text = address;
        }

        private void btnSaveFarm_Click(object sender, EventArgs e)
        {
            string farmName = txtFarmName.Text;
            string address = txtFarmAddress.Text;

            string message;

            bool success = FarmSettingsService.UpdateFarm(
                farmId,
                farmName,
                address,
                out message
            );

            if (success)
            {
                MessageBox.Show(
                    message,
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.Close();
            }
            else
            {
                MessageBox.Show(
                    message,
                    "Farm Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void btnCancelFarm_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
