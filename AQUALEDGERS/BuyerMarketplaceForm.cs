
using System;
using System.Drawing;
using System.Windows.Forms;
using AQUALEDGERS.Services;

namespace AQUALEDGERS
{
    public partial class BuyerMarketplaceForm : Form
    {
        private DataGridView gridListings;
        private NumericUpDown numQuantity;
        private Button btnOrder;
        private Button btnRefresh;
        private Label lblMessage;

        public BuyerMarketplaceForm()
        {
            InitializeComponent();
            BuildInterface();
            LoadListings();
        }

        private void BuildInterface()
        {
            Text = "AquaLedger - Buyer Marketplace";
            Size = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            Label title = new Label();
            title.Text = "Fish Marketplace";
            title.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            title.Location = new Point(25, 20);
            title.AutoSize = true;
            Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "Browse available fish and place an order.";
            subtitle.Font = new Font("Segoe UI", 10);
            subtitle.Location = new Point(28, 65);
            subtitle.AutoSize = true;
            Controls.Add(subtitle);

            gridListings = new DataGridView();
            gridListings.Location = new Point(25, 110);
            gridListings.Size = new Size(930, 330);
            gridListings.ReadOnly = true;
            gridListings.AllowUserToAddRows = false;
            gridListings.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            gridListings.MultiSelect = false;
            gridListings.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            gridListings.RowHeadersVisible = false;
            Controls.Add(gridListings);

            Label quantityLabel = new Label();
            quantityLabel.Text = "Order Quantity (kg):";
            quantityLabel.Location = new Point(28, 465);
            quantityLabel.AutoSize = true;
            Controls.Add(quantityLabel);

            numQuantity = new NumericUpDown();
            numQuantity.Location = new Point(165, 460);
            numQuantity.Width = 150;
            numQuantity.DecimalPlaces = 2;
            numQuantity.Minimum = 0.01M;
            numQuantity.Maximum = 1000000;
            numQuantity.Value = 1;
            Controls.Add(numQuantity);

            btnOrder = new Button();
            btnOrder.Text = "Place Order";
            btnOrder.Location = new Point(335, 458);
            btnOrder.Size = new Size(130, 35);
            btnOrder.BackColor = Color.FromArgb(30, 120, 85);
            btnOrder.ForeColor = Color.White;
            btnOrder.FlatStyle = FlatStyle.Flat;
            btnOrder.Click += BtnOrder_Click;
            Controls.Add(btnOrder);

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh Listings";
            btnRefresh.Location = new Point(480, 458);
            btnRefresh.Size = new Size(140, 35);
            btnRefresh.Click += (s, e) => LoadListings();
            Controls.Add(btnRefresh);

            lblMessage = new Label();
            lblMessage.Location = new Point(28, 515);
            lblMessage.Size = new Size(900, 45);
            lblMessage.ForeColor = Color.DarkSlateGray;
            lblMessage.AutoSize = false;
            Controls.Add(lblMessage);
        }

        private void LoadListings()
        {
            try
            {
                gridListings.DataSource =
                    MarketplaceService.GetAvailableListings();

                if (gridListings.Columns["Id"] != null)
                    gridListings.Columns["Id"].HeaderText = "Listing ID";

                if (gridListings.Columns["FishType"] != null)
                    gridListings.Columns["FishType"].HeaderText = "Fish Type";

                if (gridListings.Columns["QuantityKg"] != null)
                    gridListings.Columns["QuantityKg"].HeaderText =
                        "Listed Quantity (kg)";

                if (gridListings.Columns["PricePerKg"] != null)
                    gridListings.Columns["PricePerKg"].HeaderText =
                        "Price per kg";

                if (gridListings.Columns["AvailableKg"] != null)
                    gridListings.Columns["AvailableKg"].HeaderText =
                        "Available (kg)";

                if (gridListings.Columns["FarmerId"] != null)
                    gridListings.Columns["FarmerId"].Visible = false;

                if (gridListings.Columns["PondId"] != null)
                    gridListings.Columns["PondId"].Visible = false;

                if (gridListings.Columns["HarvestId"] != null)
                    gridListings.Columns["HarvestId"].Visible = false;

                if (gridListings.Columns["ReservedKg"] != null)
                    gridListings.Columns["ReservedKg"].Visible = false;

                if (gridListings.Columns["SoldKg"] != null)
                    gridListings.Columns["SoldKg"].Visible = false;

                if (gridListings.Columns["Status"] != null)
                    gridListings.Columns["Status"].Visible = false;

                if (gridListings.Columns["CreatedAt"] != null)
                    gridListings.Columns["CreatedAt"].Visible = false;

                lblMessage.Text = gridListings.Rows.Count == 0
                    ? "No fish listings are available yet."
                    : "Select a listing, enter the quantity, and place your order.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load marketplace listings:\n" + ex.Message,
                    "Marketplace Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnOrder_Click(object sender, EventArgs e)
        {
            if (gridListings.CurrentRow == null)
            {
                MessageBox.Show("Please select a fish listing first.");
                return;
            }

            try
            {
                int listingId = Convert.ToInt32(
                    gridListings.CurrentRow.Cells["Id"].Value);

                int buyerId = AuthService.CurrentUserId;

                if (buyerId <= 0)
                {
                    MessageBox.Show(
                        "Your buyer session was not found. Please log in again.");
                    return;
                }

                int orderId = MarketplaceOrderService.PlaceOrder(
                    listingId,
                    buyerId,
                    (double)numQuantity.Value);

                MessageBox.Show(
                    "Order placed successfully!\nOrder ID: " + orderId +
                    "\nYour order is pending confirmation.",
                    "Order Placed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadListings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to place order:\n" + ex.Message,
                    "Order Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}
