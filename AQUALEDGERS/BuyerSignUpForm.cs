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
    public partial class BuyerSignUpForm : Form
    {
        public BuyerSignUpForm()
        {
            InitializeComponent();
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string pin = txtPIN.Text;
            string confirmPin = txtConfirmPIN.Text;

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pin) ||
                string.IsNullOrWhiteSpace(confirmPin))
            {
                MessageBox.Show("Please complete all fields.");
                return;
            }

            if (pin != confirmPin)
            {
                MessageBox.Show(
                    "PIN and Confirm PIN do not match.",
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtConfirmPIN.Clear();
                txtConfirmPIN.Focus();
                return;
            }

            string message;

            bool registered =
                AQUALEDGERS.Services.AuthService.RegisterBuyer(
                    fullName, username, pin, out message);

            MessageBox.Show(
                message,
                "AquaLedger",
                MessageBoxButtons.OK,
                registered ? MessageBoxIcon.Information
                           : MessageBoxIcon.Warning);

            if (registered)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
