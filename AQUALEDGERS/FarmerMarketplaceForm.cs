
using System;
using System.Drawing;
using System.Windows.Forms;
using AQUALEDGERS.Services;

namespace AQUALEDGERS
{
    public partial class FarmerMarketplaceForm : Form
    {
        private NumericUpDown numFarmerId;
        private NumericUpDown numPondId;
        private NumericUpDown numHarvestId;
        private TextBox txtFishType;
        private NumericUpDown numQuantity;
        private NumericUpDown numPrice;
        private DataGridView gridListings;
        private Button btnPublish;
        private Button btnRefresh;

        public FarmerMarketplaceForm()
        {
            InitializeComponent();
            BuildInterface();
            LoadListings();
        }

        private void BuildInterface()
        {
            Text = "AquaLedger - Farmer Marketplace";
            Size = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 249, 247);
            Font = new Font("Segoe UI", 10);

            Label heading = new Label
            {
                Text = "Farmer Marketplace",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 91, 72),
                Location = new Point(25, 20),
                AutoSize = true
            };
            Controls.Add(heading);

            Label description = new Label
            {
                Text = "Publish harvested fish for buyers.",
                Location = new Point(28, 65),
                AutoSize = true
            };
            Controls.Add(description);

            GroupBox listingBox = new GroupBox
            {
                Text = "Create Fish Listing",
                Location = new Point(25, 105),
                Size = new Size(1030, 200)
            };
            Controls.Add(listingBox);

            AddLabel(listingBox, "Farmer ID", 20, 35);
            numFarmerId = AddNumber(listingBox, 20, 60, 120);

            AddLabel(listingBox, "Pond ID", 165, 35);
            numPondId = AddNumber(listingBox, 165, 60, 120);

            AddLabel(listingBox, "Harvest ID", 310, 35);
            numHarvestId = AddNumber(listingBox, 310, 60, 120);

            AddLabel(listingBox, "Fish Species", 455, 35);
            txtFishType = new TextBox
            {
                Location = new Point(455, 60),
                Width = 150
            };
            listingBox.Controls.Add(txtFishType);

            AddLabel(listingBox, "Quantity (kg)", 625, 35);
            numQuantity = AddNumber(listingBox, 625, 60, 130);
            numQuantity.DecimalPlaces = 2;
            numQuantity.Increment = 0.5M;
            numQuantity.Maximum = 1000000;

            AddLabel(listingBox, "Price/kg (₱)", 775, 35);
            numPrice = AddNumber(listingBox, 775, 60, 130);
            numPrice.DecimalPlaces = 2;
            numPrice.Maximum = 1000000;

            btnPublish = new Button
            {
                Text = "Publish Listing",
                Location = new Point(455, 115),
                Size = new Size(160, 38),
                BackColor = Color.FromArgb(31, 116, 82),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPublish.Click += BtnPublish_Click;
            listingBox.Controls.Add(btnPublish);

            btnRefresh = new Button
            {
                Text = "Refresh Listings",
                Location = new Point(25, 325),
                Size = new Size(145, 35)
            };
            btnRefresh.Click += delegate { LoadListings(); };
            Controls.Add(btnRefresh);

            gridListings = new DataGridView
            {
                Location = new Point(25, 370),
                Size = new Size(1030, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White
            };
            Controls.Add(gridListings);
        }

        private void AddLabel(
            Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true
            });
        }

        private NumericUpDown AddNumber(
            Control parent, int x, int y, int width)
        {
            NumericUpDown number = new NumericUpDown
            {
                Location = new Point(x, y),
                Width = width,
                Minimum = 0,
                Maximum = 1000000
            };

            parent.Controls.Add(number);
            return number;
        }

        private void BtnPublish_Click(object sender, EventArgs e)
        {
            try
            {
                int farmerId = Decimal.ToInt32(numFarmerId.Value);
                int pondId = Decimal.ToInt32(numPondId.Value);
                int harvestId = Decimal.ToInt32(numHarvestId.Value);

                int listingId = MarketplaceService.CreateListing(
                    farmerId,
                    pondId,
                    harvestId,
                    txtFishType.Text.Trim(),
                    Decimal.ToDouble(numQuantity.Value),
                    Decimal.ToDouble(numPrice.Value)
                );

                MessageBox.Show(
                    "Listing published successfully. Listing ID: "
                    + listingId,
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadListings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Unable to Publish Listing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void LoadListings()
        {
            try
            {
                gridListings.DataSource =
                    MarketplaceService.GetAvailableListings();

                if (gridListings.Columns["AvailableKg"] != null)
                {
                    gridListings.Columns["AvailableKg"].HeaderText =
                        "Available (kg)";
                }

                if (gridListings.Columns["PricePerKg"] != null)
                {
                    gridListings.Columns["PricePerKg"].HeaderText =
                        "Price per kg (₱)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load marketplace listings: " + ex.Message,
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }
    }
}
