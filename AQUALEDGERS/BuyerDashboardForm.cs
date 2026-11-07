
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AQUALEDGERS
{
    public partial class BuyerDashboardForm : Form
    {
        private Button btnMarketplace;
        private Button btnLogout;

        public BuyerDashboardForm()
        {
            InitializeComponent();
            BuildDashboard();
        }

        private void BuildDashboard()
        {
            Text = "AquaLedger - Buyer Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1000, 650);
            BackColor = Color.FromArgb(245, 248, 246);

            Label title = new Label();
            title.Text = "AquaLedger";
            title.Font = new Font(
                "Segoe UI", 24, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(30, 100, 75);
            title.Location = new Point(35, 25);
            title.AutoSize = true;
            Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "Buyer Dashboard";
            subtitle.Font = new Font(
                "Segoe UI", 13, FontStyle.Regular);
            subtitle.Location = new Point(38, 80);
            subtitle.AutoSize = true;
            Controls.Add(subtitle);

            Label welcome = new Label();
            welcome.Text = "Welcome to the AquaLedger Marketplace!";
            welcome.Font = new Font(
                "Segoe UI", 15, FontStyle.Bold);
            welcome.Location = new Point(40, 150);
            welcome.AutoSize = true;
            Controls.Add(welcome);

            Label description = new Label();
            description.Text =
                "Browse available fish listings and place your orders.";
            description.Font = new Font(
                "Segoe UI", 11, FontStyle.Regular);
            description.Location = new Point(42, 195);
            description.AutoSize = true;
            Controls.Add(description);

            btnMarketplace = new Button();
            btnMarketplace.Text = "Open Fish Marketplace";
            btnMarketplace.Font = new Font(
                "Segoe UI", 12, FontStyle.Bold);
            btnMarketplace.Size = new Size(250, 55);
            btnMarketplace.Location = new Point(42, 250);
            btnMarketplace.BackColor =
                Color.FromArgb(30, 120, 85);
            btnMarketplace.ForeColor = Color.White;
            btnMarketplace.FlatStyle = FlatStyle.Flat;
            btnMarketplace.Click += BtnMarketplace_Click;
            Controls.Add(btnMarketplace);

            btnLogout = new Button();
            btnLogout.Text = "Logout";
            btnLogout.Font = new Font(
                "Segoe UI", 10, FontStyle.Regular);
            btnLogout.Size = new Size(120, 40);
            btnLogout.Location = new Point(820, 30);
            btnLogout.BackColor = Color.White;
            btnLogout.ForeColor =
                Color.FromArgb(30, 100, 75);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Click += BtnLogout_Click;
            Controls.Add(btnLogout);
        }

        private void BtnMarketplace_Click(
            object sender, EventArgs e)
        {
            using (BuyerMarketplaceForm marketplace =
                new BuyerMarketplaceForm())
            {
                marketplace.ShowDialog(this);
            }
        }

        private void BtnLogout_Click(
            object sender, EventArgs e)
        {
            Close();
        }
    }
}
