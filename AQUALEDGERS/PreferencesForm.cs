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
    public partial class PreferencesForm : Form
    {
        public PreferencesForm()
        {
            InitializeComponent();
            LoadPreferences();
        }

        private void LoadPreferences()
        {
            cmbCurrency.Items.Clear();

            cmbCurrency.Items.Add("PHP");
            cmbCurrency.Items.Add("USD");
            cmbCurrency.Items.Add("EUR");

            cmbDateFormat.Items.Clear();

            cmbDateFormat.Items.Add("MM/dd/yyyy");
            cmbDateFormat.Items.Add("dd/MM/yyyy");
            cmbDateFormat.Items.Add("yyyy-MM-dd");

            UserPreferences preferences =
                PreferencesService.GetPreferences();

            cmbCurrency.SelectedItem = preferences.Currency;
            cmbDateFormat.SelectedItem = preferences.DateFormat;
        }
            private void btnSavePreferences_Click(object sender, EventArgs e)
        {
            string currency = cmbCurrency.SelectedItem?.ToString();
            string dateFormat = cmbDateFormat.SelectedItem?.ToString();

            string message;

            bool success = PreferencesService.SavePreferences(
                currency,
                dateFormat,
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
                    "Preferences",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void btnCancelPreferences_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
