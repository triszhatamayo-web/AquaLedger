using AQUALEDGERS.Services;
using AQUALEDGERS.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AQUALEDGERS
{
    // ======================================================================
    //  DASHBOARD FORM
    //  Uses the existing designer controls: pnlSidebar and lblBrand.
    //  Everything else is created in code (BuildDashboard).
    // ======================================================================
    public partial class DashboardForm : Form
    {
        // ---------- Layout containers ----------
        private DataGridView dgvFarmers;
        private TextBox txtSearchFarmer;
        private Button btnAddFarmer;
        private Button btnRefreshFarmers;
        private Panel pnlMain;
        private HeaderBar pnlHeader;
        private Panel pnlDash;      // dashboard view (scrollable)
        private Panel pnlPage;      // placeholder view for the other modules

        // ---------- Placeholder page (other modules) ----------
        private Label lblPageTitle;
        private Label lblPageDescription;
        private Panel pnlContent;
        private Label lblPlaceholder;

        // ---------- Greeting ----------
        private Label lblGreeting;
        private Label lblGreetName;
        private Label lblGreetSub;

        // ---------- Dashboard widgets ----------
        private SummaryCard cardFish, cardFeed, cardSales, cardExpenses;
        private QuickActionsPanel pnlQuick;
        private AlertsPanel pnlAlerts;
        private BannerPanel pnlBanner;
        private OverviewCard pnlOverview;

        // ---------- Sidebar ----------
        private readonly List<SidebarItem> menuItems = new List<SidebarItem>();
        private SidebarItem btnLogout;
        private readonly Dictionary<string, string> pageDescriptions =
            new Dictionary<string, string>();
        private int dividerY;
        private const int MenuItemHeight = 42;

        // ---------- Data (replace with your database values) ----------
        private string userName = "Cooperative Personnel";
        private string cooperativeName = "ABC Fishery Cooperative";
        private int fishStock = 3700;
        private double feedStockKg = 80;
        private decimal totalSales = 25000m;
        private decimal totalExpenses = 12500m;
        private int totalMembers = 24;
        private int totalFarms = 12;
        private int totalFishponds = 18;
        private double lowFeedThresholdKg = 100; 
        
        // alert threshold

        public DashboardForm()
        {
            InitializeComponent();
            BuildDashboard();



        }

        /// <summary>Call this after login to show the real user / cooperative.</summary>
        public void SetUser(string name, string cooperative)
        {
            userName = name;
            cooperativeName = cooperative;
            RefreshDashboard();
        }

        /// <summary>Call this to push real numbers from your database.</summary>
        public void SetDashboardData(int fish, double feedKg, decimal sales,
            decimal expenses, int members, int farms, int ponds)
        {
            fishStock = fish;
            feedStockKg = feedKg;
            totalSales = sales;
            totalExpenses = expenses;
            totalMembers = members;
            totalFarms = farms;
            totalFishponds = ponds;
            RefreshDashboard();
        }

        // ==================================================================
        //  BUILD
        // ==================================================================
        private void BuildDashboard()
        {
            this.Text = "AquaLedger - Dashboard";
            this.BackColor = Theme.PageBg;
            this.MinimumSize = new Size(1100, 700);
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;

            BuildSidebar();
            BuildMainArea();
            BuildPlaceholderPage();
            BuildDashboardView();

            ShowPage("Dashboard", pageDescriptions["Dashboard"]);
        }

            private void ShowFarmersPage()
        {
            foreach (SidebarItem item in menuItems)
                item.Active = item.Text == "Farmers/Members";

            lblPageTitle.Text = "Farmers/Members";
            lblPageDescription.Text =
                "Manage farmer records and contact information.";

            pnlDash.Visible = false;
            pnlPage.Visible = true;
            pnlContent.Controls.Clear();


            // Settings cards layout container
            TableLayoutPanel settingsLayout = new TableLayoutPanel();
            settingsLayout.Dock = DockStyle.Fill;
            settingsLayout.ColumnCount = 2;
            settingsLayout.RowCount = 3;
            settingsLayout.Padding = new Padding(10);
            settingsLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));
            settingsLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));

            settingsLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33.33F));
            settingsLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33.33F));
            settingsLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33.34F));


            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.White
            };

            txtSearchFarmer = new TextBox
            {
                Width = 250,
                Location = new Point(10, 12),
                Font = new Font("Segoe UI", 10)
            };

            txtSearchFarmer.TextChanged += delegate
            {
                LoadFarmers();
            };

            btnRefreshFarmers = new Button
            {
                Text = "Refresh",
                Width = 90,
                Height = 32,
                Location = new Point(270, 9)
            };

            btnRefreshFarmers.Click += delegate
            {
                LoadFarmers();
            };

            btnAddFarmer = new Button
            {
                Text = "+ Add Farmer",
                Width = 120,
                Height = 32,
                Location = new Point(370, 9)
            };

            btnAddFarmer.Click += delegate
            {
                ShowAddFarmerDialog();
            };

            toolbar.Controls.Add(txtSearchFarmer);
            toolbar.Controls.Add(btnRefreshFarmers);
            toolbar.Controls.Add(btnAddFarmer);

            dgvFarmers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 38
            };


            dgvFarmers.Columns.Clear();

            dgvFarmers.Columns.Add("Id", "ID");


            var photoColumn = new DataGridViewImageColumn
            {
                Name = "Photo",
                HeaderText = "Photo",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 65,
                FillWeight = 55,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    NullValue = null
                }
            };

            dgvFarmers.Columns.Add(photoColumn);


            dgvFarmers.Columns.Add("FullName", "Farmer Name");
            dgvFarmers.Columns.Add("Phone", "Phone");
            dgvFarmers.Columns.Add("Address", "Address");


            dgvFarmers.Columns["Id"].DefaultCellStyle.Format = "D3";

            dgvFarmers.Columns["Id"].FillWeight = 40;
            dgvFarmers.Columns["FullName"].FillWeight = 150;
            dgvFarmers.Columns["Phone"].FillWeight = 100;
            dgvFarmers.Columns["Address"].FillWeight = 150;

            // Add the grid first, then the top toolbar.
            pnlContent.Controls.Add(dgvFarmers);
            pnlContent.Controls.Add(toolbar);

            dgvFarmers.CellDoubleClick += DgvFarmers_CellDoubleClick;

            LayoutPage();
            LoadFarmers();
        }

            private void DgvFarmers_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int farmerId = Convert.ToInt32(
                dgvFarmers.Rows[e.RowIndex].Cells["Id"].Value);

            using (FarmerProfileForm profile =
                new FarmerProfileForm(farmerId))
            {
                profile.ShowDialog(this);
            }
        }

        private void LoadFarmers()
        {
            if (dgvFarmers == null || txtSearchFarmer == null)
                return;

            try
            {
                string search = txtSearchFarmer.Text.Trim();

                var farmers =
                    AQUALEDGERS.Services.FarmerService.GetAllFarmers();

                dgvFarmers.Rows.Clear();

                foreach (var farmer in farmers)
                {
                    string name = farmer.FullName ?? "";
                    string phone = farmer.Phone ?? "";
                    string address = farmer.Address ?? "";

                    string searchable = name + " " + phone + " " + address;

                    if (search.Length > 0 &&
                        searchable.IndexOf(
                            search,
                            StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    Image farmerPhoto = null;

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(farmer.PhotoPath) &&
                            File.Exists(farmer.PhotoPath))
                        {
                            using (Image original = Image.FromFile(farmer.PhotoPath))
                            {
                                farmerPhoto = new Bitmap(original);
                            }
                        }
                    }
                    catch
                    {
                        // Keep the photo cell blank if the image cannot be loaded.
                    }

                    int rowIndex = dgvFarmers.Rows.Add(
                        farmer.Id,
                        farmerPhoto,
                        name,
                        phone,
                        address);

                    dgvFarmers.Rows[rowIndex].Height = 60;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load farmer records.\r\n\r\n" + ex.Message,
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ShowAddFarmerDialog()
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Add Farmer";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new Size(400, 270);
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label lblName = new Label
                {
                    Text = "Full Name *",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                TextBox txtName = new TextBox
                {
                    Location = new Point(20, 45),
                    Width = 350
                };

                Label lblPhone = new Label
                {
                    Text = "Phone",
                    Location = new Point(20, 85),
                    AutoSize = true
                };

                TextBox txtPhone = new TextBox
                {
                    Location = new Point(20, 110),
                    Width = 350
                };

                Label lblAddress = new Label
                {
                    Text = "Address",
                    Location = new Point(20, 150),
                    AutoSize = true
                };

                TextBox txtAddress = new TextBox
                {
                    Location = new Point(20, 175),
                    Width = 350
                };

                Button btnSave = new Button
                {
                    Text = "Save Farmer",
                    Location = new Point(170, 220),
                    Width = 100
                };

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(280, 220),
                    Width = 90,
                    DialogResult = DialogResult.Cancel
                };

                btnSave.Click += delegate
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        MessageBox.Show(
                            "Please enter the farmer's full name.",
                            "Missing Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        txtName.Focus();
                        return;
                    }

                    try
                    {
                        AQUALEDGERS.Services.FarmerService.AddFarmer(
                            txtName.Text.Trim(),
                            txtPhone.Text.Trim(),
                            txtAddress.Text.Trim());

                        MessageBox.Show(
                            "Farmer added successfully.",
                            "AquaLedger",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        dialog.DialogResult = DialogResult.OK;
                        dialog.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            "Unable to Add Farmer",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                };

                dialog.Controls.AddRange(new Control[]
                {
            lblName, txtName,
            lblPhone, txtPhone,
            lblAddress, txtAddress,
            btnSave, btnCancel
                });

                dialog.AcceptButton = btnSave;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                    LoadFarmers();
            }
        }

        // ------------------------------------------------------------------
        //  SIDEBAR
        // ------------------------------------------------------------------
        private void BuildSidebar()
        {
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 240;
            pnlSidebar.Controls.Clear();
            lblBrand.Visible = false;   // brand is painted (logo + wordmark)

            // smooth painting for the gradient sidebar
            typeof(Panel).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(pnlSidebar, true, null);
            pnlSidebar.Paint -= pnlSidebar_Paint;
            pnlSidebar.Paint += pnlSidebar_Paint;
            pnlSidebar.Resize += delegate { pnlSidebar.Invalidate(); };

            string[,] items = new string[,]
            {
                // text, glyph, description
                { "Dashboard",       "\U0001F3E0", "Overview of your fish farm operations." },
                { "Farmers/Members", "\U0001F465", "Manage farmer and member records." },
                { "Marketplace",     "\U0001F6D2", "Manage fish listings and marketplace activity." },
                { "Fishponds",       "\U0001F30A", "Manage farms, ponds, species, and fish stock." },
                { "Production",      "\U0001F4C8", "Monitor fish stock, mortality, and growth." },
                { "Feed",            "\U0001F33E", "Manage feed inventory and feeding records." },
                { "Treatment",       "\U0001F48A", "Record pond treatments and health activities." },
                { "Harvests & Sales","\U0001F9FA", "Manage harvest records and fish sales." },
                { "Expenses",        "\U0001F45B", "Track farm expenses and purchases." },
                { "Reports",         "\U0001F4C4", "Review fish farm production and financial reports." },
                { "Settings",        "\u2699",     "Manage application preferences." }
            };

            int y = 96;
            for (int i = 0; i < items.GetLength(0); i++)
            {
                string text = items[i, 0];
                string glyph = items[i, 1];
                string desc = items[i, 2];
                pageDescriptions[text] = desc;

                SidebarItem item = CreateMenuItem(text, glyph, y);
                item.Click += delegate
                {
                    if (text == "Farmers/Members")
                    {
                        ShowFarmersPage();
                    }
                    else
                    {
                        ShowPage(text, desc);
                    }
                };
                y += MenuItemHeight;
            }

            dividerY = y + 8;
            y = dividerY + 10;

            btnLogout = CreateMenuItem("Logout", "\U0001F6AA", y);
            btnLogout.Click += delegate
            {
                DialogResult r = MessageBox.Show(
                    "Are you sure you want to log out?", "Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes) this.Close();
            };
            menuItems.Remove(btnLogout); // logout never stays "active"
        }

        private SidebarItem CreateMenuItem(string text, string glyph, int y)
        {
            SidebarItem item = new SidebarItem();
            item.Text = text;
            item.Glyph = glyph;
            item.Bounds = new Rectangle(0, y, pnlSidebar.Width, MenuItemHeight);
            item.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlSidebar.Controls.Add(item);
            menuItems.Add(item);
            return item;
        }

        // Sidebar background: navy -> teal gradient, waves, logo, wordmark
        private void pnlSidebar_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = pnlSidebar.ClientRectangle;
            if (r.Width < 10 || r.Height < 10) return;

            using (LinearGradientBrush b = new LinearGradientBrush(r,
                Color.FromArgb(10, 58, 106), Color.FromArgb(14, 124, 134), 90f))
            {
                g.FillRectangle(b, r);
            }

            // decorative waves at the bottom
            Rectangle wr = new Rectangle(0, Math.Max(0, r.Height - 190), r.Width, 190);
            Gfx.DrawWave(g, wr, Color.FromArgb(60, 120, 220, 200), 0.3, 12, 0.55, 1.2);
            Gfx.DrawWave(g, wr, Color.FromArgb(70, 40, 170, 150), 1.6, 14, 0.70, 1.0);
            Gfx.DrawWave(g, wr, Color.FromArgb(90, 120, 200, 240), 3.0, 10, 0.85, 1.4);

            // logo + wordmark
            Gfx.DrawLogo(g, new Rectangle(16, 22, 46, 46));
            Font f = new Font("Segoe UI", 16.5f, FontStyle.Bold);
            TextFormatFlags fl = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
            int w1 = TextRenderer.MeasureText("Aqua", f, new Size(500, 100), fl).Width;
            TextRenderer.DrawText(g, "Aqua", f, new Point(68, 31), Color.White, fl);
            TextRenderer.DrawText(g, "Ledger", f, new Point(68 + w1 - 2, 31),
                Color.FromArgb(120, 230, 190), fl);
            f.Dispose();

            // divider above Logout
            using (Pen p = new Pen(Color.FromArgb(70, 255, 255, 255), 1f))
            {
                g.DrawLine(p, 24, dividerY, r.Width - 24, dividerY);
            }
        }

        // ------------------------------------------------------------------
        //  MAIN AREA
        // ------------------------------------------------------------------
        private void BuildMainArea()
        {
            pnlMain = new Panel();
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.BackColor = Theme.PageBg;
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            pnlHeader = new HeaderBar();
            pnlHeader.UserMenu = BuildUserMenu();
            pnlMain.Controls.Add(pnlHeader);

            pnlDash = new Panel();
            pnlDash.BackColor = Theme.PageBg;
            pnlDash.AutoScroll = true;
            pnlMain.Controls.Add(pnlDash);

            pnlPage = new Panel();
            pnlPage.BackColor = Theme.PageBg;
            pnlMain.Controls.Add(pnlPage);

            pnlMain.Resize += delegate { LayoutMain(); };
            pnlDash.Resize += delegate { LayoutDashboard(); };
            pnlPage.Resize += delegate { LayoutPage(); };
        }

        private ContextMenuStrip BuildUserMenu()
        {
            ContextMenuStrip m = new ContextMenuStrip();
            m.Font = new Font("Segoe UI", 10);
            m.Items.Add("My Profile", null, delegate
            {
                MessageBox.Show("My Profile will be connected in a later step.");
            });
            m.Items.Add("Change PIN", null, delegate
            {
                ShowPage("Settings", pageDescriptions["Settings"]);
            });
            m.Items.Add(new ToolStripSeparator());
            m.Items.Add("Logout", null, delegate { btnLogout.PerformClickSafe(); });
            return m;
        }

        private void LayoutMain()
        {
            int w = pnlMain.ClientSize.Width;
            int h = pnlMain.ClientSize.Height;
            int hh = 72;
            pnlHeader.Bounds = new Rectangle(0, 0, w, hh);
            pnlDash.Bounds = new Rectangle(0, hh, w, Math.Max(0, h - hh));
            pnlPage.Bounds = new Rectangle(0, hh, w, Math.Max(0, h - hh));
            LayoutDashboard();
            LayoutPage();
        }

        // ------------------------------------------------------------------
        //  PLACEHOLDER PAGE (all modules other than Dashboard)
        // ------------------------------------------------------------------
        private void BuildPlaceholderPage()
        {
            lblPageTitle = new Label();
            lblPageTitle.Text = "Page";
            lblPageTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblPageTitle.ForeColor = Theme.Navy;
            lblPageTitle.BackColor = Theme.PageBg;
            lblPageTitle.Location = new Point(35, 18);
            lblPageTitle.Size = new Size(650, 45);
            pnlPage.Controls.Add(lblPageTitle);

            lblPageDescription = new Label();
            lblPageDescription.Font = new Font("Segoe UI", 11);
            lblPageDescription.ForeColor = Color.Gray;
            lblPageDescription.BackColor = Theme.PageBg;
            lblPageDescription.Location = new Point(38, 66);
            lblPageDescription.Size = new Size(700, 30);
            pnlPage.Controls.Add(lblPageDescription);

            pnlContent = new Panel();
            pnlContent.BackColor = Color.White;
            pnlContent.BorderStyle = BorderStyle.FixedSingle;
            pnlPage.Controls.Add(pnlContent);

            lblPlaceholder = new Label();
            lblPlaceholder.Font = new Font("Segoe UI", 16);
            lblPlaceholder.ForeColor = Color.FromArgb(80, 95, 110);
            lblPlaceholder.Dock = DockStyle.Fill;
            lblPlaceholder.TextAlign = ContentAlignment.MiddleCenter;
            pnlContent.Controls.Add(lblPlaceholder);
        }

        private void LayoutPage()
        {
            int w = pnlPage.ClientSize.Width;
            int h = pnlPage.ClientSize.Height;
            pnlContent.Bounds = new Rectangle(35, 110, Math.Max(100, w - 70),
                Math.Max(100, h - 140));
        }

        // ------------------------------------------------------------------
        //  DASHBOARD VIEW
        // ------------------------------------------------------------------
        private void BuildDashboardView()
        {
            // greeting
            lblGreeting = MakeLabel(new Font("Segoe UI", 20, FontStyle.Bold), Theme.Navy);
            lblGreetName = MakeLabel(new Font("Segoe UI", 24, FontStyle.Bold), Theme.Green);
            lblGreetSub = MakeLabel(new Font("Segoe UI", 10), Theme.Muted);

            // summary cards
            cardFish = new SummaryCard
            {
                Caption = "Fish Stock",
                UnitText = "fish",
                Glyph = "\U0001F41F",
                IconColor = Color.FromArgb(70, 160, 220),
                FillTop = Color.FromArgb(232, 244, 253),
                FillBottom = Color.FromArgb(214, 235, 250),
                WaveColor = Color.FromArgb(60, 120, 190, 235)
            };
            cardFeed = new SummaryCard
            {
                Caption = "Feed Stock",
                UnitText = "kg",
                Glyph = "\U0001F33E",
                IconColor = Color.FromArgb(76, 175, 100),
                FillTop = Color.FromArgb(236, 247, 238),
                FillBottom = Color.FromArgb(218, 240, 224),
                WaveColor = Color.FromArgb(60, 130, 210, 150)
            };
            cardSales = new SummaryCard
            {
                Caption = "Sales",
                UnitText = "",
                Glyph = "\U0001F4B0",
                IconColor = Color.FromArgb(30, 160, 120),
                FillTop = Color.FromArgb(230, 247, 241),
                FillBottom = Color.FromArgb(210, 240, 229),
                WaveColor = Color.FromArgb(60, 100, 205, 170)
            };
            cardExpenses = new SummaryCard
            {
                Caption = "Expenses",
                UnitText = "",
                Glyph = "\U0001F9FE",
                IconColor = Color.FromArgb(242, 150, 60),
                FillTop = Color.FromArgb(254, 244, 232),
                FillBottom = Color.FromArgb(252, 230, 208),
                WaveColor = Color.FromArgb(60, 250, 190, 130)
            };
            cardFish.Click += delegate { GoTo("Production"); };
            cardFeed.Click += delegate { GoTo("Feed"); };
            cardSales.Click += delegate { GoTo("Harvests & Sales"); };
            cardExpenses.Click += delegate { GoTo("Expenses"); };

            // quick actions
            pnlQuick = new QuickActionsPanel();
            AddQuick("+", "+ Add Pond", Color.FromArgb(27, 108, 181), "Fishponds");
            AddQuick("\U0001F33E", "+ Record Feeding", Color.FromArgb(46, 158, 91), "Feed");
            AddQuick("\U0001F9FE", "+ Record Expense", Color.FromArgb(22, 143, 168), "Expenses");
            AddQuick("\U0001F41F", "+ Record Harvest", Color.FromArgb(107, 84, 184), "Harvests & Sales");
            AddQuick("\U0001FA99", "+ Record Sale", Color.FromArgb(15, 92, 168), "Harvests & Sales");
            pnlQuick.Resize += delegate { LayoutQuick(); };

            // alerts
            pnlAlerts = new AlertsPanel();
            pnlAlerts.AlertClicked += delegate (AlertItem a) { GoTo(a.Target); };
            pnlAlerts.ViewAllClicked += delegate
            {
                ShowPage("Alerts", "All alerts and reminders for your cooperative.");
            };

            // banner + overview
            pnlBanner = new BannerPanel();
            pnlOverview = new OverviewCard();

            pnlDash.Controls.Add(lblGreeting);
            pnlDash.Controls.Add(lblGreetName);
            pnlDash.Controls.Add(lblGreetSub);
            pnlDash.Controls.Add(cardFish);
            pnlDash.Controls.Add(cardFeed);
            pnlDash.Controls.Add(cardSales);
            pnlDash.Controls.Add(cardExpenses);
            pnlDash.Controls.Add(pnlQuick);
            pnlDash.Controls.Add(pnlAlerts);
            pnlDash.Controls.Add(pnlBanner);
            pnlDash.Controls.Add(pnlOverview);

            RefreshDashboard();
        }

        private Label MakeLabel(Font f, Color c)
        {
            Label l = new Label();
            l.Font = f;
            l.ForeColor = c;
            l.BackColor = Theme.PageBg;
            l.AutoSize = true;
            return l;
        }

        private void AddQuick(string glyph, string text, Color color, string target)
        {
            QuickButton b = new QuickButton();
            b.Glyph = glyph;
            b.Text = text;
            b.BaseColor = color;
            b.Click += delegate { GoTo(target); };   // open the matching form here
            pnlQuick.Controls.Add(b);
        }

        private void LayoutQuick()
        {
            List<QuickButton> btns = new List<QuickButton>();
            foreach (Control c in pnlQuick.Controls)
            {
                QuickButton qb = c as QuickButton;
                if (qb != null) btns.Add(qb);
            }
            if (btns.Count == 0) return;

            int padX = 20, gap = 12, top = 46;
            int availW = pnlQuick.Width - padX * 2 - 6;
            int bw = (availW - gap * (btns.Count - 1)) / btns.Count;
            int bh = Math.Max(50, pnlQuick.Height - top - 18);
            for (int i = 0; i < btns.Count; i++)
                btns[i].Bounds = new Rectangle(padX + i * (bw + gap), top, bw, bh);
        }

        /// <summary>Re-reads all values and repaints the dashboard.</summary>
        public void RefreshDashboard()
        {
            if (pnlDash == null) return;

            // greeting by time of day
            int hour = DateTime.Now.Hour;
            lblGreeting.Text = hour < 12 ? "Good morning," :
                               hour < 18 ? "Good afternoon," : "Good evening,";
            lblGreetName.Text = userName + "!";
            lblGreetSub.Text = "Here's an overview of your cooperative's fish farm records.";

            // header
            pnlHeader.DateText = DateTime.Now.ToString("MMMM d, yyyy");
            pnlHeader.DayText = DateTime.Now.DayOfWeek.ToString();
            pnlHeader.WelcomeText = "Welcome, " + userName;
            pnlHeader.CoopText = cooperativeName;
            pnlHeader.Invalidate();

            // cards
            cardFish.ValueText = fishStock.ToString("N0");
            cardFeed.ValueText = feedStockKg.ToString("N0");
            cardSales.ValueText = "\u20B1" + totalSales.ToString("N0");
            cardExpenses.ValueText = "\u20B1" + totalExpenses.ToString("N0");
            cardFish.Invalidate(); cardFeed.Invalidate();
            cardSales.Invalidate(); cardExpenses.Invalidate();

            // overview
            pnlOverview.CoopName = cooperativeName;
            pnlOverview.Members = totalMembers.ToString();
            pnlOverview.Farms = totalFarms.ToString();
            pnlOverview.Ponds = totalFishponds.ToString();
            pnlOverview.Invalidate();

            // alerts
            pnlAlerts.Items = BuildAlerts();
            pnlAlerts.Height = pnlAlerts.PreferredHeight;
            pnlAlerts.Invalidate();

            LayoutDashboard();
        }

        // Automatic alert rules. Connect these to your real data.
        private List<AlertItem> BuildAlerts()
        {
            List<AlertItem> list = new List<AlertItem>();

            if (feedStockKg < lowFeedThresholdKg)
                list.Add(new AlertItem(AlertSeverity.Warning, "Feed stock is low",
                    "Only " + feedStockKg.ToString("N0") + " kg remaining.", "Feed"));

            if (totalExpenses > totalSales)
                list.Add(new AlertItem(AlertSeverity.Warning, "Expenses exceed sales",
                    "Review your expenses for this period.", "Expenses"));

            // TODO: replace with real treatment / water-check schedule from the database
            list.Add(new AlertItem(AlertSeverity.Info, "Treatment follow-up tomorrow",
                "Pond 1 - Check water quality", "Treatment"));

            list.Sort(delegate (AlertItem a, AlertItem b)
            {
                return ((int)b.Severity).CompareTo((int)a.Severity);
            });
            if (list.Count > 3) list.RemoveRange(3, list.Count - 3);
            return list;
        }

        private void LayoutDashboard()
        {
            if (pnlDash == null || lblGreeting == null || pnlOverview == null) return;
            if (pnlDash.ClientSize.Width < 50) return;

            pnlDash.AutoScrollPosition = new Point(0, 0);

            int margin = 30, gap = 18;
            int W = Math.Max(pnlDash.ClientSize.Width, 800);
            int IW = W - margin * 2;

            lblGreeting.Location = new Point(margin, 14);
            lblGreetName.Location = new Point(margin, 46);
            lblGreetSub.Location = new Point(margin + 2, 92);

            // summary cards
            int y = 124;
            int cardH = 104;
            int cw = (IW - gap * 3) / 4;
            SummaryCard[] cards = { cardFish, cardFeed, cardSales, cardExpenses };
            for (int i = 0; i < 4; i++)
                cards[i].Bounds = new Rectangle(margin + i * (cw + gap), y, cw, cardH);
            y += cardH + 16;

            // quick actions (left) + alerts (right)
            int alertsW = Math.Max(270, (int)(IW * 0.27));
            int LW = IW - alertsW - gap;
            pnlQuick.Bounds = new Rectangle(margin, y, LW, 120);
            pnlAlerts.Bounds = new Rectangle(margin + LW + gap, y, alertsW,
                pnlAlerts.PreferredHeight);
            LayoutQuick();

            // banner + overview
            int y2 = y + 120 + 14;
            int rowH = 222;
            int bannerW = (int)(LW * 0.64);
            pnlBanner.Bounds = new Rectangle(margin, y2, bannerW, rowH);
            pnlOverview.Bounds = new Rectangle(margin + bannerW + gap, y2,
                LW - bannerW - gap, rowH);

            pnlDash.AutoScrollMinSize = new Size(800, y2 + rowH + 24);
        }

        // ==================================================================
        //  NAVIGATION
        // ==================================================================
        private void GoTo(string page)
        {
            string desc;
            if (!pageDescriptions.TryGetValue(page, out desc))
                desc = "This module will be connected in a later step.";
            ShowPage(page, desc);
        }

        private void ShowPage(string title, string description)
        {
            foreach (SidebarItem item in menuItems)
                item.Active = (item.Text == title);

            if (title == "Dashboard")
            {
                pnlPage.Visible = false;
                pnlDash.Visible = true;
                RefreshDashboard();
                return;
            }

            if (title == "Farmers/Members")
            {
                ShowFarmersPage();
                return;
            }

            if (title == "Fishponds")
            {
                ShowFarmsPage();
                return;
            }

            if (title == "Production")
            {
                ShowProductionPage();
                return;
            }

            if (title == "Feed")
            {
                ShowFeedPage();
                return;
            }

            if (title == "Treatment")
            {
                ShowTreatmentPage();
                return;
            }

            if (title == "Harvests & Sales")
            {
                ShowHarvestsSalesPage();
                return;
            }

            if (title == "Expenses")
            {
                ShowExpensesPage();
                return;
            }

            if (title == "Reports")
            {
                ShowReportsPage();
                return;
            }
            if (title == "Settings")
            {
                ShowSettingsPage();
                return;
            }

            lblPageTitle.Text = title;
            lblPageDescription.Text = description;

            // Clear the previous module before showing another page.
            pnlContent.Controls.Clear();

            lblPlaceholder.Text = title + "\r\n\r\n" +
                "This module will be connected here in a later step.";
            lblPlaceholder.Visible = true;

            pnlContent.Controls.Add(lblPlaceholder);

            pnlDash.Visible = false;
            pnlPage.Visible = true;

            LayoutPage();
        }



        private void ShowFarmsPage()
        {
            foreach (SidebarItem item in menuItems)
                item.Active = (item.Text == "Fishponds");

            lblPageTitle.Text = "Fishponds";
            lblPageDescription.Text =
                "Manage farms, ponds, species, and fish stock.";

            pnlDash.Visible = false;
            pnlPage.Visible = true;
            pnlContent.Controls.Clear();

            // TOP TOOLBAR
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White
            };

            Label lblFarmer = new Label
            {
                Text = "Farmer",
                Location = new Point(12, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            ComboBox cmbFarmer = new ComboBox
            {
                Name = "cmbFishpondFarmer",
                Location = new Point(12, 32),
                Width = 210,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            Label lblFarm = new Label
            {
                Text = "Farm",
                Location = new Point(235, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            ComboBox cmbFarm = new ComboBox
            {
                Name = "cmbFishpondFarm",
                Location = new Point(235, 32),
                Width = 210,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };

            Button btnAddFarm = new Button
            {
                Text = "+ ADD FARM",
                Location = new Point(460, 30),
                Size = new Size(120, 36),
                BackColor = Color.FromArgb(35, 125, 88),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddFarm.FlatAppearance.BorderSize = 0;

            Button btnAddPond = new Button
            {
                Text = "+ ADD POND",
                Location = new Point(590, 30),
                Size = new Size(120, 36),
                BackColor = Color.FromArgb(35, 125, 88),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddPond.FlatAppearance.BorderSize = 0;

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(720, 30),
                Size = new Size(90, 36)
            };

            toolbar.Controls.Add(lblFarmer);
            toolbar.Controls.Add(cmbFarmer);
            toolbar.Controls.Add(lblFarm);
            toolbar.Controls.Add(cmbFarm);
            toolbar.Controls.Add(btnAddFarm);
            toolbar.Controls.Add(btnAddPond);
            toolbar.Controls.Add(btnRefresh);

            // POND TABLE
            DataGridView dgvPonds = new DataGridView
            {
                Name = "dgvFishponds",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 38
            };

            dgvPonds.Columns.Add("Id", "Pond ID");
            dgvPonds.Columns.Add("PondName", "Pond Name");
            dgvPonds.Columns.Add("Species", "Species");
            dgvPonds.Columns.Add("InitialStock", "Initial Stock");
            dgvPonds.Columns.Add("StockingDate", "Stocking Date");

            dgvPonds.Columns["Id"].FillWeight = 55;
            dgvPonds.Columns["PondName"].FillWeight = 130;
            dgvPonds.Columns["Species"].FillWeight = 100;
            dgvPonds.Columns["InitialStock"].FillWeight = 90;
            dgvPonds.Columns["StockingDate"].FillWeight = 100;

            dgvPonds.Columns["InitialStock"]
                .DefaultCellStyle.Format = "N0";

            // Add the grid before the top toolbar.
            pnlContent.Controls.Add(dgvPonds);
            pnlContent.Controls.Add(toolbar);

            // HELPER: READ THE SELECTED ID
            bool TryGetSelectedId(ComboBox combo, out int id)
            {
                id = 0;

                return combo.SelectedValue != null
                    && int.TryParse(
                        combo.SelectedValue.ToString(), out id)
                    && id > 0;
            }

// LOAD PONDS FOR THE SELECTED FARM
            void LoadPonds()
            {
                dgvPonds.Rows.Clear();

                if (!TryGetSelectedId(cmbFarm, out int farmId))
                    return;

                try
                {
                    var ponds =
                        PondService.GetPondsByFarm(farmId);

                    foreach (var pond in ponds)
                    {
                        dgvPonds.Rows.Add(
                            pond.Id,
                            pond.PondName,
                            pond.Species,
                            pond.InitialStock,
                            pond.StockingDate);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to load ponds.\n\n" + ex.Message,
                        "Fishponds Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // LOAD FARMS FOR THE SELECTED FARMER
            // LOAD FARMS
            void LoadFarms()
            {
                dgvPonds.Rows.Clear();
                cmbFarm.DataSource = null;

                if (!TryGetSelectedId(cmbFarmer, out int farmerId))
                    return;

                try
                {
                    var farms =
                        FarmService.GetFarmsByFarmer(farmerId);

                    cmbFarm.DisplayMember = "FarmName";
                    cmbFarm.ValueMember = "Id";
                    cmbFarm.DataSource = farms;

                    if (farms.Count == 0)
                        return;

                    LoadPonds();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to load farms.\n\n" + ex.Message,
                        "Fishponds Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // LOAD FARMERS
            void LoadFarmers()
            {
                try
                {
                    cmbFarm.DataSource = null;
                    dgvPonds.Rows.Clear();

                    var farmers = FarmerService.GetAllFarmers();

                    cmbFarmer.DisplayMember = "FullName";
                    cmbFarmer.ValueMember = "Id";
                    cmbFarmer.DataSource = farmers;

                    if (farmers.Count == 0)
                    {
                        MessageBox.Show(
                            "Please add a farmer first, then add a farm.",
                            "No Farmers Found",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to load farmers.\n\n" + ex.Message,
                        "Fishponds Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // SELECTION EVENTS
            cmbFarmer.SelectedIndexChanged += (sender, e) =>
            {
                LoadFarms();
            };

            cmbFarm.SelectedIndexChanged += (sender, e) =>
            {
                LoadPonds();
            };

            // ADD FARM
            btnAddFarm.Click += (sender, e) =>
            {
                using (AddFarmForm form = new AddFarmForm())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                        LoadFarmers();
                }
            };

            // ADD POND
            btnAddPond.Click += (sender, e) =>
            {
                if (!TryGetSelectedId(cmbFarm, out int farmId))
                {
                    MessageBox.Show(
                        "Please select a farmer and a farm first.",
                        "Select Farm",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                using (Form dialog = new Form())
                {
                    dialog.Text = "Add Pond";
                    dialog.Size = new Size(390, 360);
                    dialog.StartPosition =
                        FormStartPosition.CenterParent;
                    dialog.FormBorderStyle =
                        FormBorderStyle.FixedDialog;
                    dialog.MaximizeBox = false;
                    dialog.MinimizeBox = false;

                    Label lblName = new Label
                    {
                        Text = "Pond Name",
                        Location = new Point(20, 18),
                        AutoSize = true
                    };

                    TextBox txtName = new TextBox
                    {
                        Location = new Point(20, 42),
                        Width = 325
                    };

                    Label lblSpecies = new Label
                    {
                        Text = "Fish Species",
                        Location = new Point(20, 82),
                        AutoSize = true
                    };

                    TextBox txtSpecies = new TextBox
                    {
                        Location = new Point(20, 106),
                        Width = 325
                    };

                    Label lblStock = new Label
                    {
                        Text = "Initial Fish Count",
                        Location = new Point(20, 146),
                        AutoSize = true
                    };

                    NumericUpDown numStock = new NumericUpDown
                    {
                        Location = new Point(20, 170),
                        Width = 325,
                        Minimum = 0,
                        Maximum = 100000000
                    };

                    Label lblDate = new Label
                    {
                        Text = "Stocking Date",
                        Location = new Point(20, 210),
                        AutoSize = true
                    };

                    DateTimePicker dtpDate = new DateTimePicker
                    {
                        Location = new Point(20, 234),
                        Width = 325,
                        Format = DateTimePickerFormat.Short,
                        MaxDate = DateTime.Today
                    };

                    Button btnSavePond = new Button
                    {
                        Text = "Save Pond",
                        Location = new Point(140, 275),
                        Size = new Size(100, 32),
                        DialogResult = DialogResult.None
                    };

                    btnSavePond.Click += (saveSender, saveEvent) =>
                    {
                        if (string.IsNullOrWhiteSpace(txtName.Text))
                        {
                            MessageBox.Show("Please enter a pond name.");
                            txtName.Focus();
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(txtSpecies.Text))
                        {
                            MessageBox.Show("Please enter a fish species.");
                            txtSpecies.Focus();
                            return;
                        }

                        try
                        {
                            PondService.AddPond(
                                farmId,
                                txtName.Text,
                                txtSpecies.Text,
                                (int)numStock.Value,
                                dtpDate.Value.ToString("yyyy-MM-dd"));

                            dialog.DialogResult = DialogResult.OK;
                            dialog.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                "Unable to save pond.\n\n" + ex.Message,
                                "Add Pond Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    };

                    dialog.Controls.AddRange(new Control[]
                    {
                lblName, txtName,
                lblSpecies, txtSpecies,
                lblStock, numStock,
                lblDate, dtpDate,
                btnSavePond
                    });

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        LoadPonds();
                }
            };

            // REFRESH
            btnRefresh.Click += (sender, e) =>
            {
                LoadFarmers();
            };

            LayoutPage();
            LoadFarmers();
        }

        private void ShowFeedPage()
        {
            foreach (SidebarItem item in menuItems)
                item.Active = item.Text == "Feed";

            lblPageTitle.Text = "Feed";
            lblPageDescription.Text =
                "Manage feed inventory, purchases, and daily feeding records.";

            pnlDash.Visible = false;
            pnlPage.Visible = true;
            pnlContent.Controls.Clear();

            // ----------------------------------------------------------
            // SUMMARY CARDS
            // ----------------------------------------------------------
            Panel pnlSummary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.White
            };

            Label lblAvailable = new Label
            {
                Text = "AVAILABLE FEED",
                Location = new Point(18, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblAvailableValue = new Label
            {
                Text = "0 kg",
                Location = new Point(18, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Green
            };

            Label lblToday = new Label
            {
                Text = "TODAY'S FEEDING",
                Location = new Point(280, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblTodayValue = new Label
            {
                Text = "0 kg",
                Location = new Point(280, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Navy
            };

            Label lblMonth = new Label
            {
                Text = "THIS MONTH",
                Location = new Point(540, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblMonthValue = new Label
            {
                Text = "0 kg",
                Location = new Point(540, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Navy
            };

            pnlSummary.Controls.AddRange(new Control[]
            {
                lblAvailable, lblAvailableValue,
                lblToday, lblTodayValue,
                lblMonth, lblMonthValue
            });

            // ----------------------------------------------------------
            // ACTION TOOLBAR
            // ----------------------------------------------------------
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.White
            };

            Button btnAddStock = new Button
            {
                Text = "+ ADD FEED STOCK",
                Location = new Point(12, 10),
                Size = new Size(155, 34),
                BackColor = Theme.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAddStock.FlatAppearance.BorderSize = 0;

            Button btnRecordFeeding = new Button
            {
                Text = "RECORD FEEDING",
                Location = new Point(178, 10),
                Size = new Size(150, 34),
                BackColor = Theme.Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnRecordFeeding.FlatAppearance.BorderSize = 0;

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(340, 10),
                Size = new Size(90, 34),
                Font = new Font("Segoe UI", 9)
            };

            toolbar.Controls.AddRange(new Control[]
            {
                btnAddStock, btnRecordFeeding, btnRefresh
            });

            // ----------------------------------------------------------
            // INVENTORY TABLE
            // ----------------------------------------------------------
            DataGridView dgvInventory = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 35
            };

            dgvInventory.Columns.Add("FeedType", "Feed Type");
            dgvInventory.Columns.Add("QuantityKg", "Available Quantity (kg)");

            dgvInventory.Columns["FeedType"].FillWeight = 150;
            dgvInventory.Columns["QuantityKg"].FillWeight = 100;
            dgvInventory.Columns["QuantityKg"].DefaultCellStyle.Format = "N2";

            // ----------------------------------------------------------
            // FEEDING HISTORY TABLE
            // ----------------------------------------------------------
            DataGridView dgvHistory = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 35
            };

            dgvHistory.Columns.Add("PondName", "Pond");
            dgvHistory.Columns.Add("FeedType", "Feed Type");
            dgvHistory.Columns.Add("QuantityKg", "Quantity (kg)");
            dgvHistory.Columns.Add("FeedingDate", "Date");
            dgvHistory.Columns.Add("Notes", "Notes");

            dgvHistory.Columns["QuantityKg"].DefaultCellStyle.Format = "N2";

            // ----------------------------------------------------------
            // TABS
            // ----------------------------------------------------------
            TabControl tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9)
            };

            TabPage tabInventory = new TabPage("Feed Inventory");
            TabPage tabHistory = new TabPage("Feeding History");

            tabInventory.Controls.Add(dgvInventory);
            tabHistory.Controls.Add(dgvHistory);

            tabs.TabPages.Add(tabInventory);
            tabs.TabPages.Add(tabHistory);

            // Add fill control first, then the top panels.
            pnlContent.Controls.Add(tabs);
            pnlContent.Controls.Add(toolbar);
            pnlContent.Controls.Add(pnlSummary);

            // ----------------------------------------------------------
            // LOAD FEED DATA
            // ----------------------------------------------------------
            void LoadFeedData()
            {
                try
                {
                    dgvInventory.Rows.Clear();
                    dgvHistory.Rows.Clear();

                    using (SqliteConnection connection = Db.GetConnection())
                    {
                        // Available feed
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT FeedType, QuantityKg " +
                                "FROM FeedInventory ORDER BY FeedType;";

                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dgvInventory.Rows.Add(
                                        reader["FeedType"].ToString(),
                                        Convert.ToDouble(reader["QuantityKg"]));
                                }
                            }
                        }

                        // Total available feed
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COALESCE(SUM(QuantityKg), 0) " +
                                "FROM FeedInventory;";

                            double available =
                                Convert.ToDouble(command.ExecuteScalar());

                            lblAvailableValue.Text =
                                available.ToString("N2") + " kg";
                        }

                        // Today's feeding
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COALESCE(SUM(QuantityKg), 0) " +
                                "FROM FeedingRecords " +
                                "WHERE FeedingDate = @Date;";

                            command.Parameters.AddWithValue(
                                "@Date", DateTime.Today.ToString("yyyy-MM-dd"));

                            double today =
                                Convert.ToDouble(command.ExecuteScalar());

                            lblTodayValue.Text = today.ToString("N2") + " kg";
                        }

                        // This month's feeding
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COALESCE(SUM(QuantityKg), 0) " +
                                "FROM FeedingRecords " +
                                "WHERE substr(FeedingDate, 1, 7) = @Month;";

                            command.Parameters.AddWithValue(
                                "@Month", DateTime.Today.ToString("yyyy-MM"));

                            double month =
                                Convert.ToDouble(command.ExecuteScalar());

                            lblMonthValue.Text = month.ToString("N2") + " kg";
                        }

                        // Feeding history
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT p.PondName, fr.FeedType, " +
                                "fr.QuantityKg, fr.FeedingDate, fr.Notes " +
                                "FROM FeedingRecords fr " +
                                "INNER JOIN Ponds p ON p.Id = fr.PondId " +
                                "ORDER BY fr.FeedingDate DESC, fr.rowid DESC;";

                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dgvHistory.Rows.Add(
                                        reader["PondName"].ToString(),
                                        reader["FeedType"].ToString(),
                                        Convert.ToDouble(reader["QuantityKg"]),
                                        reader["FeedingDate"].ToString(),
                                        reader["Notes"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to load feed records.\r\n\r\n" + ex.Message,
                        "Feed Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }


            }


            // ----------------------------------------------------------
            // ADD FEED STOCK DIALOG
            // ----------------------------------------------------------
            void OpenAddStockDialog()
            {
                using (Form dialog = new Form())
                {
                    dialog.Text = "Add Feed Stock";
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dialog.ClientSize = new Size(390, 340);
                    dialog.MaximizeBox = false;
                    dialog.MinimizeBox = false;

                    Label lblType = new Label
                    {
                        Text = "Feed Type",
                        Location = new Point(20, 15),
                        AutoSize = true
                    };

                    TextBox txtType = new TextBox
                    {
                        Location = new Point(20, 38),
                        Width = 340
                    };

                    Label lblQuantity = new Label
                    {
                        Text = "Quantity (kg)",
                        Location = new Point(20, 80),
                        AutoSize = true
                    };

                    NumericUpDown numQuantity = new NumericUpDown
                    {
                        Location = new Point(20, 103),
                        Width = 340,
                        Minimum = 0.01m,
                        Maximum = 1000000,
                        DecimalPlaces = 2
                    };

                    Label lblCost = new Label
                    {
                        Text = "Cost per kg (₱)",
                        Location = new Point(20, 145),
                        AutoSize = true
                    };

                    NumericUpDown numCost = new NumericUpDown
                    {
                        Location = new Point(20, 168),
                        Width = 340,
                        Minimum = 0,
                        Maximum = 1000000,
                        DecimalPlaces = 2
                    };

                    Label lblDate = new Label
                    {
                        Text = "Purchase Date",
                        Location = new Point(20, 210),
                        AutoSize = true
                    };

                    DateTimePicker dtpDate = new DateTimePicker
                    {
                        Location = new Point(20, 233),
                        Width = 340,
                        Format = DateTimePickerFormat.Short,
                        MaxDate = DateTime.Today
                    };

                    Button btnSave = new Button
                    {
                        Text = "Save Feed Stock",
                        Location = new Point(125, 280),
                        Size = new Size(125, 35)
                    };

                    btnSave.Click += delegate
                    {
                        if (string.IsNullOrWhiteSpace(txtType.Text))
                        {
                            MessageBox.Show("Please enter a feed type.");
                            txtType.Focus();
                            return;
                        }

                        try
                        {
                            FeedService.AddFeedStock(
                                txtType.Text.Trim(),
                                (double)numQuantity.Value,
                                (double)numCost.Value,
                                dtpDate.Value,
                                "Feed stock purchase");

                            dialog.DialogResult = DialogResult.OK;
                            dialog.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                ex.Message,
                                "Unable to Add Feed Stock",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    };

                    dialog.Controls.AddRange(new Control[]
                    {
                        lblType, txtType,
                        lblQuantity, numQuantity,
                        lblCost, numCost,
                        lblDate, dtpDate,
                        btnSave
                    });

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        LoadFeedData();
                }
            }

            // ----------------------------------------------------------
            // RECORD FEEDING DIALOG
            // ----------------------------------------------------------
            void OpenRecordFeedingDialog()
            {
                using (Form dialog = new Form())
                {
                    dialog.Text = "Record Feeding";
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dialog.ClientSize = new Size(390, 370);
                    dialog.MaximizeBox = false;
                    dialog.MinimizeBox = false;

                    Label lblPond = new Label
                    {
                        Text = "Pond",
                        Location = new Point(20, 15),
                        AutoSize = true
                    };

                    ComboBox cmbPond = new ComboBox
                    {
                        Location = new Point(20, 38),
                        Width = 340,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };

                    Label lblType = new Label
                    {
                        Text = "Feed Type",
                        Location = new Point(20, 80),
                        AutoSize = true
                    };

                    ComboBox cmbType = new ComboBox
                    {
                        Location = new Point(20, 103),
                        Width = 340,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };

                    Label lblQuantity = new Label
                    {
                        Text = "Quantity (kg)",
                        Location = new Point(20, 145),
                        AutoSize = true
                    };

                    NumericUpDown numQuantity = new NumericUpDown
                    {
                        Location = new Point(20, 168),
                        Width = 340,
                        Minimum = 0.01m,
                        Maximum = 1000000,
                        DecimalPlaces = 2
                    };

                    Label lblDate = new Label
                    {
                        Text = "Feeding Date",
                        Location = new Point(20, 210),
                        AutoSize = true
                    };

                    DateTimePicker dtpDate = new DateTimePicker
                    {
                        Location = new Point(20, 233),
                        Width = 340,
                        Format = DateTimePickerFormat.Short,
                        MaxDate = DateTime.Today
                    };

                    Label lblNotes = new Label
                    {
                        Text = "Notes",
                        Location = new Point(20, 270),
                        AutoSize = true
                    };

                    TextBox txtNotes = new TextBox
                    {
                        Location = new Point(20, 293),
                        Width = 340
                    };

                    Button btnSave = new Button
                    {
                        Text = "Save Feeding",
                        Location = new Point(135, 325),
                        Size = new Size(110, 32)
                    };

                    // Load ponds and feed types.
                    try
                    {
                        using (SqliteConnection connection = Db.GetConnection())
                        {
                            using (SqliteCommand command = connection.CreateCommand())
                            {
                                command.CommandText =
                                    "SELECT Id, PondName FROM Ponds ORDER BY PondName;";

                                using (SqliteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        cmbPond.Items.Add(new
                                        {
                                            Id = Convert.ToInt32(reader["Id"]),
                                            Name = reader["PondName"].ToString()
                                        });
                                    }
                                }
                            }

                            using (SqliteCommand command = connection.CreateCommand())
                            {
                                command.CommandText =
                                    "SELECT FeedType FROM FeedInventory " +
                                    "WHERE QuantityKg > 0 ORDER BY FeedType;";

                                using (SqliteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                        cmbType.Items.Add(reader["FeedType"].ToString());
                                }
                            }
                        }

                        cmbPond.DisplayMember = "Name";
                        cmbPond.ValueMember = "Id";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Unable to load ponds and feed types.\r\n\r\n" + ex.Message,
                            "Feed Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    btnSave.Click += delegate
                    {
                        if (cmbPond.SelectedItem == null)
                        {
                            MessageBox.Show("Please select a pond.");
                            return;
                        }

                        if (cmbType.SelectedItem == null)
                        {
                            MessageBox.Show("Please select a feed type.");
                            return;
                        }

                        try
                        {
                            int pondId = (int)cmbPond.SelectedItem.GetType()
                                .GetProperty("Id").GetValue(cmbPond.SelectedItem, null);

                            FeedService.RecordFeeding(
                                pondId,
                                cmbType.SelectedItem.ToString(),
                                (double)numQuantity.Value,
                                dtpDate.Value,
                                txtNotes.Text.Trim());

                            dialog.DialogResult = DialogResult.OK;
                            dialog.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                ex.Message,
                                "Unable to Record Feeding",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    };

                    dialog.Controls.AddRange(new Control[]
                    {
                        lblPond, cmbPond,
                        lblType, cmbType,
                        lblQuantity, numQuantity,
                        lblDate, dtpDate,
                        lblNotes, txtNotes,
                        btnSave
                    });

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        LoadFeedData();
                }

            }


            // ----------------------------------------------------------
            // BUTTON EVENTS
            // ----------------------------------------------------------

            btnAddStock.Click += (s, e) =>
            {
                OpenAddStockDialog();
            };

            btnRecordFeeding.Click += (s, e) =>
            {
                OpenRecordFeedingDialog();
            };

            btnRefresh.Click += (s, e) =>
            {
                LoadFeedData();
            };

            // ----------------------------------------------------------
            // INITIAL PAGE SETUP
            // ----------------------------------------------------------

            LayoutPage();
            LoadFeedData();


        }


        private void ShowProductionPage()
        {
            pnlContent.Controls.Clear();

            Label titleLabel = new Label();
            titleLabel.Text = "Production";
            titleLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(35, 70, 55);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(30, 25);

            Label descriptionLabel = new Label();
            descriptionLabel.Text =
                "Monitor fish stock, mortality, harvest, and growth records.";
            descriptionLabel.Font = new Font("Segoe UI", 10);
            descriptionLabel.ForeColor = Color.Gray;
            descriptionLabel.AutoSize = true;
            descriptionLabel.Location = new Point(32, 70);

            ComboBox farmerCombo = new ComboBox();
            farmerCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            farmerCombo.Font = new Font("Segoe UI", 10);
            farmerCombo.Location = new Point(30, 120);
            farmerCombo.Width = 220;

            ComboBox farmCombo = new ComboBox();
            farmCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            farmCombo.Font = new Font("Segoe UI", 10);
            farmCombo.Location = new Point(270, 120);
            farmCombo.Width = 220;
            farmCombo.Enabled = false;

            ComboBox pondCombo = new ComboBox();
            pondCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            pondCombo.Font = new Font("Segoe UI", 10);
            pondCombo.Location = new Point(510, 120);
            pondCombo.Width = 220;
            pondCombo.Enabled = false;

            farmerCombo.DisplayMember = "FullName";
            farmerCombo.ValueMember = "Id";

            farmCombo.DisplayMember = "FarmName";
            farmCombo.ValueMember = "Id";

            pondCombo.DisplayMember = "PondName";
            pondCombo.ValueMember = "Id";

            farmerCombo.DataSource = FarmerService.GetAllFarmers();

            farmerCombo.SelectedIndexChanged += (sender, e) =>
            {
                farmCombo.DataSource = null;
                pondCombo.DataSource = null;

                farmCombo.Enabled = false;
                pondCombo.Enabled = false;

                if (farmerCombo.SelectedValue == null)
                    return;

                int farmerId;

                if (!int.TryParse(
                    farmerCombo.SelectedValue.ToString(),
                    out farmerId))
                    return;

                var farms = FarmService.GetFarmsByFarmer(farmerId);

                farmCombo.DataSource = farms;
                farmCombo.Enabled = farms.Count > 0;
            };

            farmCombo.SelectedIndexChanged += (sender, e) =>
            {
                pondCombo.DataSource = null;
                pondCombo.Enabled = false;

                if (farmCombo.SelectedValue == null)
                    return;

                int farmId;

                if (!int.TryParse(
                    farmCombo.SelectedValue.ToString(),
                    out farmId))
                    return;

                var ponds = PondService.GetPondsByFarm(farmId);

                pondCombo.DisplayMember = "PondName";
                pondCombo.ValueMember = "Id";
                pondCombo.DataSource = ponds;
                pondCombo.Enabled = ponds.Count > 0;
            };

            pnlContent.Controls.Add(titleLabel);
            pnlContent.Controls.Add(descriptionLabel);
            pnlContent.Controls.Add(farmerCombo);
            pnlContent.Controls.Add(farmCombo);
            pnlContent.Controls.Add(pondCombo);


            Label initialStockLabel = new Label();
            Label currentStockLabel = new Label();
            Label mortalityLabel = new Label();
            Label harvestedLabel = new Label();

            Label[] stockLabels =
            {
        initialStockLabel,
        currentStockLabel,
        mortalityLabel,
        harvestedLabel
    };

            string[] stockTitles =
            {
        "Initial Stock",
        "Current Live Stock",
        "Mortality",
        "Harvested Fish"
    };

            for (int i = 0; i < stockLabels.Length; i++)
            {
                Panel card = new Panel();
                card.Size = new Size(200, 100);
                card.Location = new Point(30 + (i * 215), 180);
                card.BackColor = Color.White;
                card.BorderStyle = BorderStyle.FixedSingle;

                Label cardTitle = new Label();
                cardTitle.Text = stockTitles[i];
                cardTitle.Font = new Font("Segoe UI", 10);
                cardTitle.ForeColor = Color.Gray;
                cardTitle.Location = new Point(15, 15);
                cardTitle.AutoSize = true;

                stockLabels[i].Text = "0";
                stockLabels[i].Font =
                    new Font("Segoe UI", 22, FontStyle.Bold);
                stockLabels[i].ForeColor =
                    Color.FromArgb(35, 70, 55);
                stockLabels[i].Location = new Point(15, 45);
                stockLabels[i].AutoSize = true;

                card.Controls.Add(cardTitle);
                card.Controls.Add(stockLabels[i]);

                pnlContent.Controls.Add(card);
            }

            pondCombo.SelectedIndexChanged += (sender, e) =>
            {
                if (pondCombo.SelectedValue == null)
                    return;

                int pondId;

                if (!int.TryParse(
                    pondCombo.SelectedValue.ToString(),
                    out pondId))
                    return;

                try
                {
                    ProductionSummary summary =
                        ProductionSummaryService.GetSummary(pondId);

                    initialStockLabel.Text =
                        summary.InitialStock.ToString("N0");

                    currentStockLabel.Text =
                        summary.CurrentStock.ToString("N0");

                    mortalityLabel.Text =
                        summary.TotalMortality.ToString("N0");

                    harvestedLabel.Text =
                        summary.TotalHarvested.ToString("N0");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Production Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };
            Button btnStockVerification = new Button();

            btnStockVerification.Text = "Stock Verification";
            btnStockVerification.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnStockVerification.BackColor = Color.FromArgb(35, 70, 55);
            btnStockVerification.ForeColor = Color.White;
            btnStockVerification.FlatStyle = FlatStyle.Flat;
            btnStockVerification.Size = new Size(200, 40);
            btnStockVerification.Location = new Point(30, 350);

            btnStockVerification.Click += (sender, e) =>
            {
                if (pondCombo.SelectedValue == null)
                {
                    MessageBox.Show(
                        "Please select a pond first.",
                        "Stock Verification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                int pondId = Convert.ToInt32(pondCombo.SelectedValue);

                string pondName = pondCombo.Text;

                using (StockVerificationForm form =
                    new StockVerificationForm(
                        pondId,
                        pondName,
                        userName))
                {
                    form.ShowDialog(this);
                }
            };

            pnlContent.Controls.Add(btnStockVerification);


            Button btnRecordMortality = new Button();

            btnRecordMortality.Text = "Record Mortality";
            btnRecordMortality.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            btnRecordMortality.BackColor = Color.FromArgb(35, 70, 55);
            btnRecordMortality.ForeColor = Color.White;
            btnRecordMortality.FlatStyle = FlatStyle.Flat;
            btnRecordMortality.Size = new Size(200, 40);
            btnRecordMortality.Location = new Point(250, 350);

            btnRecordMortality.Click += (sender, e) =>
            {
                if (pondCombo.SelectedValue == null)
                {
                    MessageBox.Show(
                        "Please select a pond first.",
                        "Record Mortality",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                int pondId = Convert.ToInt32(
                    pondCombo.SelectedValue);

                string pondName = pondCombo.Text;

                using (MortalityForm form =
                    new MortalityForm(
                        pondId,
                        pondName,
                        userName))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        pondCombo.SelectedIndexChanged -= null;

                        ProductionSummary summary =
                            ProductionSummaryService.GetSummary(pondId);

                        initialStockLabel.Text =
                            summary.InitialStock.ToString("N0");

                        currentStockLabel.Text =
                            summary.CurrentStock.ToString("N0");

                        mortalityLabel.Text =
                            summary.TotalMortality.ToString("N0");

                        harvestedLabel.Text =
                            summary.TotalHarvested.ToString("N0");
                    }
                }
            };

            pnlContent.Controls.Add(btnRecordMortality);


        }


        private void ShowTreatmentPage()
        {
            foreach (SidebarItem item in menuItems)
                item.Active = item.Text == "Treatment";

            lblPageTitle.Text = "Treatment";
            lblPageDescription.Text =
                "Monitor and manage fishpond treatment records.";

            pnlDash.Visible = false;
            pnlPage.Visible = true;
            pnlContent.Controls.Clear();

            // ----------------------------------------------------------
            // SUMMARY SECTION
            // ----------------------------------------------------------

            Panel pnlSummary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.White
            };

            Label lblTotal = new Label
            {
                Text = "TOTAL TREATMENTS",
                Location = new Point(18, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblTotalValue = new Label
            {
                Text = "0",
                Location = new Point(18, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Green
            };

            Label lblToday = new Label
            {
                Text = "TREATMENTS TODAY",
                Location = new Point(280, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblTodayValue = new Label
            {
                Text = "0",
                Location = new Point(280, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Navy
            };

            Label lblMonth = new Label
            {
                Text = "THIS MONTH",
                Location = new Point(540, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblMonthValue = new Label
            {
                Text = "0",
                Location = new Point(540, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Navy
            };

            pnlSummary.Controls.AddRange(new Control[]
            {
        lblTotal, lblTotalValue,
        lblToday, lblTodayValue,
        lblMonth, lblMonthValue
            });

            // ----------------------------------------------------------
            // ACTION TOOLBAR
            // ----------------------------------------------------------

            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.White
            };

            Button btnRecord = new Button
            {
                Text = "+ RECORD TREATMENT",
                Location = new Point(12, 10),
                Size = new Size(175, 34),
                BackColor = Theme.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnRecord.FlatAppearance.BorderSize = 0;

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(200, 10),
                Size = new Size(90, 34),
                Font = new Font("Segoe UI", 9)
            };

            toolbar.Controls.AddRange(new Control[]
            {
        btnRecord, btnRefresh
            });

            // ----------------------------------------------------------
            // TREATMENT HISTORY TABLE
            // ----------------------------------------------------------

            DataGridView dgvTreatments = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 35
            };

            dgvTreatments.Columns.Add("PondName", "Pond");
            dgvTreatments.Columns.Add("TreatmentName", "Treatment Name");
            dgvTreatments.Columns.Add("Dosage", "Dosage");
            dgvTreatments.Columns.Add("TreatmentDate", "Treatment Date");
            dgvTreatments.Columns.Add("Notes", "Notes");

            // ----------------------------------------------------------
            // ADD CONTROLS TO PAGE
            // ----------------------------------------------------------

            pnlContent.Controls.Add(dgvTreatments);
            pnlContent.Controls.Add(toolbar);
            pnlContent.Controls.Add(pnlSummary);

            // ----------------------------------------------------------
            // LOAD TREATMENT DATA
            // ----------------------------------------------------------

            void LoadTreatmentData()
            {
                try
                {
                    dgvTreatments.Rows.Clear();

                    using (SqliteConnection connection = Db.GetConnection())
                    {
                        // Total treatments
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COUNT(*) FROM TreatmentRecords;";

                            lblTotalValue.Text =
                                Convert.ToInt32(command.ExecuteScalar()).ToString();
                        }

                        // Treatments today
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COUNT(*) FROM TreatmentRecords " +
                                "WHERE TreatmentDate = @Date;";

                            command.Parameters.AddWithValue(
                                "@Date", DateTime.Today.ToString("yyyy-MM-dd"));

                            lblTodayValue.Text =
                                Convert.ToInt32(command.ExecuteScalar()).ToString();
                        }

                        // Treatments this month
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COUNT(*) FROM TreatmentRecords " +
                                "WHERE substr(TreatmentDate, 1, 7) = @Month;";

                            command.Parameters.AddWithValue(
                                "@Month", DateTime.Today.ToString("yyyy-MM"));

                            lblMonthValue.Text =
                                Convert.ToInt32(command.ExecuteScalar()).ToString();
                        }

                        // Treatment history
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT p.PondName, tr.TreatmentName, " +
                                "tr.Dosage, tr.TreatmentDate, tr.Notes " +
                                "FROM TreatmentRecords tr " +
                                "INNER JOIN Ponds p ON p.Id = tr.PondId " +
                                "ORDER BY tr.TreatmentDate DESC, tr.Id DESC;";

                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dgvTreatments.Rows.Add(
                                        reader["PondName"].ToString(),
                                        reader["TreatmentName"].ToString(),
                                        reader["Dosage"] == DBNull.Value
                                            ? ""
                                            : reader["Dosage"].ToString(),
                                        reader["TreatmentDate"].ToString(),
                                        reader["Notes"] == DBNull.Value
                                            ? ""
                                            : reader["Notes"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to load treatment records.\r\n\r\n" + ex.Message,
                        "Treatment Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // ----------------------------------------------------------
            // BUTTON EVENTS
            // ----------------------------------------------------------

            btnRecord.Click += (s, e) =>
            {
                OpenTreatmentDialog(LoadTreatmentData);
            };

            btnRefresh.Click += (s, e) =>
            {
                LoadTreatmentData();
            };

            // ----------------------------------------------------------
            // INITIAL PAGE SETUP
            // ----------------------------------------------------------

            LayoutPage();
            LoadTreatmentData();
        }
        private void OpenTreatmentDialog(Action refreshData)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Record Treatment";
                dialog.Size = new Size(450, 500);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label lblPond = new Label
                {
                    Text = "Pond",
                    Left = 25,
                    Top = 25,
                    Width = 120
                };

                ComboBox cmbPond = new ComboBox
                {
                    Left = 25,
                    Top = 50,
                    Width = 380,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                Label lblTreatment = new Label
                {
                    Text = "Treatment Name",
                    Left = 25,
                    Top = 95,
                    Width = 150
                };

                TextBox txtTreatment = new TextBox
                {
                    Left = 25,
                    Top = 120,
                    Width = 380
                };

                Label lblDosage = new Label
                {
                    Text = "Dosage",
                    Left = 25,
                    Top = 165,
                    Width = 120
                };

                TextBox txtDosage = new TextBox
                {
                    Left = 25,
                    Top = 190,
                    Width = 380
                };

                Label lblDate = new Label
                {
                    Text = "Treatment Date",
                    Left = 25,
                    Top = 235,
                    Width = 150
                };

                DateTimePicker dtpDate = new DateTimePicker
                {
                    Left = 25,
                    Top = 260,
                    Width = 380,
                    Format = DateTimePickerFormat.Short,
                    MaxDate = DateTime.Today,
                    Value = DateTime.Today
                };

                Label lblNotes = new Label
                {
                    Text = "Notes",
                    Left = 25,
                    Top = 305,
                    Width = 120
                };

                TextBox txtNotes = new TextBox
                {
                    Left = 25,
                    Top = 330,
                    Width = 380,
                    Height = 55,
                    Multiline = true
                };

                Button btnSave = new Button
                {
                    Text = "SAVE TREATMENT",
                    Left = 245,
                    Top = 405,
                    Width = 160,
                    Height = 35
                };

                Button btnCancel = new Button
                {
                    Text = "CANCEL",
                    Left = 25,
                    Top = 405,
                    Width = 100,
                    Height = 35,
                    DialogResult = DialogResult.Cancel
                };

                // LOAD PONDS
                using (var connection = AQUALEDGERS.Data.Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT Id, PondName FROM Ponds ORDER BY PondName;";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbPond.Items.Add(new
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                cmbPond.DisplayMember = "Name";
                cmbPond.ValueMember = "Id";

                btnSave.Click += (s, e) =>
                {
                    if (cmbPond.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a pond.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtTreatment.Text))
                    {
                        MessageBox.Show("Please enter the treatment name.");
                        txtTreatment.Focus();
                        return;
                    }

                    try
                    {
                        int pondId = (int)cmbPond.SelectedItem
                            .GetType()
                            .GetProperty("Id")
                            .GetValue(cmbPond.SelectedItem);

                        AQUALEDGERS.Services.TreatmentService.RecordTreatment(
                            pondId,
                            txtTreatment.Text,
                            dtpDate.Value.Date,
                            txtDosage.Text,
                            txtNotes.Text
                        );

                        MessageBox.Show(
                            "Treatment recorded successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        dialog.DialogResult = DialogResult.OK;
                        dialog.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Unable to record treatment:\n" + ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                };

                dialog.Controls.AddRange(new Control[]
                {
            lblPond, cmbPond,
            lblTreatment, txtTreatment,
            lblDosage, txtDosage,
            lblDate, dtpDate,
            lblNotes, txtNotes,
            btnSave, btnCancel
                });

                dialog.AcceptButton = btnSave;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    refreshData();
                }
            }
        }
        private void ShowHarvestsSalesPage()
        {
            foreach (SidebarItem item in menuItems)
                item.Active = (item.Text == "Harvests & Sales");

            lblPageTitle.Text = "Harvests & Sales";
            lblPageDescription.Text =
                "Manage harvested fish, available quantities, and sales records.";

            pnlContent.Controls.Clear();

            pnlDash.Visible = false;
            pnlPage.Visible = true;

            // MAIN CONTAINER
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10),
                AutoScroll = true
            };

            mainLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 100));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 65));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 280));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 280));

            // SUMMARY CARDS
            FlowLayoutPanel summaryPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 5)
            };

            Label lblTotalHarvests = new Label
            {
                Text = "0\r\nTOTAL HARVESTS",
                Width = 220,
                Height = 75,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(5)
            };

            Label lblAvailable = new Label
            {
                Text = "0 kg\r\nAVAILABLE TO SELL",
                Width = 220,
                Height = 75,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(5)
            };

            Label lblTotalSales = new Label
            {
                Text = "₱0.00\r\nTOTAL SALES",
                Width = 220,
                Height = 75,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(5)
            };

            summaryPanel.Controls.Add(lblTotalHarvests);
            summaryPanel.Controls.Add(lblAvailable);
            summaryPanel.Controls.Add(lblTotalSales);

            // ACTION BUTTONS
            FlowLayoutPanel actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                Padding = new Padding(5)
            };

            Button btnRecordHarvest = new Button
            {
                Text = "+ RECORD HARVEST",
                Width = 160,
                Height = 38
            };

            Button btnSellHarvest = new Button
            {
                Text = "SELL HARVEST",
                Width = 140,
                Height = 38
            };

            Button btnRefresh = new Button
            {
                Text = "REFRESH",
                Width = 100,
                Height = 38
            };

            actionPanel.Controls.Add(btnRecordHarvest);
            actionPanel.Controls.Add(btnSellHarvest);
            actionPanel.Controls.Add(btnRefresh);

            // HARVEST HISTORY
            Panel harvestPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            Label lblHarvestHeading = new Label
            {
                Text = "HARVEST HISTORY",
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            DataGridView dgvHarvests = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };

            dgvHarvests.Columns.Add("HarvestId", "Harvest ID");
            dgvHarvests.Columns.Add("Pond", "Pond");
            dgvHarvests.Columns.Add("FishCount", "Fish Count");
            dgvHarvests.Columns.Add("WeightKg", "Weight (kg)");
            dgvHarvests.Columns.Add("SoldKg", "Sold (kg)");
            dgvHarvests.Columns.Add("RemainingKg", "Remaining (kg)");
            dgvHarvests.Columns.Add("HarvestDate", "Harvest Date");

            harvestPanel.Controls.Add(dgvHarvests);
            harvestPanel.Controls.Add(lblHarvestHeading);

            // SALES HISTORY
            Panel salesPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            Label lblSalesHeading = new Label
            {
                Text = "SALES HISTORY",
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            DataGridView dgvSales = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false
            };
            dgvSales.Columns.Add("SaleId", "Sale ID");
            dgvSales.Columns["SaleId"].Visible = false;

            dgvSales.Columns.Add("BuyerName", "Buyer");
            dgvSales.Columns.Add("HarvestId", "Harvest ID");
            dgvSales.Columns.Add("QuantityKg", "Quantity (kg)");
            dgvSales.Columns.Add("PricePerKg", "Price/kg");
            dgvSales.Columns.Add("TotalAmount", "Total Amount");
            dgvSales.Columns.Add("SaleDate", "Sale Date");

            DataGridViewButtonColumn btnEditSale = new DataGridViewButtonColumn
            {
                Name = "EditSale",
                HeaderText = "Actions",
                Text = "EDIT",
                UseColumnTextForButtonValue = true
            };

            dgvSales.Columns.Add(btnEditSale);

            salesPanel.Controls.Add(dgvSales);
            salesPanel.Controls.Add(lblSalesHeading);

            // ADD SECTIONS TO MAIN LAYOUT
            mainLayout.Controls.Add(summaryPanel, 0, 0);
            mainLayout.Controls.Add(actionPanel, 0, 1);
            mainLayout.Controls.Add(harvestPanel, 0, 2);
            mainLayout.Controls.Add(salesPanel, 0, 3);

            pnlContent.Controls.Add(mainLayout);

            // LOAD DATABASE RECORDS
            void LoadHarvestSalesData()
            {
                dgvHarvests.Rows.Clear();
                dgvSales.Rows.Clear();

                using (SqliteConnection connection = Db.GetConnection())
                {
                    // HARVEST SUMMARY
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    SELECT
                        COUNT(*),
                        COALESCE(SUM(WeightKg - SoldKg), 0)
                    FROM Harvests;
                ";

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblTotalHarvests.Text =
                                    Convert.ToInt32(reader.GetInt64(0)) +
                                    "\r\nTOTAL HARVESTS";

                                lblAvailable.Text =
                                    Convert.ToDouble(reader.GetValue(1))
                                    .ToString("N2") +
                                    " kg\r\nAVAILABLE TO SELL";
                            }
                        }
                    }

                    // TOTAL SALES
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    SELECT COALESCE(SUM(TotalAmount), 0)
                    FROM Sales;
                ";

                        lblTotalSales.Text =
                            "₱" + Convert.ToDouble(
                                command.ExecuteScalar()).ToString("N2") +
                            "\r\nTOTAL SALES";
                    }

                    // HARVEST HISTORY
                    // HARVEST HISTORY
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                         SELECT
                         h.Id,
                         p.PondName,
                         h.FishCount,
                         h.WeightKg,
                         h.SoldKg,
                         h.WeightKg - h.SoldKg,
                         h.HarvestDate
                         FROM Harvests h
                         INNER JOIN Ponds p ON h.PondId = p.Id
                         ORDER BY h.HarvestDate DESC, h.Id DESC;
                         ";

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvHarvests.Rows.Add(
                                    reader.GetInt32(0),
                                    reader.GetString(1),
                                    reader.GetInt32(2),
                                    Convert.ToDouble(reader.GetValue(3)).ToString("N2"),
                                    Convert.ToDouble(reader.GetValue(4)).ToString("N2"),
                                    Convert.ToDouble(reader.GetValue(5)).ToString("N2"),
                                    reader.GetString(6)
                                );
                            }
                        }
                    }

                    // SALES HISTORY
                    // SALES HISTORY
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        SELECT
                        s.Id,
                        s.BuyerName,
                        s.HarvestId,
                        s.QuantityKg,
                        s.PricePerKg,
                        s.TotalAmount,
                        s.SaleDate
                        FROM Sales s
                        ORDER BY s.SaleDate DESC, s.Id DESC;
                        ";

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dgvSales.Rows.Add(
                                    reader.GetInt32(0),
                                    reader.IsDBNull(1) ? "Walk-in Buyer" : reader.GetString(1),
                                    reader.GetInt32(2),
                                    Convert.ToDouble(reader.GetValue(3)).ToString("N2"),
                                    "₱" + Convert.ToDouble(reader.GetValue(4)).ToString("N2"),
                                    "₱" + Convert.ToDouble(reader.GetValue(5)).ToString("N2"),
                                    reader.GetString(6),
                                    "EDIT"
                                );
                            }
                        }
                    }

                }

            }

            // BUTTON EVENTS
            btnRefresh.Click += (s, e) =>
            {
                LoadHarvestSalesData();
            };

            btnRecordHarvest.Click += (s, e) =>
            {
                OpenRecordHarvestDialog(LoadHarvestSalesData);
            };

            btnSellHarvest.Click += (s, e) =>
            {
                OpenSellHarvestDialog(LoadHarvestSalesData);
            };

            dgvSales.CellContentClick += (s, e) =>
            {
                if (e.RowIndex < 0)
                    return;

                if (dgvSales.Columns[e.ColumnIndex].Name == "EditSale")
                {
                    int saleId = Convert.ToInt32(
                        dgvSales.Rows[e.RowIndex].Cells["SaleId"].Value
                    );

                    OpenEditSaleDialog(saleId, LoadHarvestSalesData);
                }
            };

            // INITIAL PAGE SETUP
            LayoutPage();
            LoadHarvestSalesData();
        }
        private void OpenRecordHarvestDialog(Action refreshData)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Record Harvest";
                dialog.Size = new Size(450, 500);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label lblPond = new Label
                {
                    Text = "Pond",
                    Left = 25,
                    Top = 25,
                    Width = 120
                };

                ComboBox cmbPond = new ComboBox
                {
                    Left = 25,
                    Top = 50,
                    Width = 380,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                Label lblFishCount = new Label
                {
                    Text = "Fish Count",
                    Left = 25,
                    Top = 95,
                    Width = 150
                };

                TextBox txtFishCount = new TextBox
                {
                    Left = 25,
                    Top = 120,
                    Width = 380
                };

                Label lblWeight = new Label
                {
                    Text = "Harvest Weight (kg)",
                    Left = 25,
                    Top = 165,
                    Width = 180
                };

                TextBox txtWeight = new TextBox
                {
                    Left = 25,
                    Top = 190,
                    Width = 380
                };

                Label lblDate = new Label
                {
                    Text = "Harvest Date",
                    Left = 25,
                    Top = 235,
                    Width = 150
                };

                DateTimePicker dtpDate = new DateTimePicker
                {
                    Left = 25,
                    Top = 260,
                    Width = 380,
                    Format = DateTimePickerFormat.Short,
                    MaxDate = DateTime.Today,
                    Value = DateTime.Today
                };

                Label lblNotes = new Label
                {
                    Text = "Notes",
                    Left = 25,
                    Top = 305,
                    Width = 120
                };

                TextBox txtNotes = new TextBox
                {
                    Left = 25,
                    Top = 330,
                    Width = 380,
                    Height = 55,
                    Multiline = true
                };

                Button btnSave = new Button
                {
                    Text = "SAVE HARVEST",
                    Left = 245,
                    Top = 405,
                    Width = 160,
                    Height = 35
                };

                Button btnCancel = new Button
                {
                    Text = "CANCEL",
                    Left = 25,
                    Top = 405,
                    Width = 100,
                    Height = 35,
                    DialogResult = DialogResult.Cancel
                };

                // LOAD PONDS
                using (var connection = Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT Id, PondName FROM Ponds ORDER BY PondName;";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbPond.Items.Add(new
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                cmbPond.DisplayMember = "Name";
                cmbPond.ValueMember = "Id";

                // SAVE HARVEST
                btnSave.Click += (s, e) =>
                {
                    if (cmbPond.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a pond.");
                        return;
                    }

                    int fishCount;

                    if (!int.TryParse(txtFishCount.Text, out fishCount) ||
                        fishCount <= 0)
                    {
                        MessageBox.Show(
                            "Please enter a valid fish count greater than zero.");
                        txtFishCount.Focus();
                        return;
                    }

                    double weightKg;

                    if (!double.TryParse(txtWeight.Text, out weightKg) ||
                        weightKg <= 0)
                    {
                        MessageBox.Show(
                            "Please enter a valid harvest weight greater than zero.");
                        txtWeight.Focus();
                        return;
                    }

                    try
                    {
                        int pondId = (int)cmbPond.SelectedItem
                            .GetType()
                            .GetProperty("Id")
                            .GetValue(cmbPond.SelectedItem);

                        HarvestService.RecordHarvest(
                            pondId,
                            fishCount,
                            weightKg,
                            dtpDate.Value.Date,
                            txtNotes.Text
                        );

                        MessageBox.Show(
                            "Harvest recorded successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        dialog.DialogResult = DialogResult.OK;
                        dialog.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Unable to record harvest:\n" + ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                };

                dialog.Controls.AddRange(new Control[]
                {
            lblPond,
            cmbPond,
            lblFishCount,
            txtFishCount,
            lblWeight,
            txtWeight,
            lblDate,
            dtpDate,
            lblNotes,
            txtNotes,
            btnSave,
            btnCancel
                });

                dialog.AcceptButton = btnSave;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    refreshData();
                }
            }

        }
        private void OpenSellHarvestDialog(Action refreshData)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Sell Harvest";
                dialog.Size = new Size(450, 560);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label lblHarvest = new Label
                {
                    Text = "Select Harvest",
                    Left = 25,
                    Top = 20,
                    Width = 150
                };

                ComboBox cmbHarvest = new ComboBox
                {
                    Left = 25,
                    Top = 45,
                    Width = 380,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                Label lblAvailable = new Label
                {
                    Text = "Available Quantity: 0.00 kg",
                    Left = 25,
                    Top = 85,
                    Width = 380,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                Label lblBuyer = new Label
                {
                    Text = "Buyer Name",
                    Left = 25,
                    Top = 120,
                    Width = 150
                };

                TextBox txtBuyer = new TextBox
                {
                    Left = 25,
                    Top = 145,
                    Width = 380
                };

                Label lblQuantity = new Label
                {
                    Text = "Quantity Sold (kg)",
                    Left = 25,
                    Top = 190,
                    Width = 150
                };

                TextBox txtQuantity = new TextBox
                {
                    Left = 25,
                    Top = 215,
                    Width = 380
                };

                Label lblPrice = new Label
                {
                    Text = "Price per Kilogram (₱)",
                    Left = 25,
                    Top = 260,
                    Width = 200
                };

                TextBox txtPrice = new TextBox
                {
                    Left = 25,
                    Top = 285,
                    Width = 380
                };

                Label lblTotal = new Label
                {
                    Text = "Total Amount: ₱0.00",
                    Left = 25,
                    Top = 325,
                    Width = 380,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                Label lblDate = new Label
                {
                    Text = "Sale Date",
                    Left = 25,
                    Top = 360,
                    Width = 150
                };

                DateTimePicker dtpDate = new DateTimePicker
                {
                    Left = 25,
                    Top = 385,
                    Width = 380,
                    Format = DateTimePickerFormat.Short,
                    MaxDate = DateTime.Today,
                    Value = DateTime.Today
                };

                Label lblNotes = new Label
                {
                    Text = "Notes",
                    Left = 25,
                    Top = 425,
                    Width = 120
                };

                TextBox txtNotes = new TextBox
                {
                    Left = 25,
                    Top = 450,
                    Width = 380,
                    Height = 35,
                    Multiline = true
                };

                Button btnCancel = new Button
                {
                    Text = "CANCEL",
                    Left = 25,
                    Top = 495,
                    Width = 100,
                    Height = 35,
                    DialogResult = DialogResult.Cancel
                };

                Button btnSave = new Button
                {
                    Text = "SAVE SALE",
                    Left = 245,
                    Top = 495,
                    Width = 160,
                    Height = 35
                };

                // LOAD HARVESTS WITH AVAILABLE QUANTITY
                using (var connection = Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                SELECT
                    h.Id,
                    p.PondName,
                    h.WeightKg - h.SoldKg AS AvailableKg
                FROM Harvests h
                INNER JOIN Ponds p ON h.PondId = p.Id
                WHERE h.WeightKg - h.SoldKg > 0
                ORDER BY h.HarvestDate DESC, h.Id DESC;
            ";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbHarvest.Items.Add(new
                            {
                                Id = reader.GetInt32(0),
                                Name = "Harvest #" +
                                       reader.GetInt32(0) +
                                       " - " + reader.GetString(1) +
                                       " - " +
                                       Convert.ToDouble(
                                           reader.GetValue(2)).ToString("N2") +
                                       " kg available",
                                AvailableKg = Convert.ToDouble(
                                    reader.GetValue(2))
                            });
                        }
                    }
                }

                cmbHarvest.DisplayMember = "Name";
                cmbHarvest.ValueMember = "Id";

                // DISPLAY AVAILABLE QUANTITY
                cmbHarvest.SelectedIndexChanged += (s, e) =>
                {
                    if (cmbHarvest.SelectedItem != null)
                    {
                        double availableKg = Convert.ToDouble(
                            cmbHarvest.SelectedItem
                            .GetType()
                            .GetProperty("AvailableKg")
                            .GetValue(cmbHarvest.SelectedItem));

                        lblAvailable.Text =
                            "Available Quantity: " +
                            availableKg.ToString("N2") + " kg";
                    }
                };

                // CALCULATE TOTAL AMOUNT
                void UpdateTotal()
                {
                    double quantity;
                    double price;

                    if (double.TryParse(txtQuantity.Text, out quantity) &&
                        double.TryParse(txtPrice.Text, out price))
                    {
                        lblTotal.Text =
                            "Total Amount: ₱" +
                            (quantity * price).ToString("N2");
                    }
                    else
                    {
                        lblTotal.Text = "Total Amount: ₱0.00";
                    }
                }

                txtQuantity.TextChanged += (s, e) => UpdateTotal();
                txtPrice.TextChanged += (s, e) => UpdateTotal();

                // SAVE SALE
                btnSave.Click += (s, e) =>
                {
                    if (cmbHarvest.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a harvest.");
                        return;
                    }

                    double quantityKg;

                    if (!double.TryParse(txtQuantity.Text, out quantityKg) ||
                        quantityKg <= 0)
                    {
                        MessageBox.Show(
                            "Please enter a valid quantity greater than zero.");
                        txtQuantity.Focus();
                        return;
                    }

                    double pricePerKg;

                    if (!double.TryParse(txtPrice.Text, out pricePerKg) ||
                        pricePerKg < 0)
                    {
                        MessageBox.Show(
                            "Please enter a valid price per kilogram.");
                        txtPrice.Focus();
                        return;
                    }

                    try
                    {
                        int harvestId = (int)cmbHarvest.SelectedItem
                            .GetType()
                            .GetProperty("Id")
                            .GetValue(cmbHarvest.SelectedItem);

                        SalesService.RecordSale(
                            harvestId,
                            txtBuyer.Text,
                            quantityKg,
                            pricePerKg,
                            dtpDate.Value.Date,
                            txtNotes.Text
                        );

                        MessageBox.Show(
                            "Sale recorded successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        dialog.DialogResult = DialogResult.OK;
                        dialog.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Unable to record sale:\n" + ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                };

                dialog.Controls.AddRange(new Control[]
                {
            lblHarvest,
            cmbHarvest,
            lblAvailable,
            lblBuyer,
            txtBuyer,
            lblQuantity,
            txtQuantity,
            lblPrice,
            txtPrice,
            lblTotal,
            lblDate,
            dtpDate,
            lblNotes,
            txtNotes,
            btnCancel,
            btnSave
                });

                dialog.AcceptButton = btnSave;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    refreshData();
                }
            }
        }
        private void OpenEditSaleDialog(int saleId, Action refreshData)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Edit Sale";
                dialog.Size = new Size(450, 560);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label lblBuyer = new Label
                {
                    Text = "Buyer Name",
                    Left = 25,
                    Top = 25,
                    Width = 150
                };

                TextBox txtBuyer = new TextBox
                {
                    Left = 25,
                    Top = 50,
                    Width = 380
                };

                Label lblQuantity = new Label
                {
                    Text = "Quantity Sold (kg)",
                    Left = 25,
                    Top = 95,
                    Width = 180
                };

                TextBox txtQuantity = new TextBox
                {
                    Left = 25,
                    Top = 120,
                    Width = 380
                };

                Label lblPrice = new Label
                {
                    Text = "Price per Kilogram (₱)",
                    Left = 25,
                    Top = 165,
                    Width = 200
                };

                TextBox txtPrice = new TextBox
                {
                    Left = 25,
                    Top = 190,
                    Width = 380
                };

                Label lblTotal = new Label
                {
                    Text = "Total Amount: ₱0.00",
                    Left = 25,
                    Top = 230,
                    Width = 380,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                Label lblDate = new Label
                {
                    Text = "Sale Date",
                    Left = 25,
                    Top = 270,
                    Width = 150
                };

                DateTimePicker dtpDate = new DateTimePicker
                {
                    Left = 25,
                    Top = 295,
                    Width = 380,
                    Format = DateTimePickerFormat.Short,
                    MaxDate = DateTime.Today,
                    Value = DateTime.Today
                };

                Label lblNotes = new Label
                {
                    Text = "Notes",
                    Left = 25,
                    Top = 335,
                    Width = 120
                };

                TextBox txtNotes = new TextBox
                {
                    Left = 25,
                    Top = 360,
                    Width = 380,
                    Height = 55,
                    Multiline = true
                };

                Button btnCancel = new Button
                {
                    Text = "CANCEL",
                    Left = 25,
                    Top = 440,
                    Width = 100,
                    Height = 35,
                    DialogResult = DialogResult.Cancel
                };

                Button btnSave = new Button
                {
                    Text = "SAVE CHANGES",
                    Left = 225,
                    Top = 440,
                    Width = 180,
                    Height = 35
                };

                int harvestId = 0;
                double oldQuantity = 0;
                double harvestWeight = 0;
                double oldSoldKg = 0;

                // LOAD EXISTING SALE DETAILS
                using (var connection = Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                SELECT
                    s.HarvestId,
                    s.BuyerName,
                    s.QuantityKg,
                    s.PricePerKg,
                    s.SaleDate,
                    s.Notes,
                    h.WeightKg,
                    h.SoldKg
                FROM Sales s
                INNER JOIN Harvests h ON s.HarvestId = h.Id
                WHERE s.Id = $saleId;
            ";

                    command.Parameters.AddWithValue("$saleId", saleId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("Sale record not found.");
                            return;
                        }

                        harvestId = reader.GetInt32(0);
                        txtBuyer.Text = reader.IsDBNull(1)
                            ? ""
                            : reader.GetString(1);

                        oldQuantity = Convert.ToDouble(reader.GetValue(2));
                        txtQuantity.Text = oldQuantity.ToString();

                        txtPrice.Text =
                            Convert.ToDouble(reader.GetValue(3)).ToString();

                        dtpDate.Value = Convert.ToDateTime(reader.GetString(4));

                        txtNotes.Text = reader.IsDBNull(5)
                            ? ""
                            : reader.GetString(5);

                        harvestWeight = Convert.ToDouble(reader.GetValue(6));
                        oldSoldKg = Convert.ToDouble(reader.GetValue(7));
                    }
                }

                // UPDATE TOTAL AMOUNT DISPLAY
                void UpdateTotal()
                {
                    double quantity;
                    double price;

                    if (double.TryParse(txtQuantity.Text, out quantity) &&
                        double.TryParse(txtPrice.Text, out price) &&
                        quantity > 0 && price >= 0)
                    {
                        lblTotal.Text =
                            "Total Amount: ₱" +
                            (quantity * price).ToString("N2");
                    }
                    else
                    {
                        lblTotal.Text = "Total Amount: ₱0.00";
                    }
                }

                txtQuantity.TextChanged += (s, e) => UpdateTotal();
                txtPrice.TextChanged += (s, e) => UpdateTotal();

                UpdateTotal();

                // SAVE EDITED SALE
                btnSave.Click += (s, e) =>
                {
                    double newQuantity;
                    double newPrice;

                    if (!double.TryParse(txtQuantity.Text, out newQuantity) ||
                        newQuantity <= 0)
                    {
                        MessageBox.Show("Enter a valid quantity greater than zero.");
                        txtQuantity.Focus();
                        return;
                    }

                    if (!double.TryParse(txtPrice.Text, out newPrice) ||
                        newPrice < 0)
                    {
                        MessageBox.Show("Enter a valid price of zero or greater.");
                        txtPrice.Focus();
                        return;
                    }

                    double availableBeforeThisSale = harvestWeight -
                        (oldSoldKg - oldQuantity);

                    if (newQuantity > availableBeforeThisSale)
                    {
                        MessageBox.Show(
                            "The edited quantity exceeds the available harvest.\n\n" +
                            "Maximum allowed: " +
                            availableBeforeThisSale.ToString("N2") + " kg",
                            "Invalid Quantity",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    try
                    {
                        using (var connection = Db.GetConnection())
                        using (var transaction = connection.BeginTransaction())
                        {
                            double newTotal = newQuantity * newPrice;

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = @"
                            UPDATE Sales
                            SET BuyerName = $buyer,
                                QuantityKg = $quantity,
                                PricePerKg = $price,
                                TotalAmount = $total,
                                SaleDate = $date,
                                Notes = $notes
                            WHERE Id = $saleId;
                        ";

                                command.Parameters.AddWithValue(
                                    "$buyer", txtBuyer.Text.Trim());

                                command.Parameters.AddWithValue(
                                    "$quantity", newQuantity);

                                command.Parameters.AddWithValue(
                                    "$price", newPrice);

                                command.Parameters.AddWithValue(
                                    "$total", newTotal);

                                command.Parameters.AddWithValue(
                                    "$date", dtpDate.Value.Date.ToString("yyyy-MM-dd"));

                                command.Parameters.AddWithValue(
                                    "$notes", txtNotes.Text.Trim());

                                command.Parameters.AddWithValue(
                                    "$saleId", saleId);

                                command.ExecuteNonQuery();
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = @"
                            UPDATE Harvests
                            SET SoldKg = SoldKg - $oldQuantity + $newQuantity
                            WHERE Id = $harvestId;
                        ";

                                command.Parameters.AddWithValue(
                                    "$oldQuantity", oldQuantity);

                                command.Parameters.AddWithValue(
                                    "$newQuantity", newQuantity);

                                command.Parameters.AddWithValue(
                                    "$harvestId", harvestId);

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }

                        MessageBox.Show(
                            "Sale updated successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        dialog.DialogResult = DialogResult.OK;
                        dialog.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Unable to update sale:\n" + ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                };

                dialog.Controls.AddRange(new Control[]
                {
            lblBuyer,
            txtBuyer,
            lblQuantity,
            txtQuantity,
            lblPrice,
            txtPrice,
            lblTotal,
            lblDate,
            dtpDate,
            lblNotes,
            txtNotes,
            btnCancel,
            btnSave
                });

                dialog.AcceptButton = btnSave;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    refreshData();
                }
            }
        }
        private void ShowExpensesPage()
        {
            foreach (SidebarItem item in menuItems)
                item.Active = item.Text == "Expenses";

            lblPageTitle.Text = "Expenses";
            lblPageDescription.Text =
                "Track and manage farm expenses and financial records.";

            pnlDash.Visible = false;
            pnlPage.Visible = true;
            pnlContent.Controls.Clear();

            // ----------------------------------------------------------
            // SUMMARY CARDS
            // ----------------------------------------------------------
            Panel pnlSummary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.White
            };

            Label lblTotal = new Label
            {
                Text = "TOTAL EXPENSES",
                Location = new Point(18, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblTotalValue = new Label
            {
                Text = "₱0.00",
                Location = new Point(18, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Green
            };

            Label lblToday = new Label
            {
                Text = "EXPENSES TODAY",
                Location = new Point(280, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblTodayValue = new Label
            {
                Text = "₱0.00",
                Location = new Point(280, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Navy
            };

            Label lblMonth = new Label
            {
                Text = "THIS MONTH",
                Location = new Point(540, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            Label lblMonthValue = new Label
            {
                Text = "₱0.00",
                Location = new Point(540, 34),
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Theme.Navy
            };

            pnlSummary.Controls.AddRange(new Control[]
            {
        lblTotal, lblTotalValue,
        lblToday, lblTodayValue,
        lblMonth, lblMonthValue
            });

            // ----------------------------------------------------------
            // ACTION TOOLBAR
            // ----------------------------------------------------------
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.White
            };

            Button btnAddExpense = new Button
            {
                Text = "+ ADD EXPENSE",
                Location = new Point(12, 10),
                Size = new Size(140, 34),
                BackColor = Theme.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAddExpense.FlatAppearance.BorderSize = 0;

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(165, 10),
                Size = new Size(90, 34),
                Font = new Font("Segoe UI", 9)
            };

            toolbar.Controls.AddRange(new Control[]
            {
        btnAddExpense, btnRefresh
            });

            // ----------------------------------------------------------
            // EXPENSE HISTORY TABLE
            // ----------------------------------------------------------
            DataGridView dgvExpenses = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 35
            };

            dgvExpenses.Columns.Add("Category", "Category");
            dgvExpenses.Columns.Add("Description", "Description");
            dgvExpenses.Columns.Add("Amount", "Amount");
            dgvExpenses.Columns.Add("ExpenseDate", "Expense Date");
            dgvExpenses.Columns.Add("Farm", "Farm");
            dgvExpenses.Columns.Add("Pond", "Pond");

            dgvExpenses.Columns["Category"].FillWeight = 90;
            dgvExpenses.Columns["Description"].FillWeight = 150;
            dgvExpenses.Columns["Amount"].FillWeight = 90;
            dgvExpenses.Columns["ExpenseDate"].FillWeight = 100;
            dgvExpenses.Columns["Farm"].FillWeight = 110;
            dgvExpenses.Columns["Pond"].FillWeight = 90;

            // ----------------------------------------------------------
            // ADD CONTROLS
            // ----------------------------------------------------------
            pnlContent.Controls.Add(dgvExpenses);
            pnlContent.Controls.Add(toolbar);
            pnlContent.Controls.Add(pnlSummary);

            // ----------------------------------------------------------
            // LOAD EXPENSE DATA
            // ----------------------------------------------------------
            void LoadExpenseData()
            {
                try
                {
                    dgvExpenses.Rows.Clear();

                    using (SqliteConnection connection = Db.GetConnection())
                    {
                        // Total expenses
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COALESCE(SUM(Amount), 0) FROM Expenses;";

                            double total =
                                Convert.ToDouble(command.ExecuteScalar());

                            lblTotalValue.Text = "₱" + total.ToString("N2");
                        }

                        // Today's expenses
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COALESCE(SUM(Amount), 0) " +
                                "FROM Expenses WHERE ExpenseDate = @Date;";

                            command.Parameters.AddWithValue(
                                "@Date", DateTime.Today.ToString("yyyy-MM-dd"));

                            double today =
                                Convert.ToDouble(command.ExecuteScalar());

                            lblTodayValue.Text = "₱" + today.ToString("N2");
                        }

                        // This month's expenses
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "SELECT COALESCE(SUM(Amount), 0) " +
                                "FROM Expenses " +
                                "WHERE substr(ExpenseDate, 1, 7) = @Month;";

                            command.Parameters.AddWithValue(
                                "@Month", DateTime.Today.ToString("yyyy-MM"));

                            double month =
                                Convert.ToDouble(command.ExecuteScalar());

                            lblMonthValue.Text = "₱" + month.ToString("N2");
                        }

                        // Expense history
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText =
                            "SELECT e.ExpenseCategory, e.Description, " +
                            "e.Amount, e.ExpenseDate, " +
                            "COALESCE(f.FarmName, 'Not specified') AS Farm, " +
                            "COALESCE(p.PondName, 'Not specified') AS Pond " +
                            "FROM Expenses e " +
                            "LEFT JOIN Farms f ON f.Id = e.FarmId " +
                            "LEFT JOIN Ponds p ON p.Id = e.PondId " +
                            "ORDER BY e.ExpenseDate DESC, e.Id DESC;";

                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dgvExpenses.Rows.Add(
                                        reader["ExpenseCategory"].ToString(),
                                        reader["Description"].ToString(),
                                        "₱" + Convert.ToDouble(
                                            reader["Amount"]).ToString("N2"),
                                        reader["ExpenseDate"].ToString(),
                                        reader["Farm"].ToString(),
                                        reader["Pond"].ToString()
                                    );
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to load expense records.\r\n\r\n" + ex.Message,
                        "Expenses Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // ----------------------------------------------------------
            // ADD EXPENSE DIALOG
            // ----------------------------------------------------------
            void OpenAddExpenseDialog()
            {
                using (Form dialog = new Form())
                {
                    dialog.Text = "Add Expense";
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dialog.ClientSize = new Size(410, 510);
                    dialog.MaximizeBox = false;
                    dialog.MinimizeBox = false;

                    Label lblCategory = new Label
                    {
                        Text = "Expense Category",
                        Location = new Point(20, 15),
                        AutoSize = true
                    };

                    ComboBox cmbCategory = new ComboBox
                    {
                        Location = new Point(20, 38),
                        Width = 360,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };

                    cmbCategory.Items.AddRange(new object[]
                    {
                "Labor",
                "Maintenance",
                "Utilities",
                "Transportation",
                "Supplies",
                "Other"
                    });

                    Label lblDescription = new Label
                    {
                        Text = "Description",
                        Location = new Point(20, 80),
                        AutoSize = true
                    };

                    TextBox txtDescription = new TextBox
                    {
                        Location = new Point(20, 103),
                        Width = 360
                    };

                    Label lblAmount = new Label
                    {
                        Text = "Amount (₱)",
                        Location = new Point(20, 145),
                        AutoSize = true
                    };

                    NumericUpDown numAmount = new NumericUpDown
                    {
                        Location = new Point(20, 168),
                        Width = 360,
                        Minimum = 0.01m,
                        Maximum = 100000000,
                        DecimalPlaces = 2,
                        ThousandsSeparator = true
                    };

                    Label lblDate = new Label
                    {
                        Text = "Expense Date",
                        Location = new Point(20, 210),
                        AutoSize = true
                    };

                    DateTimePicker dtpDate = new DateTimePicker
                    {
                        Location = new Point(20, 233),
                        Width = 360,
                        Format = DateTimePickerFormat.Short,
                        MaxDate = DateTime.Today
                    };

                    Label lblFarm = new Label
                    {
                        Text = "Farm (Optional)",
                        Location = new Point(20, 275),
                        AutoSize = true
                    };

                    ComboBox cmbFarm = new ComboBox
                    {
                        Location = new Point(20, 298),
                        Width = 360,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };

                    Label lblPond = new Label
                    {
                        Text = "Pond (Optional)",
                        Location = new Point(20, 340),
                        AutoSize = true
                    };

                    ComboBox cmbPond = new ComboBox
                    {
                        Location = new Point(20, 363),
                        Width = 360,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };

                    Button btnSave = new Button
                    {
                        Text = "Save Expense",
                        Location = new Point(135, 435),
                        Size = new Size(130, 35),
                        BackColor = Theme.Green,
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold)
                    };
                    btnSave.FlatAppearance.BorderSize = 0;

                    // --------------------------------------------------
                    // LOAD FARMS AND PONDS
                    // --------------------------------------------------
                    try
                    {
                        cmbFarm.Items.Add(new
                        {
                            Id = 0,
                            Name = "Not specified"
                        });

                        cmbPond.Items.Add(new
                        {
                            Id = 0,
                            Name = "Not specified",
                            FarmId = 0
                        });

                        using (SqliteConnection connection = Db.GetConnection())
                        {
                            using (SqliteCommand command = connection.CreateCommand())
                            {
                                command.CommandText =
                                    "SELECT Id, FarmName FROM Farms ORDER BY FarmName;";

                                using (SqliteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        cmbFarm.Items.Add(new
                                        {
                                            Id = Convert.ToInt32(reader["Id"]),
                                            Name = reader["FarmName"].ToString()
                                        });
                                    }
                                }
                            }

                            using (SqliteCommand command = connection.CreateCommand())
                            {
                                command.CommandText =
                                    "SELECT p.Id, p.PondName, p.FarmId, f.FarmName " +
                                    "FROM Ponds p " +
                                    "LEFT JOIN Farms f ON f.Id = p.FarmId " +
                                    "ORDER BY p.PondName;";

                                using (SqliteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        cmbPond.Items.Add(new
                                        {
                                            Id = Convert.ToInt32(reader["Id"]),
                                            Name = reader["PondName"].ToString() +
                                                " - " +
                                                (reader["FarmName"] == DBNull.Value
                                                    ? "No farm"
                                                    : reader["FarmName"].ToString()),
                                            FarmId = Convert.ToInt32(reader["FarmId"])
                                        });
                                    }
                                }
                            }
                        }

                        cmbFarm.DisplayMember = "Name";
                        cmbFarm.ValueMember = "Id";
                        cmbPond.DisplayMember = "Name";
                        cmbPond.ValueMember = "Id";

                        cmbFarm.SelectedIndex = 0;
                        cmbPond.SelectedIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Unable to load farm and pond records.\r\n\r\n" + ex.Message,
                            "Expenses Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    // --------------------------------------------------
                    // SAVE EXPENSE
                    // --------------------------------------------------
                    btnSave.Click += delegate
                    {
                        if (cmbCategory.SelectedItem == null)
                        {
                            MessageBox.Show("Please select an expense category.");
                            cmbCategory.Focus();
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(txtDescription.Text))
                        {
                            MessageBox.Show("Please enter an expense description.");
                            txtDescription.Focus();
                            return;
                        }

                        try
                        {
                            int? farmId = null;
                            int? pondId = null;

                            if (cmbFarm.SelectedItem != null)
                            {
                                int selectedFarmId = Convert.ToInt32(
                                    cmbFarm.SelectedItem.GetType()
                                        .GetProperty("Id")
                                        .GetValue(cmbFarm.SelectedItem, null));

                                if (selectedFarmId > 0)
                                    farmId = selectedFarmId;
                            }

                            if (cmbPond.SelectedItem != null)
                            {
                                int selectedPondId = Convert.ToInt32(
                                    cmbPond.SelectedItem.GetType()
                                        .GetProperty("Id")
                                        .GetValue(cmbPond.SelectedItem, null));

                                if (selectedPondId > 0)
                                {
                                    pondId = selectedPondId;

                                    int pondFarmId = Convert.ToInt32(
                                        cmbPond.SelectedItem.GetType()
                                            .GetProperty("FarmId")
                                            .GetValue(cmbPond.SelectedItem, null));

                                    if (pondFarmId > 0)
                                    {
                                        if (farmId.HasValue &&
                                            farmId.Value != pondFarmId)
                                        {
                                            MessageBox.Show(
                                                "The selected pond does not belong to the selected farm.",
                                                "Invalid Farm and Pond",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                                            return;
                                        }

                                        farmId = pondFarmId;
                                    }
                                }
                            }

                            ExpenseService.RecordExpense(
                                cmbCategory.SelectedItem.ToString(),
                                txtDescription.Text.Trim(),
                                (double)numAmount.Value,
                                dtpDate.Value,
                                farmId,
                                pondId);

                            dialog.DialogResult = DialogResult.OK;
                            dialog.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                ex.Message,
                                "Unable to Save Expense",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    };

                    dialog.Controls.AddRange(new Control[]
                    {
                lblCategory, cmbCategory,
                lblDescription, txtDescription,
                lblAmount, numAmount,
                lblDate, dtpDate,
                lblFarm, cmbFarm,
                lblPond, cmbPond,
                btnSave
                    });

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        LoadExpenseData();
                }
            }

            // ----------------------------------------------------------
            // BUTTON EVENTS
            // ----------------------------------------------------------
            btnAddExpense.Click += delegate
            {
                OpenAddExpenseDialog();
            };

            btnRefresh.Click += delegate
            {
                LoadExpenseData();
            };

            // Initial load
            LayoutPage();
            LoadExpenseData();
        }

        private void ShowReportsPage()
        {
            foreach (SidebarItem item in menuItems)
                item.Active = item.Text == "Reports";

            lblPageTitle.Text = "Reports";
            lblPageDescription.Text =
                "View production, feed, financial, and farm summary reports.";

            pnlDash.Visible = false;
            pnlPage.Visible = true;
            pnlContent.Controls.Clear();

            // ----------------------------------------------------------
            // DATE FILTER TOOLBAR
            // ----------------------------------------------------------
            Panel toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 125,
                BackColor = Color.White
            };

            Label lblStartDate = new Label
            {
                Text = "START DATE",
                Location = new Point(15, 7),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            DateTimePicker dtpStartDate = new DateTimePicker
            {
                Location = new Point(15, 28),
                Width = 145,
                Format = DateTimePickerFormat.Short,
                Value = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1),
                MaxDate = DateTime.Today
            };

            Label lblEndDate = new Label
            {
                Text = "END DATE",
                Location = new Point(175, 7),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            DateTimePicker dtpEndDate = new DateTimePicker
            {
                Location = new Point(175, 28),
                Width = 145,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                MaxDate = DateTime.Today
            };

            Button btnGenerate = new Button
            {
                Text = "GENERATE REPORT",
                Location = new Point(340, 24),
                Size = new Size(150, 34),
                BackColor = Theme.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnGenerate.FlatAppearance.BorderSize = 0;

            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(505, 24),
                Size = new Size(90, 34),
                Font = new Font("Segoe UI", 9)
            };

            // FARMER FILTER
            Label lblFarmerFilter = new Label
            {
                Text = "FARMER",
                Location = new Point(15, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            ComboBox cmbFarmerFilter = new ComboBox
            {
                Location = new Point(15, 90),
                Width = 210,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };

            // FARM FILTER
            Label lblFarmFilter = new Label
            {
                Text = "FARM",
                Location = new Point(245, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Theme.Muted
            };

            ComboBox cmbFarmFilter = new ComboBox
            {
                Location = new Point(245, 90),
                Width = 210,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };

            toolbar.Controls.AddRange(new Control[]
             {
              lblStartDate,
              dtpStartDate,
              lblEndDate,
              dtpEndDate,
              btnGenerate,
              btnRefresh,
              lblFarmerFilter,
              cmbFarmerFilter,
              lblFarmFilter,
              cmbFarmFilter
});

            // ----------------------------------------------------------
            // REPORT TYPE SELECTOR
            // ----------------------------------------------------------
            Panel categoryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 8)
            };

            FlowLayoutPanel categoryButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                WrapContents = false
            };

            Button CreateCategoryButton(string text, int width)
            {
                Button button = new Button
                {
                    Text = text,
                    Size = new Size(width, 36),
                    Margin = new Padding(4, 2, 4, 2),
                    BackColor = Color.White,
                    ForeColor = Theme.Navy,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

                button.FlatAppearance.BorderColor = Theme.Green;
                button.FlatAppearance.BorderSize = 1;

                categoryButtons.Controls.Add(button);
                return button;
            }

            Button btnProduction = CreateCategoryButton(
                "PRODUCTION REPORT", 165);

            Button btnFeed = CreateCategoryButton(
                "FEED REPORT", 135);

            Button btnFinancial = CreateCategoryButton(
                "FINANCIAL REPORT", 155);

            Button btnFarmSummary = CreateCategoryButton(
                "FARM SUMMARY", 145);

            categoryPanel.Controls.Add(categoryButtons);

            // ----------------------------------------------------------
            // REPORT CARDS CONTAINER
            // ----------------------------------------------------------
            FlowLayoutPanel pnlReportCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(12),
                WrapContents = true
            };

            // Store each card under its report category.
            Dictionary<string, List<Control>> reportCards =
                new Dictionary<string, List<Control>>();

            void RegisterReportCard(string category, Control card)
            {
                if (!reportCards.ContainsKey(category))
                    reportCards[category] = new List<Control>();

                reportCards[category].Add(card);
            }

            // ----------------------------------------------------------
            // REPORT CARD CREATION
            // ----------------------------------------------------------
            Label AddReportCard(
                string category,
                string title,
                string initialValue,
                int width,
                int height,
                Color valueColor)
            {
                Panel card = new Panel
                {
                    Size = new Size(width, height),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(8)
                };

                Label lblTitle = new Label
                {
                    Text = title.ToUpper(),
                    Location = new Point(14, 15),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    ForeColor = Theme.Muted
                };

                Label lblValue = new Label
                {
                    Text = initialValue,
                    Location = new Point(14, 42),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = valueColor
                };

                card.Controls.Add(lblTitle);
                card.Controls.Add(lblValue);

                pnlReportCards.Controls.Add(card);
                RegisterReportCard(category, card);

                return lblValue;
            }

            // ----------------------------------------------------------
            // PRODUCTION REPORT CARDS
            // ----------------------------------------------------------
            Label lblFishStock = AddReportCard(
                "Production",
                "Current Fish Stock",
                "0 fish",
                195,
                90,
                Theme.Green);

            Label lblHarvestedFish = AddReportCard(
                "Production",
                "Fish Harvested",
                "0 fish",
                195,
                90,
                Theme.Green);

            Label lblHarvestWeight = AddReportCard(
                "Production",
                "Harvest Weight",
                "0.00 kg",
                195,
                90,
                Theme.Navy);

            // ----------------------------------------------------------
            // FEED REPORT CARDS
            // ----------------------------------------------------------
            Label lblFeedPurchased = AddReportCard(
                "Feed",
                "Feed Purchased",
                "0.00 kg",
                195,
                90,
                Theme.Navy);

            Label lblFeedUsed = AddReportCard(
                "Feed",
                "Feed Used",
                "0.00 kg",
                195,
                90,
                Theme.Green);

            // ----------------------------------------------------------
            // FINANCIAL REPORT CARDS
            // ----------------------------------------------------------
            Label lblTotalSales = AddReportCard(
                "Financial",
                "Total Sales",
                "₱0.00",
                195,
                90,
                Theme.Green);

            Label lblTotalExpenses = AddReportCard(
                "Financial",
                "Total Expenses",
                "₱0.00",
                195,
                90,
                Theme.Navy);

            // ----------------------------------------------------------
            // FARM SUMMARY CARDS
            // ----------------------------------------------------------
            Label lblFarmers = AddReportCard(
                "Farm Summary",
                "Total Farmers",
                "0",
                195,
                90,
                Theme.Green);

            Label lblFarms = AddReportCard(
                "Farm Summary",
                "Total Farms",
                "0",
                195,
                90,
                Theme.Navy);

            Label lblPonds = AddReportCard(
                "Farm Summary",
                "Total Ponds",
                "0",
                195,
                90,
                Theme.Navy);

            // ----------------------------------------------------------
            // SHOW SELECTED REPORT
            // ----------------------------------------------------------
            void ShowReportCategory(string selectedCategory)
            {
                foreach (KeyValuePair<string, List<Control>> group
                    in reportCards)
                {
                    bool isSelected =
                        group.Key == selectedCategory;

                    foreach (Control card in group.Value)
                        card.Visible = isSelected;
                }

                Button[] buttons =
                {
            btnProduction,
            btnFeed,
            btnFinancial,
            btnFarmSummary
        };

                string[] categories =
                {
            "Production",
            "Feed",
            "Financial",
            "Farm Summary"
        };

                for (int i = 0; i < buttons.Length; i++)
                {
                    bool selected = categories[i] == selectedCategory;

                    buttons[i].BackColor =
                        selected ? Theme.Green : Color.White;

                    buttons[i].ForeColor =
                        selected ? Color.White : Theme.Navy;
                }

                pnlReportCards.PerformLayout();
            }

            // ----------------------------------------------------------
            // ADD CONTROLS
            // ----------------------------------------------------------
            pnlContent.Controls.Add(pnlReportCards);
            pnlContent.Controls.Add(categoryPanel);
            pnlContent.Controls.Add(toolbar);

            // ----------------------------------------------------------
            // LOAD FARMER FILTER
            // ----------------------------------------------------------
            cmbFarmerFilter.Items.Clear();

            cmbFarmerFilter.Items.Add(new
            {
                Id = 0,
                Name = "All Cooperative"
            });

            // Load farmers from database
            using (var connection = AQUALEDGERS.Data.Db.GetConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT Id, FullName FROM Farmers ORDER BY FullName;";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbFarmerFilter.Items.Add(new
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            cmbFarmerFilter.DisplayMember = "Name";
            cmbFarmerFilter.ValueMember = "Id";
            cmbFarmerFilter.SelectedIndex = 0;


            // ----------------------------------------------------------
            // LOAD FARMS BASED ON SELECTED FARMER
            // ----------------------------------------------------------
            void LoadFarmFilter()
            {
                cmbFarmFilter.Items.Clear();

                cmbFarmFilter.Items.Add(new
                {
                    Id = 0,
                    Name = "All Farms"
                });

                int farmerId = Convert.ToInt32(
                    cmbFarmerFilter.SelectedValue
                );

                using (var connection = AQUALEDGERS.Data.Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    if (farmerId == 0)
                    {
                        command.CommandText = @"
                SELECT Id, FarmName
                FROM Farms
                ORDER BY FarmName;
            ";
                    }
                    else
                    {
                        command.CommandText = @"
                SELECT Id, FarmName
                FROM Farms
                WHERE FarmerId = @FarmerId
                ORDER BY FarmName;
            ";

                        command.Parameters.AddWithValue(
                            "@FarmerId", farmerId
                        );
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbFarmFilter.Items.Add(new
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                cmbFarmFilter.DisplayMember = "Name";
                cmbFarmFilter.ValueMember = "Id";
                cmbFarmFilter.SelectedIndex = 0;
            }

            // Load farms initially
            LoadFarmFilter();

            // Reload farms whenever farmer changes
            cmbFarmerFilter.SelectedIndexChanged += (s, e) =>
            {
                if (cmbFarmerFilter.SelectedIndex >= 0)
                    LoadFarmFilter();
            };

            // ----------------------------------------------------------
            // LOAD REPORT DATA
            // ----------------------------------------------------------
            void LoadReportData()
            {
                try
                {
                    if (dtpStartDate.Value.Date > dtpEndDate.Value.Date)
                    {
                        MessageBox.Show(
                            "Start date cannot be later than end date.",
                            "Invalid Date Range",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    int selectedFarmerId = Convert.ToInt32(
    cmbFarmerFilter.SelectedItem
        .GetType()
        .GetProperty("Id")
        .GetValue(cmbFarmerFilter.SelectedItem));

                    int selectedFarmId = Convert.ToInt32(
                        cmbFarmFilter.SelectedItem
                            .GetType()
                            .GetProperty("Id")
                            .GetValue(cmbFarmFilter.SelectedItem));

                    int? farmerId = selectedFarmerId == 0
                        ? (int?)null
                        : selectedFarmerId;

                    int? farmId = selectedFarmId == 0
                        ? (int?)null
                        : selectedFarmId;

                    ReportSummary report = ReportService.GetSummary(
                        dtpStartDate.Value.Date,
                        dtpEndDate.Value.Date,
                        farmerId,
                        farmId);

                    // Production
                    lblFishStock.Text =
                        report.TotalFishStock.ToString("N0") + " fish";

                    lblHarvestedFish.Text =
                        report.TotalHarvestedFish.ToString("N0") + " fish";

                    lblHarvestWeight.Text =
                        report.TotalHarvestWeightKg.ToString("N2") + " kg";

                    // Feed
                    lblFeedPurchased.Text =
                        report.TotalFeedPurchasedKg.ToString("N2") + " kg";

                    lblFeedUsed.Text =
                        report.TotalFeedUsedKg.ToString("N2") + " kg";

                    // Financial
                    lblTotalSales.Text =
                        "₱" + report.TotalSales.ToString("N2");

                    lblTotalExpenses.Text =
                        "₱" + report.TotalExpenses.ToString("N2");

                    // Farm Summary
                    lblFarmers.Text =
                        report.TotalFarmers.ToString("N0");

                    lblFarms.Text =
                        report.TotalFarms.ToString("N0");

                    lblPonds.Text =
                        report.TotalPonds.ToString("N0");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to generate report.\r\n\r\n" + ex.Message,
                        "Reports Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // ----------------------------------------------------------
            // BUTTON EVENTS
            // ----------------------------------------------------------
            btnProduction.Click += delegate
            {
                ShowReportCategory("Production");
            };

            btnFeed.Click += delegate
            {
                ShowReportCategory("Feed");
            };

            btnFinancial.Click += delegate
            {
                ShowReportCategory("Financial");
            };

            btnFarmSummary.Click += delegate
            {
                ShowReportCategory("Farm Summary");
            };

            btnGenerate.Click += delegate
            {
                LoadReportData();
            };

            btnRefresh.Click += delegate
            {
                dtpStartDate.Value = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1);

                dtpEndDate.Value = DateTime.Today;

                LoadReportData();
            };

            // ----------------------------------------------------------
            // INITIAL REPORT
            // ----------------------------------------------------------
            LayoutPage();

            ShowReportCategory("Farm Summary");
            LoadReportData();

        }

        private void ShowSettingsPage()
        {
            
    // Settings cards layout container
         TableLayoutPanel settingsLayout = new TableLayoutPanel();
         settingsLayout.Dock = DockStyle.None;
         settingsLayout.Size = new Size(700, 500);
         settingsLayout.ColumnCount = 2;
         settingsLayout.RowCount = 3;
         settingsLayout.Padding = new Padding(10);

         settingsLayout.ColumnStyles.Add(
         new ColumnStyle(SizeType.Percent, 50F));
         settingsLayout.ColumnStyles.Add(
         new ColumnStyle(SizeType.Percent, 50F));

         settingsLayout.RowStyles.Add(
         new RowStyle(SizeType.Percent, 33.33F));
         settingsLayout.RowStyles.Add(
         new RowStyle(SizeType.Percent, 33.33F));
         settingsLayout.RowStyles.Add(
         new RowStyle(SizeType.Percent, 33.34F));


            pnlPage.Visible = true;
            pnlDash.Visible = false;

            lblPageTitle.Text = "Settings";
            lblPageDescription.Text =
                "Manage your cooperative account and application preferences.";

            pnlContent.Controls.Clear();

            lblPlaceholder.Visible = false;


            // Account Settings Section
            Panel accountPanel = new Panel();
            accountPanel.BackColor = Color.White;
            accountPanel.BorderStyle = BorderStyle.FixedSingle;
            accountPanel.Padding = new Padding(10);
            accountPanel.Size = new Size(300, 150);
            accountPanel.Location = new Point(0, 0);

            Label lblAccountTitle = new Label();
            lblAccountTitle.Text = "Account";
            lblAccountTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblAccountTitle.Location = new Point(15, 15);
            lblAccountTitle.AutoSize = true;

            Label lblAccountDescription = new Label();
            lblAccountDescription.Text = "Manage your account information.";
            lblAccountDescription.Font = new Font("Segoe UI", 9);
            lblAccountDescription.Location = new Point(15, 50);
            lblAccountDescription.AutoSize = true;

            Button btnProfile = new Button();
            btnProfile.Text = "My Profile";
            btnProfile.Location = new Point(15, 85);
            btnProfile.Size = new Size(120, 35);

            btnProfile.Click += delegate
            {
                UserProfile profile = ProfileService.GetCurrentProfile();

                MessageBox.Show(
                    $"Full Name: {profile.FullName}\n" +
                    $"Username: {profile.Username}\n" +
                    $"Role: {profile.Role}\n" +
                    $"Created At: {profile.CreatedAt}",
                    "My Profile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };

            Button btnChangePin = new Button();
            btnChangePin.Text = "Change PIN";
            btnChangePin.Location = new Point(150, 85);
            btnChangePin.Size = new Size(120, 35);

            btnChangePin.Click += delegate
            {
                using (ChangePinForm form = new ChangePinForm())
                {
                    form.ShowDialog();
                }
            };

            accountPanel.Controls.Add(lblAccountTitle);
            accountPanel.Controls.Add(lblAccountDescription);
            accountPanel.Controls.Add(btnProfile);
            accountPanel.Controls.Add(btnChangePin);

            settingsLayout.Controls.Add(accountPanel, 0, 0);


            // Farm Settings Section
            Panel farmPanel = new Panel();
            farmPanel.BackColor = Color.White;
            farmPanel.BorderStyle = BorderStyle.FixedSingle;
            farmPanel.Padding = new Padding(10);
            farmPanel.Size = new Size(300, 150);
            farmPanel.Location = new Point(320, 0);

            Label lblFarmTitle = new Label();
            lblFarmTitle.Text = "Farm";
            lblFarmTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblFarmTitle.Location = new Point(15, 15);
            lblFarmTitle.AutoSize = true;

            Label lblFarmDescription = new Label();
            lblFarmDescription.Text = "Manage your farm information.";
            lblFarmDescription.Font = new Font("Segoe UI", 9);
            lblFarmDescription.Location = new Point(15, 50);
            lblFarmDescription.AutoSize = true;

            Button btnFarmInformation = new Button();
            btnFarmInformation.Text = "Farm Information";
            btnFarmInformation.Location = new Point(15, 85);
            btnFarmInformation.Size = new Size(150, 35);

            farmPanel.Controls.Add(lblFarmTitle);
            farmPanel.Controls.Add(lblFarmDescription);
            farmPanel.Controls.Add(btnFarmInformation);

            btnFarmInformation.Click += delegate
            {
                using (FarmInformationForm form = new FarmInformationForm())
                {
                    form.ShowDialog();
                }
            };

            settingsLayout.Controls.Add(farmPanel, 1, 0);


            // Preferences Settings Section
            Panel preferencesPanel = new Panel();
            preferencesPanel.BackColor = Color.White;
            preferencesPanel.BorderStyle = BorderStyle.FixedSingle;
            preferencesPanel.Padding = new Padding(10);
            preferencesPanel.Size = new Size(300, 150);
            preferencesPanel.Location = new Point(0, 170);

            Label lblPreferencesTitle = new Label();
            lblPreferencesTitle.Text = "Preferences";
            lblPreferencesTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblPreferencesTitle.Location = new Point(15, 15);
            lblPreferencesTitle.AutoSize = true;

            Label lblPreferencesDescription = new Label();
            lblPreferencesDescription.Text = "Customize application preferences.";
            lblPreferencesDescription.Font = new Font("Segoe UI", 9);
            lblPreferencesDescription.Location = new Point(15, 50);
            lblPreferencesDescription.AutoSize = true;

            Button btnPreferences = new Button();
            btnPreferences.Text = "Currency & Date Format";

            btnPreferences.Click += (sender, e) =>
            {
                using (PreferencesForm form = new PreferencesForm())
                {
                    form.ShowDialog();
                }
            };

            btnPreferences.Location = new Point(15, 85);
            btnPreferences.Size = new Size(200, 35);

            preferencesPanel.Controls.Add(lblPreferencesTitle);
            preferencesPanel.Controls.Add(lblPreferencesDescription);
            preferencesPanel.Controls.Add(btnPreferences);

            settingsLayout.Controls.Add(preferencesPanel, 0, 1);


            // Data Settings Section
            Panel dataPanel = new Panel();
            dataPanel.BackColor = Color.White;
            dataPanel.BorderStyle = BorderStyle.FixedSingle;
            dataPanel.Padding = new Padding(10);
            dataPanel.Size = new Size(300, 150);
            dataPanel.Location = new Point(320, 170);

            Label lblDataTitle = new Label();
            lblDataTitle.Text = "Data";
            lblDataTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblDataTitle.Location = new Point(15, 15);
            lblDataTitle.AutoSize = true;

            Label lblDataDescription = new Label();
            lblDataDescription.Text = "Manage and protect your application data.";
            lblDataDescription.Font = new Font("Segoe UI", 9);
            lblDataDescription.Location = new Point(15, 50);
            lblDataDescription.AutoSize = true;

            Button btnBackupDatabase = new Button();
            btnBackupDatabase.Text = "Backup Database";
            btnBackupDatabase.Location = new Point(15, 85);
            btnBackupDatabase.Size = new Size(150, 35);

            dataPanel.Controls.Add(lblDataTitle);
            dataPanel.Controls.Add(lblDataDescription);
            dataPanel.Controls.Add(btnBackupDatabase);

            settingsLayout.Controls.Add(dataPanel, 1, 1);


            // Support Settings Section
            Panel supportPanel = new Panel();
            supportPanel.BackColor = Color.White;
            supportPanel.BorderStyle = BorderStyle.FixedSingle;
            supportPanel.Padding = new Padding(10);
            supportPanel.Size = new Size(300, 150);
            supportPanel.Location = new Point(0, 340);

            Label lblSupportTitle = new Label();
            lblSupportTitle.Text = "Support";
            lblSupportTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblSupportTitle.Location = new Point(15, 15);
            lblSupportTitle.AutoSize = true;

            Label lblSupportDescription = new Label();
            lblSupportDescription.Text = "Get help and learn about AquaLedger.";
            lblSupportDescription.Font = new Font("Segoe UI", 9);
            lblSupportDescription.Location = new Point(15, 50);
            lblSupportDescription.AutoSize = true;

            Button btnHelp = new Button();
            btnHelp.Text = "Help";
            btnHelp.Location = new Point(15, 85);
            btnHelp.Size = new Size(100, 35);

            Button btnAbout = new Button();
            btnAbout.Text = "About AquaLedger";
            btnAbout.Location = new Point(125, 85);
            btnAbout.Size = new Size(150, 35);

            supportPanel.Controls.Add(lblSupportTitle);
            supportPanel.Controls.Add(lblSupportDescription);
            supportPanel.Controls.Add(btnHelp);
            supportPanel.Controls.Add(btnAbout);

            settingsLayout.Controls.Add(supportPanel, 0, 2);

            pnlContent.Controls.Add(settingsLayout);
            LayoutPage();

        }



    }

    // ======================================================================
    //  THEME
    // ======================================================================
    internal static class Theme
    {
        public static readonly Color PageBg = Color.FromArgb(243, 249, 254);
        public static readonly Color Navy = Color.FromArgb(11, 42, 91);
        public static readonly Color Teal = Color.FromArgb(26, 166, 183);
        public static readonly Color Green = Color.FromArgb(18, 135, 90);
        public static readonly Color Blue = Color.FromArgb(27, 108, 181);
        public static readonly Color Muted = Color.FromArgb(107, 124, 147);
        public static readonly Color Border = Color.FromArgb(222, 235, 246);

        public static readonly Font Emoji = new Font("Segoe UI Emoji", 12f);
        public static readonly Font EmojiSmall = new Font("Segoe UI Emoji", 10f);
    }

    // ======================================================================
    //  DRAWING HELPERS
    // ======================================================================
    internal static class Gfx
    {
        public static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            radius = Math.Max(1, Math.Min(radius, Math.Min(r.Width, r.Height) / 2));
            int d = radius * 2;
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static void DrawWave(Graphics g, Rectangle a, Color c, double phase,
            double amp, double baseFrac, double cycles)
        {
            if (a.Width < 8 || a.Height < 8) return;
            List<PointF> pts = new List<PointF>();
            for (int x = 0; x <= a.Width; x += 4)
            {
                double t = (double)x / a.Width;
                float y = (float)(a.Top + a.Height * baseFrac +
                    amp * Math.Sin(t * Math.PI * 2 * cycles + phase));
                pts.Add(new PointF(a.Left + x, y));
            }
            pts.Add(new PointF(a.Right, a.Bottom));
            pts.Add(new PointF(a.Left, a.Bottom));
            using (SolidBrush b = new SolidBrush(c))
                g.FillPolygon(b, pts.ToArray());
        }

        public static void Text(Graphics g, string s, Font f, Point p, Color c)
        {
            TextRenderer.DrawText(g, s, f, p, c,
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix |
                TextFormatFlags.Left | TextFormatFlags.Top);
        }

        public static void TextIn(Graphics g, string s, Font f, Rectangle r, Color c,
            TextFormatFlags extra)
        {
            TextRenderer.DrawText(g, s, f, r, c,
                extra | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
        }

        public static void TextCenter(Graphics g, string s, Font f, Rectangle r, Color c)
        {
            TextIn(g, s, f, r, c, TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }

        public static int Measure(string s, Font f)
        {
            return TextRenderer.MeasureText(s, f, new Size(2000, 200),
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
        }

        // AquaLedger logo: round badge + fish + wave + growth bars
        public static void DrawLogo(Graphics g, Rectangle r)
        {
            SmoothingMode old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (LinearGradientBrush b = new LinearGradientBrush(r,
                Color.FromArgb(24, 104, 190), Color.FromArgb(0, 170, 160), 45f))
                g.FillEllipse(b, r);
            using (Pen p = new Pen(Color.White, Math.Max(2f, r.Width / 16f)))
                g.DrawEllipse(p, r.X + 2, r.Y + 2, r.Width - 4, r.Height - 4);

            float w = r.Width, h = r.Height;

            // fish body + tail
            using (SolidBrush white = new SolidBrush(Color.White))
            {
                g.FillEllipse(white, r.X + w * 0.20f, r.Y + h * 0.26f, w * 0.40f, h * 0.24f);
                PointF[] tail =
                {
                    new PointF(r.X + w * 0.20f, r.Y + h * 0.38f),
                    new PointF(r.X + w * 0.08f, r.Y + h * 0.26f),
                    new PointF(r.X + w * 0.08f, r.Y + h * 0.50f)
                };
                g.FillPolygon(white, tail);
            }

            // growth bars
            using (SolidBrush green = new SolidBrush(Color.FromArgb(120, 230, 150)))
            {
                g.FillRectangle(green, r.X + w * 0.62f, r.Y + h * 0.50f, w * 0.07f, h * 0.18f);
                g.FillRectangle(green, r.X + w * 0.72f, r.Y + h * 0.40f, w * 0.07f, h * 0.28f);
                g.FillRectangle(green, r.X + w * 0.82f, r.Y + h * 0.28f, w * 0.07f, h * 0.40f);
            }

            // wave
            using (Pen wp = new Pen(Color.White, Math.Max(2f, w / 14f)))
            {
                wp.StartCap = LineCap.Round;
                wp.EndCap = LineCap.Round;
                g.DrawArc(wp, r.X + w * 0.16f, r.Y + h * 0.58f, w * 0.34f, h * 0.22f, 180, 180);
                g.DrawArc(wp, r.X + w * 0.46f, r.Y + h * 0.58f, w * 0.34f, h * 0.22f, 180, 180);
            }
            g.SmoothingMode = old;
        }
    }

    internal static class ControlExt
    {
        public static void PerformClickSafe(this Control c)
        {
            MethodInfo m = typeof(Control).GetMethod("OnClick",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (m != null) m.Invoke(c, new object[] { EventArgs.Empty });
        }
    }

    // ======================================================================
    //  SIDEBAR ITEM
    // ======================================================================
    internal class SidebarItem : Control
    {
        public string Glyph = "";
        private bool active;
        private bool hover;
        private static readonly Font TextFont = new Font("Segoe UI", 10.5f);

        public bool Active
        {
            get { return active; }
            set { active = value; Invalidate(); }
        }

        public SidebarItem()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor | ControlStyles.StandardClick, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(10, 3, Width - 20, Height - 6);

            if (active || hover)
            {
                Color fill = active ? Color.FromArgb(26, 166, 183)
                                    : Color.FromArgb(40, 255, 255, 255);
                using (GraphicsPath p = Gfx.RoundRect(r, 10))
                using (SolidBrush b = new SolidBrush(fill))
                    g.FillPath(b, p);
            }

            Gfx.TextCenter(g, Glyph, Theme.EmojiSmall,
                new Rectangle(r.X + 6, r.Y, 30, r.Height), Color.White);
            Gfx.TextIn(g, Text, TextFont,
                new Rectangle(r.X + 44, r.Y, r.Width - 48, r.Height), Color.White,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }



    }


    // ======================================================================
    //  HEADER BAR (date + welcome + user dropdown)
    // ======================================================================
    internal class HeaderBar : Panel
    {
        public string DateText = "", DayText = "", WelcomeText = "", CoopText = "";
        public ContextMenuStrip UserMenu;
        private Rectangle userRect = Rectangle.Empty;
        private static readonly Font BoldF = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        private static readonly Font SmallF = new Font("Segoe UI", 8.5f);

        public HeaderBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Cursor = userRect.Contains(e.Location) ? Cursors.Hand : Cursors.Default;
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (userRect.Contains(e.Location) && UserMenu != null)
                UserMenu.Show(this, new Point(Math.Max(0, userRect.Right - 170), userRect.Bottom));
            base.OnMouseClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = ClientRectangle;
            if (r.Width < 10 || r.Height < 10) return;

            using (LinearGradientBrush b = new LinearGradientBrush(r,
                Color.FromArgb(208, 233, 246), Color.FromArgb(232, 246, 246), 0f))
                g.FillRectangle(b, r);
            Gfx.DrawWave(g, r, Color.FromArgb(110, 255, 255, 255), 0.5, 7, 0.55, 1.5);
            Gfx.DrawWave(g, r, Color.FromArgb(70, 120, 210, 215), 2.2, 6, 0.75, 1.1);

            int cy = Height / 2;
            int right = Width - 22;

            // chevron
            using (Pen p = new Pen(Theme.Navy, 1.8f))
            {
                g.DrawLines(p, new Point[]
                {
                    new Point(right - 11, cy - 2), new Point(right - 7, cy + 2),
                    new Point(right - 3, cy - 2)
                });
            }
            int x = right - 20;

            // user text
            int tw = Math.Max(Gfx.Measure(WelcomeText, BoldF), Gfx.Measure(CoopText, SmallF));
            x -= tw;
            Gfx.Text(g, WelcomeText, BoldF, new Point(x, cy - 17), Theme.Navy);
            Gfx.Text(g, CoopText, SmallF, new Point(x, cy + 2), Theme.Muted);

            // avatar
            x -= 44;
            Rectangle av = new Rectangle(x, cy - 17, 34, 34);
            using (SolidBrush b = new SolidBrush(Color.FromArgb(37, 118, 200)))
                g.FillEllipse(b, av);
            using (SolidBrush w = new SolidBrush(Color.White))
            {
                g.FillEllipse(w, av.X + 11, av.Y + 6, 12, 12);
                g.SetClip(new Rectangle(av.X, av.Y, av.Width, av.Height - 3));
                g.FillEllipse(w, av.X + 6, av.Y + 20, 22, 18);
                g.ResetClip();
            }
            userRect = new Rectangle(x - 4, 6, right - x + 8, Height - 12);

            // divider
            x -= 18;
            using (Pen p = new Pen(Color.FromArgb(120, 11, 42, 91), 1f))
                g.DrawLine(p, x, cy - 18, x, cy + 18);

            // date
            int dw = Math.Max(Gfx.Measure(DateText, BoldF), Gfx.Measure(DayText, SmallF));
            x -= 16 + dw;
            Gfx.Text(g, DateText, BoldF, new Point(x, cy - 17), Theme.Navy);
            Gfx.Text(g, DayText, SmallF, new Point(x, cy + 2), Theme.Muted);
            Gfx.TextCenter(g, "\U0001F4C5", Theme.Emoji,
                new Rectangle(x - 34, cy - 14, 28, 28), Theme.Navy);
        }
    }

    // ======================================================================
    //  CARD PANEL (rounded, shadow, gradient, wave)
    // ======================================================================
    internal class CardPanel : Panel
    {
        public int Radius = 16;
        public Color FillTop = Color.White;
        public Color FillBottom = Color.White;
        public float GradientAngle = 60f;
        public Color BorderColor = Theme.Border;
        public Color WaveColor = Color.Empty;
        public bool Shadow = true;

        public CardPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.PageBg;
        }

        protected Rectangle CardRect
        {
            get { return new Rectangle(3, 1, Width - 7, Height - 8); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle c = CardRect;
            if (c.Width < 20 || c.Height < 20) return;

            if (Shadow)
            {
                for (int i = 0; i < 4; i++)
                {
                    Rectangle s = Rectangle.Inflate(c, i, i);
                    s.Offset(0, 2);
                    using (GraphicsPath sp = Gfx.RoundRect(s, Radius + i))
                    using (SolidBrush sb = new SolidBrush(Color.FromArgb(9, 20, 70, 130)))
                        g.FillPath(sb, sp);
                }
            }

            using (GraphicsPath path = Gfx.RoundRect(c, Radius))
            {
                if (FillTop == FillBottom)
                {
                    using (SolidBrush b = new SolidBrush(FillTop)) g.FillPath(b, path);
                }
                else
                {
                    using (LinearGradientBrush b = new LinearGradientBrush(c, FillTop,
                        FillBottom, GradientAngle))
                        g.FillPath(b, path);
                }

                if (WaveColor != Color.Empty)
                {
                    g.SetClip(path);
                    Rectangle wr = new Rectangle(c.X, c.Y + c.Height / 2, c.Width, c.Height / 2);
                    Gfx.DrawWave(g, wr, WaveColor, 0.4, 5, 0.45, 1.3);
                    Gfx.DrawWave(g, wr, Color.FromArgb(Math.Max(10, WaveColor.A - 25),
                        WaveColor.R, WaveColor.G, WaveColor.B), 2.4, 4, 0.70, 1.0);
                    g.ResetClip();
                }

                using (Pen p = new Pen(BorderColor, 1f)) g.DrawPath(p, path);
            }

            DrawContent(g, c);
        }

        protected virtual void DrawContent(Graphics g, Rectangle card) { }
    }

    // ======================================================================
    //  SUMMARY CARD (Fish Stock / Feed Stock / Sales / Expenses)
    // ======================================================================
    internal class SummaryCard : CardPanel
    {
        public string Caption = "", ValueText = "", UnitText = "", Glyph = "";
        public Color IconColor = Color.SteelBlue;
        private static readonly Font CapF = new Font("Segoe UI", 10f);
        private static readonly Font ValF = new Font("Segoe UI", 20f, FontStyle.Bold);
        private static readonly Font UnitF = new Font("Segoe UI", 8.5f);

        public SummaryCard() { Cursor = Cursors.Hand; }

        protected override void DrawContent(Graphics g, Rectangle c)
        {
            Rectangle ic = new Rectangle(c.X + 16, c.Y + 14, 38, 38);
            using (SolidBrush b = new SolidBrush(IconColor)) g.FillEllipse(b, ic);
            Gfx.TextCenter(g, Glyph, Theme.EmojiSmall, ic, Color.White);

            Gfx.Text(g, Caption, CapF, new Point(c.X + 64, c.Y + 24), Theme.Navy);
            Gfx.Text(g, ValueText, ValF, new Point(c.X + 62, c.Y + 42), Theme.Navy);
            if (!string.IsNullOrEmpty(UnitText))
                Gfx.Text(g, UnitText, UnitF, new Point(c.X + 66, c.Y + 76), Theme.Muted);
        }
    }

    // ======================================================================
    //  QUICK ACTIONS
    // ======================================================================
    internal class QuickActionsPanel : CardPanel
    {
        private static readonly Font TitleF = new Font("Segoe UI", 10f, FontStyle.Bold);

        public QuickActionsPanel() { Radius = 14; }

        protected override void DrawContent(Graphics g, Rectangle c)
        {
            Gfx.TextCenter(g, "\u26A1", Theme.EmojiSmall,
                new Rectangle(c.X + 14, c.Y + 12, 22, 22), Theme.Blue);
            Gfx.Text(g, "Quick Actions", TitleF, new Point(c.X + 40, c.Y + 15), Theme.Navy);
        }
    }

    internal class QuickButton : Control
    {
        public string Glyph = "";
        public Color BaseColor = Theme.Blue;
        private bool hover, down;
        private static readonly Font LabelF = new Font("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font PlusF = new Font("Segoe UI", 16f, FontStyle.Bold);

        public QuickButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.StandardClick, true);
            BackColor = Color.White;
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { down = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { down = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color c = BaseColor;
            if (down) c = ControlPaint.Dark(c, 0.05f);
            else if (hover) c = ControlPaint.Light(c, 0.2f);

            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath p = Gfx.RoundRect(r, 12))
            using (LinearGradientBrush b = new LinearGradientBrush(r,
                ControlPaint.Light(c, 0.12f), c, 90f))
                g.FillPath(b, p);

            Rectangle top = new Rectangle(0, 6, Width, Height / 2 - 4);
            if (Glyph == "+")
                Gfx.TextCenter(g, "+", PlusF, top, Color.White);
            else
                Gfx.TextCenter(g, Glyph, Theme.Emoji, top, Color.White);

            Gfx.TextCenter(g, Text, LabelF,
                new Rectangle(0, Height / 2 + 2, Width, Height / 2 - 6), Color.White);
        }
    }

    // ======================================================================
    //  ALERTS
    // ======================================================================
    internal enum AlertSeverity { Success = 0, Info = 1, Warning = 2, Critical = 3 }

    internal class AlertItem
    {
        public AlertSeverity Severity;
        public string Title, Detail, Target;
        public AlertItem(AlertSeverity s, string title, string detail, string target)
        {
            Severity = s; Title = title; Detail = detail; Target = target;
        }
    }

    internal class AlertsPanel : CardPanel
    {
        public List<AlertItem> Items = new List<AlertItem>();
        public event Action<AlertItem> AlertClicked;
        public event Action ViewAllClicked;

        private Rectangle viewAllRect = Rectangle.Empty;
        private readonly List<Rectangle> rowRects = new List<Rectangle>();
        private const int RowH = 54;
        private const int HeaderH = 52;

        private static readonly Font TitleF = new Font("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font LinkF = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        private static readonly Font RowTitleF = new Font("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font RowDetailF = new Font("Segoe UI", 8.5f);
        private static readonly Font ChevF = new Font("Segoe UI", 14f);

        public AlertsPanel() { Radius = 14; }

        public int PreferredHeight
        {
            get { return HeaderH + Math.Max(1, Items.Count) * RowH + 16; }
        }

        private static Color SeverityColor(AlertSeverity s)
        {
            switch (s)
            {
                case AlertSeverity.Critical: return Color.FromArgb(214, 62, 62);
                case AlertSeverity.Warning: return Color.FromArgb(242, 150, 40);
                case AlertSeverity.Success: return Color.FromArgb(46, 158, 91);
                default: return Color.FromArgb(27, 108, 181);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool over = viewAllRect.Contains(e.Location);
            foreach (Rectangle r in rowRects) if (r.Contains(e.Location)) over = true;
            Cursor = over ? Cursors.Hand : Cursors.Default;
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (viewAllRect.Contains(e.Location))
            {
                if (ViewAllClicked != null) ViewAllClicked();
            }
            else
            {
                for (int i = 0; i < rowRects.Count && i < Items.Count; i++)
                    if (rowRects[i].Contains(e.Location) && AlertClicked != null)
                    { AlertClicked(Items[i]); break; }
            }
            base.OnMouseClick(e);
        }

        protected override void DrawContent(Graphics g, Rectangle c)
        {
            Gfx.TextCenter(g, "\U0001F514", Theme.EmojiSmall,
                new Rectangle(c.X + 14, c.Y + 12, 22, 22), Theme.Navy);
            Gfx.Text(g, "Alerts", TitleF, new Point(c.X + 40, c.Y + 15), Theme.Navy);

            int lw = Gfx.Measure("View All", LinkF);
            Point lp = new Point(c.Right - lw - 16, c.Y + 17);
            Gfx.Text(g, "View All", LinkF, lp, Theme.Blue);
            viewAllRect = new Rectangle(lp.X - 4, lp.Y - 4, lw + 8, 24);

            rowRects.Clear();
            int y = c.Y + HeaderH - 6;
            for (int i = 0; i < Items.Count; i++)
            {
                AlertItem a = Items[i];
                Rectangle row = new Rectangle(c.X, y, c.Width, RowH);
                rowRects.Add(row);

                if (i > 0)
                    using (Pen p = new Pen(Color.FromArgb(232, 240, 247), 1f))
                        g.DrawLine(p, c.X + 14, y, c.Right - 14, y);

                Rectangle ic = new Rectangle(c.X + 14, y + 12, 30, 30);
                using (SolidBrush b = new SolidBrush(
                    Color.FromArgb(40, SeverityColor(a.Severity))))
                    g.FillEllipse(b, ic);
                string glyph = a.Severity == AlertSeverity.Info ? "i" :
                               a.Severity == AlertSeverity.Success ? "\u2713" : "\u26A0";
                Gfx.TextCenter(g, glyph,
                    a.Severity == AlertSeverity.Info
                        ? new Font("Segoe UI", 10f, FontStyle.Bold) : Theme.EmojiSmall,
                    ic, SeverityColor(a.Severity));

                int tx = c.X + 54;
                int tw = c.Width - 54 - 30;
                Gfx.TextIn(g, a.Title, RowTitleF, new Rectangle(tx, y + 10, tw, 18),
                    Theme.Navy, TextFormatFlags.Left | TextFormatFlags.SingleLine |
                    TextFormatFlags.EndEllipsis);
                Gfx.TextIn(g, a.Detail, RowDetailF, new Rectangle(tx, y + 28, tw, 18),
                    Theme.Muted, TextFormatFlags.Left | TextFormatFlags.SingleLine |
                    TextFormatFlags.EndEllipsis);

                Gfx.TextCenter(g, "\u203A", ChevF,
                    new Rectangle(c.Right - 30, y + 12, 20, 30), Theme.Muted);
                y += RowH;
            }

            if (Items.Count == 0)
                Gfx.TextIn(g, "No alerts right now.", RowDetailF,
                    new Rectangle(c.X + 16, c.Y + HeaderH, c.Width - 32, 30),
                    Theme.Muted, TextFormatFlags.Left);
        }
    }

    // ======================================================================
    //  COOPERATIVE OVERVIEW
    // ======================================================================
    internal class OverviewCard : CardPanel
    {
        public string CoopName = "", Members = "", Farms = "", Ponds = "";
        private static readonly Font TitleF = new Font("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font LabelF = new Font("Segoe UI", 8f);
        private static readonly Font ValueF = new Font("Segoe UI", 10f, FontStyle.Bold);

        public OverviewCard() { Radius = 14; }

        protected override void DrawContent(Graphics g, Rectangle c)
        {
            Gfx.TextCenter(g, "\U0001F465", Theme.EmojiSmall,
                new Rectangle(c.X + 14, c.Y + 12, 24, 24), Theme.Blue);
            Gfx.Text(g, "Cooperative Overview", TitleF, new Point(c.X + 42, c.Y + 15), Theme.Navy);

            using (Pen p = new Pen(Color.FromArgb(228, 238, 246), 1f))
                g.DrawLine(p, c.X + 14, c.Y + 46, c.Right - 14, c.Y + 46);

            string[] glyphs = { "\U0001F3E2", "\U0001F465", "\U0001F3E1", "\U0001F30A" };
            string[] labels = { "Cooperative Name", "Total Member Farmers", "Total Farms", "Total Fishponds" };
            string[] values = { CoopName, Members, Farms, Ponds };

            int y = c.Y + 56;
            int rowH = Math.Max(36, (c.Bottom - y - 8) / 4);
            for (int i = 0; i < 4; i++)
            {
                Gfx.TextCenter(g, glyphs[i], Theme.Emoji,
                    new Rectangle(c.X + 14, y + 2, 30, rowH - 6), Theme.Blue);
                Gfx.Text(g, labels[i], LabelF, new Point(c.X + 54, y + 2), Theme.Muted);
                Gfx.TextIn(g, values[i], ValueF,
                    new Rectangle(c.X + 54, y + 16, c.Width - 64, 20), Theme.Navy,
                    TextFormatFlags.Left | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
                y += rowH;
            }
        }
    }

    // ======================================================================
    //  BANNER (photo if available, otherwise drawn fish-cage scene)
    //  Optional photo: <app folder>\Assets\fish_cage_banner.jpg
    // ======================================================================
    internal class BannerPanel : Panel
    {
        private Image photo;
        private static readonly Font TitleF = new Font("Segoe UI", 20f, FontStyle.Bold);
        private static readonly Font SubF = new Font("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font DescF = new Font("Segoe UI", 8.5f);

        public BannerPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.PageBg;
            try
            {
                string path = Path.Combine(Application.StartupPath, "Assets", "fish_cage_banner.jpg");
                if (File.Exists(path))
                    using (Image tmp = Image.FromFile(path)) photo = new Bitmap(tmp);
            }
            catch { photo = null; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && photo != null) photo.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(3, 1, Width - 7, Height - 8);
            if (r.Width < 40 || r.Height < 40) return;

            // soft shadow
            for (int i = 0; i < 4; i++)
            {
                Rectangle s = Rectangle.Inflate(r, i, i); s.Offset(0, 2);
                using (GraphicsPath sp = Gfx.RoundRect(s, 16 + i))
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(9, 20, 70, 130)))
                    g.FillPath(sb, sp);
            }

            using (GraphicsPath path = Gfx.RoundRect(r, 16))
            {
                g.SetClip(path);
                if (photo != null) DrawPhoto(g, r); else DrawScene(g, r);

                // white fade so the text is readable
                using (LinearGradientBrush fade = new LinearGradientBrush(r,
                    Color.FromArgb(215, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), 90f))
                {
                    Rectangle fr = new Rectangle(r.X, r.Y, r.Width, (int)(r.Height * 0.55));
                    using (LinearGradientBrush fade2 = new LinearGradientBrush(fr,
                        Color.FromArgb(215, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), 90f))
                        g.FillRectangle(fade2, fr);
                }
                g.ResetClip();
                using (Pen p = new Pen(Theme.Border, 1f)) g.DrawPath(p, path);
            }

            // text
            Gfx.Text(g, "AquaLedger", TitleF, new Point(r.X + 20, r.Y + 14), Theme.Navy);
            Gfx.Text(g, "Your Cooperative's Digital Fish Farm Records", SubF,
                new Point(r.X + 22, r.Y + 50), Color.FromArgb(22, 115, 170));
            Gfx.TextIn(g,
                "Track production, manage expenses, record sales,\r\nand generate reports \u2014 all in one place.",
                DescF, new Rectangle(r.X + 22, r.Y + 74, r.Width - 40, 40),
                Color.FromArgb(60, 70, 85), TextFormatFlags.Left | TextFormatFlags.Top);

            using (Pen wp = new Pen(Theme.Teal, 2f))
            {
                wp.StartCap = LineCap.Round; wp.EndCap = LineCap.Round;
                g.DrawArc(wp, r.X + 22, r.Y + 116, 40, 8, 180, 180);
                g.DrawArc(wp, r.X + 62, r.Y + 116, 40, 8, 0, 180);
            }
        }

        private void DrawPhoto(Graphics g, Rectangle r)
        {
            float scale = Math.Max((float)r.Width / photo.Width, (float)r.Height / photo.Height);
            int sw = (int)(r.Width / scale), sh = (int)(r.Height / scale);
            int sx = (photo.Width - sw) / 2, sy = (photo.Height - sh) / 2;
            g.DrawImage(photo, r, sx, sy, sw, sh, GraphicsUnit.Pixel);
        }

        // procedural fallback: sky, clouds, mountains, trees, water, round fish cage
        private void DrawScene(Graphics g, Rectangle r)
        {
            using (LinearGradientBrush sky = new LinearGradientBrush(r,
                Color.FromArgb(110, 178, 232), Color.FromArgb(226, 241, 250), 90f))
                g.FillRectangle(sky, r);

            using (SolidBrush cloud = new SolidBrush(Color.FromArgb(190, 255, 255, 255)))
            {
                g.FillEllipse(cloud, r.X + r.Width * 0.55f, r.Y + r.Height * 0.10f, r.Width * 0.22f, r.Height * 0.10f);
                g.FillEllipse(cloud, r.X + r.Width * 0.66f, r.Y + r.Height * 0.06f, r.Width * 0.18f, r.Height * 0.12f);
                g.FillEllipse(cloud, r.X + r.Width * 0.12f, r.Y + r.Height * 0.20f, r.Width * 0.20f, r.Height * 0.08f);
            }

            float w = r.Width, h = r.Height;
            // mountains
            PointF[] m1 =
            {
                new PointF(r.X, r.Y + h * 0.62f), new PointF(r.X + w * 0.18f, r.Y + h * 0.46f),
                new PointF(r.X + w * 0.38f, r.Y + h * 0.58f), new PointF(r.X + w * 0.62f, r.Y + h * 0.42f),
                new PointF(r.X + w * 0.85f, r.Y + h * 0.55f), new PointF(r.Right, r.Y + h * 0.48f),
                new PointF(r.Right, r.Y + h * 0.66f), new PointF(r.X, r.Y + h * 0.66f)
            };
            using (SolidBrush b = new SolidBrush(Color.FromArgb(84, 134, 150))) g.FillPolygon(b, m1);

            // tree line
            using (SolidBrush tree = new SolidBrush(Color.FromArgb(52, 128, 76)))
            {
                g.FillRectangle(tree, r.X, r.Y + h * 0.62f, w, h * 0.06f);
                for (int x = 0; x < r.Width; x += 18)
                    g.FillEllipse(tree, r.X + x, r.Y + h * 0.58f, 22, h * 0.09f);
            }

            // water
            Rectangle water = new Rectangle(r.X, (int)(r.Y + h * 0.67f), r.Width, (int)(h * 0.33f) + 1);
            using (LinearGradientBrush wb = new LinearGradientBrush(water,
                Color.FromArgb(70, 150, 200), Color.FromArgb(22, 88, 150), 90f))
                g.FillRectangle(wb, water);
            Gfx.DrawWave(g, water, Color.FromArgb(40, 255, 255, 255), 0.5, 2.5, 0.35, 3);

            // round fish cage
            float cw = w * 0.62f, ch = h * 0.20f;
            float cx = r.X + w * 0.42f, cyy = r.Y + h * 0.82f;
            RectangleF outer = new RectangleF(cx - cw / 2, cyy - ch / 2, cw, ch);
            using (SolidBrush inner = new SolidBrush(Color.FromArgb(70, 40, 120, 170)))
                g.FillEllipse(inner, outer);
            using (Pen ring = new Pen(Color.FromArgb(45, 55, 65), 5f))
                g.DrawEllipse(ring, outer);
            RectangleF outer2 = RectangleF.Inflate(outer, -cw * 0.07f, -ch * 0.12f);
            using (Pen ring2 = new Pen(Color.FromArgb(150, 45, 55, 65), 2f))
                g.DrawEllipse(ring2, outer2);

            // walkway to the shore
            using (Pen walk = new Pen(Color.FromArgb(90, 80, 70), 4f))
                g.DrawLine(walk, cx + cw / 2, cyy, r.Right - 6, cyy - ch * 0.9f);
        }


    }






}