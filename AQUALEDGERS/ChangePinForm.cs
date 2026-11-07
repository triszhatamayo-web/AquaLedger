using AQUALEDGERS.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AQUALEDGERS
{
    public partial class ChangePinForm : Form
    {
        public ChangePinForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSavePin_Click(object sender, EventArgs e)
        {
            string currentPin = txtCurrentPin.Text;
            string newPin = txtNewPin.Text;
            string confirmPin = txtConfirmPin.Text;

            string message;

            bool success = PinService.ChangePin(
                currentPin,
                newPin,
                confirmPin,
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
                    "Change PIN",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }
    }
}
