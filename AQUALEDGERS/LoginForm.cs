using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AQUALEDGERS
{
    // ======================================================================
    //  LOGIN FORM
    //  Uses the existing designer controls: txtUsername, txtPIN,
    //  cmbAccountType (kept as the hidden data holder for the role).
    //  All other designer controls are replaced by the new design.
    //  NOTE: relies on the helper classes Theme, Gfx and CardPanel that are
    //  declared at the bottom of DashboardForm.cs (same project).
    //  Background photo: name the image "login_bg" (Properties > Resources),
    //  or place login_bg.jpg next to the .exe. If missing, a painted scene
    //  is used and a message lists what was found.
    // ======================================================================
    public partial class LoginForm : Form
    {
        // new UI parts
        private LoginBackground pnlBackground;
        private LoginCard pnlCard;
        private FieldBox boxUser;
        private FieldBox boxPin;
        private RoleCard roleFarmer;
        private RoleCard roleBuyer;
        private PrimaryButton btnLogIn;
        private Label lnkSignUp;
        private Label lnkHelp;

        // account type values taken from cmbAccountType
        private object farmerItem;
        private object buyerItem;

        public LoginForm()
        {
            InitializeComponent();

            try
            {
                AQUALEDGERS.Data.Db.Initialize();
                AQUALEDGERS.Services.AuthService.CreateDefaultUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Detailed AquaLedger Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            BuildLoginUi();
        }

        // ==================================================================
        //  NEW DESIGN
        // ==================================================================
        private void BuildLoginUi()
        {
            this.Text = "AquaLedger - Login";
            this.BackColor = Theme.PageBg;
            this.BackgroundImage = null;
            this.MinimumSize = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.AcceptButton = null;   // Enter is handled below

            // keep the real input controls, drop the old design
            TextBox user = txtUsername;
            TextBox pin = txtPIN;
            this.Controls.Clear();

            // ---- background (photo + branding) ----
            pnlBackground = new LoginBackground();
            pnlBackground.Dock = DockStyle.Fill;
            this.Controls.Add(pnlBackground);

            // ---- login card ----
            pnlCard = new LoginCard();
            pnlCard.Size = new Size(410, 600);
            pnlBackground.Controls.Add(pnlCard);

            // username field
            boxUser = new FieldBox();
            boxUser.Caption = "Username";
            boxUser.Icon = FieldIcon.User;
            boxUser.Bounds = new Rectangle(33, 188, 343, 58);
            boxUser.AttachEditor(user, "Enter your username");
            pnlCard.Controls.Add(boxUser);

            // PIN field with show / hide eye
            boxPin = new FieldBox();
            boxPin.Caption = "PIN";
            boxPin.Icon = FieldIcon.Lock;
            boxPin.HasEye = true;
            boxPin.Bounds = new Rectangle(33, 258, 343, 58);
            pin.PasswordChar = '\0';
            pin.UseSystemPasswordChar = true;
            boxPin.AttachEditor(pin, "Enter your PIN");
            boxPin.EyeClicked += delegate
            {
                boxPin.EyeRevealed = !boxPin.EyeRevealed;
                pin.UseSystemPasswordChar = !boxPin.EyeRevealed;
                boxPin.Invalidate();
            };
            pnlCard.Controls.Add(boxPin);

            // role cards (they drive the hidden cmbAccountType)
            ResolveAccountTypes();

            roleFarmer = new RoleCard();
            roleFarmer.Title = "Farmer / Cooperative";
            roleFarmer.Description = "Manage your farm records and marketplace listings.";
            roleFarmer.Farmer = true;
            roleFarmer.Bounds = new Rectangle(33, 352, 166, 104);
            roleFarmer.Click += delegate { SelectRole(true); };
            pnlCard.Controls.Add(roleFarmer);

            roleBuyer = new RoleCard();
            roleBuyer.Title = "Buyer";
            roleBuyer.Description = "Browse fish, place orders, and track your purchases.";
            roleBuyer.Farmer = false;
            roleBuyer.Bounds = new Rectangle(210, 352, 166, 104);
            roleBuyer.Click += delegate { SelectRole(false); };
            pnlCard.Controls.Add(roleBuyer);

            // Log In button
            btnLogIn = new PrimaryButton();
            btnLogIn.Text = "Log In";
            btnLogIn.Bounds = new Rectangle(33, 474, 343, 48);
            btnLogIn.Click += delegate { txtLogin_Click(this, EventArgs.Empty); };
            pnlCard.Controls.Add(btnLogIn);

            // links
            lnkSignUp = MakeLink("New buyer? Create an account", 534);
            lnkSignUp.Click += delegate
            {
                SelectRole(false);                       // switch to Buyer automatically
                btnSignUp_Click(this, EventArgs.Empty);
            };
            pnlCard.Controls.Add(lnkSignUp);

            lnkHelp = MakeLink("Need help? Contact your cooperative.", 560);
            lnkHelp.Click += delegate
            {
                MessageBox.Show(
                    "Please contact your cooperative administrator to create a " +
                    "farmer account or to reset your PIN.",
                    "AquaLedger Help",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };
            pnlCard.Controls.Add(lnkHelp);

            // keyboard: Enter moves / logs in
            user.KeyDown += delegate (object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; pin.Focus(); }
            };
            pin.KeyDown += delegate (object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    txtLogin_Click(this, EventArgs.Empty);
                }
            };

            // keep the card on the right side
            pnlBackground.Resize += delegate { PlaceCard(); };
            this.Load += delegate { PlaceCard(); user.Focus(); };

            SelectRole(true);   // default role = Farmer / Cooperative
            PlaceCard();
        }

        private Label MakeLink(string text, int y)
        {
            Label l = new Label();
            l.Text = text;
            l.AutoSize = false;
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Font = new Font("Segoe UI", 8.5f, FontStyle.Underline);
            l.ForeColor = Theme.Blue;
            l.BackColor = Color.Transparent;
            l.Cursor = Cursors.Hand;
            l.Bounds = new Rectangle(33, y, 343, 20);
            return l;
        }

        private void PlaceCard()
        {
            if (pnlCard == null || pnlBackground == null) return;
            int x = pnlBackground.ClientSize.Width - pnlCard.Width - 44;
            int y = Math.Max(8, (pnlBackground.ClientSize.Height - pnlCard.Height) / 2);
            pnlCard.Location = new Point(Math.Max(0, x), y);
        }

        // Finds the "Farmer" and "Buyer" values already stored in cmbAccountType
        private void ResolveAccountTypes()
        {
            farmerItem = null;
            buyerItem = null;

            foreach (object it in cmbAccountType.Items)
            {
                string s = it.ToString();
                if (s.IndexOf("buyer", StringComparison.OrdinalIgnoreCase) >= 0)
                    buyerItem = it;
                else if (s.IndexOf("farm", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         s.IndexOf("coop", StringComparison.OrdinalIgnoreCase) >= 0)
                    farmerItem = it;
            }
            if (farmerItem == null)
            {
                foreach (object it in cmbAccountType.Items)
                    if (!object.ReferenceEquals(it, buyerItem)) { farmerItem = it; break; }
            }
        }

        private void SelectRole(bool farmer)
        {
            roleFarmer.Selected = farmer;
            roleBuyer.Selected = !farmer;

            object item = farmer ? farmerItem : buyerItem;
            if (item != null) cmbAccountType.SelectedItem = item;
        }

        // ==================================================================
        //  ORIGINAL LOGIC (unchanged)
        // ==================================================================
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string pin = txtPIN.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pin))
            {
                MessageBox.Show(
                    "Please enter your username and PIN.",
                    "AquaLedger Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                string accountType =
    cmbAccountType.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(accountType))
                {
                    MessageBox.Show(
                        "Please select an account type.",
                        "AquaLedger",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                bool isValid =
                    AQUALEDGERS.Services.AuthService.ValidateLogin(
                        username, pin, accountType);

                if (isValid)
                {
                    this.Hide();

                    using (DashboardForm dashboard = new DashboardForm())
                    {
                        dashboard.ShowDialog();
                    }

                    this.Show();
                    txtPIN.Clear();
                }
                else
                {
                    MessageBox.Show(
                        "Invalid username or PIN.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtPIN.Clear();
                    txtPIN.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Login error: " + ex.Message,
                    "AquaLedger Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            if (cmbAccountType.SelectedItem == null ||
       cmbAccountType.SelectedItem.ToString() != "Buyer")
            {
                MessageBox.Show(
                    "Select Buyer to create a buyer account.",
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (BuyerSignUpForm form = new BuyerSignUpForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    cmbAccountType.SelectedItem = "Buyer";
                    txtUsername.Clear();
                    txtPIN.Clear();
                    txtUsername.Focus();
                }
            }
        }

        private void txtLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string pin = txtPIN.Text;

            string accountType = cmbAccountType.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pin))
            {
                MessageBox.Show(
                    "Please enter your username and PIN.",
                    "AquaLedger Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(accountType))
            {
                MessageBox.Show(
                    "Please select an account type.",
                    "AquaLedger Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool isValid =
                    AQUALEDGERS.Services.AuthService.ValidateLogin(
                        username, pin, accountType);

                if (isValid)
                {
                    this.Hide();

                    if (accountType == "Buyer")
                    {
                        using (BuyerDashboardForm dashboard =
                            new BuyerDashboardForm())
                        {
                            dashboard.ShowDialog();
                        }
                    }
                    else
                    {
                        using (DashboardForm dashboard =
                            new DashboardForm())
                        {
                            dashboard.ShowDialog();
                        }
                    }

                    this.Show();
                    txtPIN.Clear();
                    txtUsername.Focus();
                }
                else
                {
                    MessageBox.Show(
                        "Invalid username or PIN. Check your account type too.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtPIN.Clear();
                    txtPIN.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Login error: " + ex.Message,
                    "AquaLedger Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }

    // ======================================================================
    //  BACKGROUND: photo (or painted scene) + AquaLedger branding + features
    // ======================================================================
    internal class LoginBackground : Panel
    {
        private static readonly Font WordF = new Font("Segoe UI Semibold", 34f);
        private static readonly Font TagF = new Font("Segoe UI", 13f);
        private static readonly Font FeatTitleF = new Font("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font FeatDescF = new Font("Segoe UI", 8.5f);
        private static readonly Font FooterF = new Font("Segoe Script", 12f, FontStyle.Italic);
        private static readonly Font EmojiF = new Font("Segoe UI Emoji", 16f);

        // photo support
        private static Image bgPhoto;
        private static bool bgLoaded;
        private Bitmap sceneCache;   // photo pre-scaled to the current size

        public LoginBackground()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.PageBg;

            Image preload = BgPhoto;   // load now (not in the middle of painting)
        }

        // ---------------- photo loading ----------------
        private static Image BgPhoto
        {
            get
            {
                if (!bgLoaded)
                {
                    bgLoaded = true;
                    bgPhoto = LoadBackground();
                }
                return bgPhoto;
            }
        }

        private static Image LoadBackground()
        {
            Assembly asm = typeof(LoginForm).Assembly;
            List<string> seen = new List<string>();

            // 1) Any Image property in the project's Resources class whose name looks right
            try
            {
                foreach (Type t in asm.GetTypes())
                {
                    if (!t.FullName.EndsWith(".Properties.Resources")) continue;
                    foreach (PropertyInfo p in t.GetProperties(
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (p.PropertyType != typeof(Bitmap) && p.PropertyType != typeof(Image)) continue;
                        seen.Add(p.Name);
                        string n = p.Name.ToLowerInvariant();
                        if (n.Contains("login") || n.Contains("bg") ||
    n.Contains("background") || n.Contains("pond") || n.Contains("cropped"))
                        {
                            Image img = p.GetValue(null, null) as Image;
                            if (img != null) return img;
                        }
                    }
                }
            }
            catch { }

            // 2) Image file next to the .exe (set "Copy to Output Directory" = Copy always)
            string[] files = { "login_bg.jpg", "login_bg.jpeg", "login_bg.png",
                               @"Images\login_bg.jpg", @"Images\login_bg.png",
                               @"Resources\login_bg.jpg", @"Resources\login_bg.png" };
            foreach (string f in files)
            {
                try
                {
                    string path = Path.Combine(Application.StartupPath, f);
                    if (File.Exists(path))
                        using (Image tmp = Image.FromFile(path)) return new Bitmap(tmp);
                }
                catch { }
            }

            // 3) Embedded resource whose name contains "login_bg"
            try
            {
                foreach (string name in asm.GetManifestResourceNames())
                {
                    if (name.IndexOf("login_bg", StringComparison.OrdinalIgnoreCase) < 0) continue;
                    using (Stream s = asm.GetManifestResourceStream(name))
                    using (Image tmp = Image.FromStream(s)) return new Bitmap(tmp);
                }
            }
            catch { }

            // 4) Not found: tell you once what exists (delete this block when it works)
            MessageBox.Show(
                "login_bg image not found.\n\nImage resources in this project:\n" +
                (seen.Count == 0 ? "(none)" : string.Join("\n", seen)),
                "Background image");
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && sceneCache != null)
            {
                sceneCache.Dispose();
                sceneCache = null;
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int W = Width, H = Height;
            if (W < 200 || H < 200) return;

            DrawScene(g, W, H);

            // white fade behind the branding text (top-left)
            Rectangle fr = new Rectangle(-2, -2, W + 4, (int)(H * 0.78) + 2);
            using (LinearGradientBrush fade = new LinearGradientBrush(fr,
                Color.FromArgb(225, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), 90f))
            {
                fade.WrapMode = WrapMode.TileFlipXY;
                g.FillRectangle(fade, fr);
            }

            // frosted tint behind the login card
            Rectangle tr = new Rectangle(W - 524, -2, 526, H + 4);
            using (LinearGradientBrush tint = new LinearGradientBrush(tr,
                Color.FromArgb(0, 255, 255, 255), Color.FromArgb(95, 255, 255, 255), 0f))
            {
                tint.WrapMode = WrapMode.TileFlipXY;
                g.FillRectangle(tint, tr);
            }

            DrawBranding(g, W, H);
            DrawFeatures(g, W, H);
            DrawFooter(g, W, H);
        }

        // ---------------- scene (photo, with painted fallback) ----------------
        // Cover-fit: fills the panel, keeps aspect ratio, crops overflow, centered.
        private void DrawScene(Graphics g, int W, int H)
        {
            Image img = BgPhoto;
            if (img == null) { DrawPaintedScene(g, W, H); return; }

            if (sceneCache == null || sceneCache.Width != W || sceneCache.Height != H)
            {
                if (sceneCache != null) sceneCache.Dispose();
                sceneCache = new Bitmap(W, H);

                float s = Math.Max((float)W / img.Width, (float)H / img.Height);
                float srcW = W / s, srcH = H / s;
                float srcX = (img.Width - srcW) / 2f;
                float srcY = (img.Height - srcH) / 2f;

                using (Graphics cg = Graphics.FromImage(sceneCache))
                {
                    cg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    cg.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    cg.DrawImage(img, new Rectangle(0, 0, W, H),
                        srcX, srcY, srcW, srcH, GraphicsUnit.Pixel);
                }
            }
            g.DrawImageUnscaled(sceneCache, 0, 0);
        }

        // ---------------- painted fallback scene ----------------
        private void DrawPaintedScene(Graphics g, int W, int H)
        {
            Rectangle all = new Rectangle(0, 0, W, H);

            using (LinearGradientBrush sky = new LinearGradientBrush(all,
                Color.FromArgb(104, 174, 236), Color.FromArgb(228, 242, 252), 90f))
                g.FillRectangle(sky, all);

            // clouds
            using (SolidBrush cloud = new SolidBrush(Color.FromArgb(185, 255, 255, 255)))
            {
                g.FillEllipse(cloud, W * 0.28f, H * 0.05f, W * 0.20f, H * 0.10f);
                g.FillEllipse(cloud, W * 0.36f, H * 0.02f, W * 0.16f, H * 0.12f);
                g.FillEllipse(cloud, W * 0.02f, H * 0.28f, W * 0.18f, H * 0.08f);
                g.FillEllipse(cloud, W * 0.55f, H * 0.18f, W * 0.18f, H * 0.07f);
            }

            // mountains
            PointF[] m =
            {
                new PointF(0, H * 0.58f), new PointF(W * 0.10f, H * 0.50f),
                new PointF(W * 0.22f, H * 0.57f), new PointF(W * 0.38f, H * 0.47f),
                new PointF(W * 0.55f, H * 0.56f), new PointF(W * 0.72f, H * 0.48f),
                new PointF(W * 0.88f, H * 0.56f), new PointF(W, H * 0.50f),
                new PointF(W, H * 0.66f), new PointF(0, H * 0.66f)
            };
            using (SolidBrush b = new SolidBrush(Color.FromArgb(86, 136, 156)))
                g.FillPolygon(b, m);

            // tree line + palms
            using (SolidBrush tree = new SolidBrush(Color.FromArgb(54, 132, 78)))
            {
                g.FillRectangle(tree, 0, H * 0.615f, W, H * 0.05f);
                for (int x = 0; x < W; x += 20)
                    g.FillEllipse(tree, x, H * 0.595f, 26, H * 0.06f);
            }
            float[] palmX = { 0.06f, 0.14f, 0.27f, 0.35f, 0.46f, 0.55f, 0.66f, 0.80f, 0.92f };
            for (int i = 0; i < palmX.Length; i++)
                DrawPalm(g, W * palmX[i], H * 0.64f, H * (0.07f + 0.015f * (i % 3)));

            // water
            Rectangle water = new Rectangle(0, (int)(H * 0.655f), W, H - (int)(H * 0.655f));
            using (LinearGradientBrush wb = new LinearGradientBrush(water,
                Color.FromArgb(78, 156, 206), Color.FromArgb(20, 84, 146), 90f))
                g.FillRectangle(wb, water);
            Gfx.DrawWave(g, water, Color.FromArgb(28, 255, 255, 255), 0.4, 3, 0.25, 6);
            Gfx.DrawWave(g, water, Color.FromArgb(22, 255, 255, 255), 2.0, 4, 0.55, 5);

            // net cages (rows grow bigger toward the viewer)
            float cageRight = W * 0.64f;
            for (int k = 0; k < 4; k++)
            {
                float scale = 1f + 0.55f * k;
                float yTop = H * 0.675f + k * H * 0.058f * (1f + 0.25f * k);
                float hh = H * 0.026f * scale;
                RectangleF band = new RectangleF(0, yTop, cageRight + k * 20, hh);

                using (SolidBrush frame = new SolidBrush(Color.FromArgb(215, 28, 46, 58)))
                    g.FillRectangle(frame, band);
                using (SolidBrush shine = new SolidBrush(Color.FromArgb(110, 190, 215, 225)))
                    g.FillRectangle(shine, band.X, band.Y, band.Width, Math.Max(2f, hh * 0.12f));
                using (Pen mesh = new Pen(Color.FromArgb(100, 120, 170, 190), 1f))
                {
                    for (float x = 0; x < band.Width; x += 26 * scale)
                        g.DrawLine(mesh, x, band.Y, x, band.Bottom);
                }
                // reflection
                using (SolidBrush refl = new SolidBrush(Color.FromArgb(40, 10, 40, 60)))
                    g.FillRectangle(refl, band.X, band.Bottom, band.Width, hh * 0.5f);
            }

            // foreground leaves (bottom-left)
            DrawLeaf(g, 40, H - 30, -48, 280, 62, Color.FromArgb(215, 38, 128, 62));
            DrawLeaf(g, 130, H + 5, -22, 250, 56, Color.FromArgb(220, 28, 104, 52));
            DrawLeaf(g, 5, H - 150, -72, 210, 50, Color.FromArgb(205, 46, 140, 70));
            DrawLeaf(g, 90, H - 90, -60, 170, 40, Color.FromArgb(190, 60, 156, 84));
        }

        private void DrawPalm(Graphics g, float x, float baseY, float h)
        {
            using (Pen trunk = new Pen(Color.FromArgb(92, 78, 54), 3f))
                g.DrawLine(trunk, x, baseY, x + 5, baseY - h);
            using (Pen frond = new Pen(Color.FromArgb(34, 104, 56), 3f))
            {
                frond.EndCap = LineCap.Round;
                double[] ang = { -170, -140, -110, -70, -40, -10 };
                foreach (double a in ang)
                {
                    double r = a * Math.PI / 180.0;
                    float ex = x + 5 + (float)Math.Cos(r) * 22;
                    float ey = baseY - h + (float)Math.Sin(r) * 12 + 8;
                    g.DrawLine(frond, x + 5, baseY - h, ex, ey);
                }
            }
        }

        private void DrawLeaf(Graphics g, float x, float y, float angle,
            float w, float h, Color c)
        {
            GraphicsState st = g.Save();
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            using (SolidBrush b = new SolidBrush(c))
                g.FillEllipse(b, 0, -h / 2, w, h);
            g.Restore(st);
        }

        // ---------------- branding ----------------
        private void DrawBranding(Graphics g, int W, int H)
        {
            int x0 = (int)(W * 0.05f);
            int totalW = Math.Max(480, W - 520 - x0 - 30);
            int cx = x0 + totalW / 2;                    // same center as the 4 features
            int top = (int)(H * 0.10f);

            Rectangle lr = Brand.DrawFull(g, new Rectangle(cx - 220, top, 440, 130), false);

            string tag = "   Manage your fish farm with ease.";
            int tw = Gfx.Measure(tag, TagF);
            Gfx.Text(g, tag, TagF, new Point(lr.X + lr.Width / 2 - tw / 2, lr.Bottom + 12), Theme.Navy);
        }

        private void DrawFeatures(Graphics g, int W, int H)
        {
            string[] glyphs = { "\U0001F41F", "\U0001F4B0", "\U0001F33F", "\U0001F3EA" };
            string[] titles = { "Track Production", "Manage Finances", "Ensure Health", "Connect to Buyers" };
            string[] descs =
            {
                "Monitor your fish stock and growth.",
                "Keep your records organized.",
                "Record feed and treatment.",
                "List your harvest in the marketplace."
            };

            int x0 = (int)(W * 0.05f);
            int totalW = Math.Max(480, W - 520 - x0 - 30);
            int colW = totalW / 4;
            int y = (int)(H * 0.10f) + 185;

            Rectangle panel = new Rectangle(x0 + 6, y - 18, totalW - 12, 166);
            using (GraphicsPath gp = Gfx.RoundRect(panel, 22))
            using (SolidBrush fb = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
                g.FillPath(fb, gp);

            for (int i = 0; i < 4; i++)
            {
                int cx = x0 + i * colW + colW / 2;

                Rectangle ic = new Rectangle(cx - 28, y, 56, 56);
                using (SolidBrush b = new SolidBrush(Color.FromArgb(225, 214, 235, 250)))
                    g.FillEllipse(b, ic);
                using (Pen p = new Pen(Color.FromArgb(120, 150, 200, 235), 1f))
                    g.DrawEllipse(p, ic);
                Gfx.TextCenter(g, glyphs[i], EmojiF, ic, Theme.Blue);

                Gfx.TextIn(g, titles[i], FeatTitleF,
                    new Rectangle(cx - colW / 2 + 6, y + 66, colW - 12, 20), Theme.Navy,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine);
                Gfx.TextIn(g, descs[i], FeatDescF,
                    new Rectangle(cx - colW / 2 + 16, y + 88, colW - 32, 40),
                    Color.FromArgb(35, 62, 98),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);

                if (i > 0)
                    using (Pen d = new Pen(Color.FromArgb(110, 90, 130, 170), 1f))
                        g.DrawLine(d, x0 + i * colW, y + 6, x0 + i * colW, y + 120);
            }
        }

        private void DrawFooter(Graphics g, int W, int H)
        {
            string text = "Healthy Fish  \u2022  Sustainable Farms  \u2022  Stronger Communities";
            int leftW = Math.Max(500, W - 520);
            int tw = Gfx.Measure(text, FooterF);
            int x = (leftW - tw) / 2;
            int y = H - 74;

            Gfx.Text(g, text, FooterF, new Point(x + 1, y + 1), Color.FromArgb(120, 0, 30, 60));
            Gfx.Text(g, text, FooterF, new Point(x, y), Color.White);

            using (Pen wp = new Pen(Color.White, 2f))
            {
                wp.StartCap = LineCap.Round; wp.EndCap = LineCap.Round;
                int cx = leftW / 2;
                g.DrawArc(wp, cx - 40, y + 34, 40, 9, 180, 180);
                g.DrawArc(wp, cx, y + 34, 40, 9, 0, 180);
            }
        }
    }

    // ======================================================================
    //  LOGIN CARD (rounded, frosted, with heading + chip + logo)
    // ======================================================================
    internal class LoginCard : CardPanel
    {
        private static readonly Font HeadF = new Font("Segoe UI", 22f, FontStyle.Bold);
        private static readonly Font SubF = new Font("Segoe UI", 9.5f);
        private static readonly Font ChipF = new Font("Segoe UI", 8.5f);
        private static readonly Font LabelF = new Font("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font LockF = new Font("Segoe UI Emoji", 9f);

        public LoginCard()
        {
            Radius = 24;
            FillTop = Color.FromArgb(255, 255, 255);
            FillBottom = Color.FromArgb(236, 246, 254);
            GradientAngle = 90f;
            BackColor = Color.FromArgb(214, 232, 247);
            WaveColor = Color.FromArgb(46, 100, 175, 230);
        }

        protected override void DrawContent(Graphics g, Rectangle c)
        {
            // "Local Login" chip
            Rectangle chip = new Rectangle(c.Right - 18 - 118, c.Y + 14, 118, 28);
            using (GraphicsPath p = Gfx.RoundRect(chip, 14))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(222, 239, 250)))
                g.FillPath(b, p);
            Gfx.TextIn(g, "Local Login", ChipF,
                new Rectangle(chip.X + 12, chip.Y, 72, chip.Height), Theme.Navy,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            Gfx.TextCenter(g, "\U0001F512", LockF,
                new Rectangle(chip.Right - 30, chip.Y, 22, chip.Height), Theme.Navy);

            // logo
            Brand.DrawIcon(g, new Rectangle(c.X + c.Width / 2 - 42, c.Y + 28, 84, 84));

            Gfx.TextIn(g, "Welcome Back!", HeadF,
                new Rectangle(c.X, c.Y + 114, c.Width, 38), Theme.Navy,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine);
            Gfx.TextIn(g, "Sign in to your AquaLedger account.", SubF,
                new Rectangle(c.X, c.Y + 154, c.Width, 20), Theme.Muted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine);

            Gfx.Text(g, "Select Your Role", LabelF, new Point(c.X + 30, c.Y + 330), Theme.Navy);
        }
    }

    // ======================================================================
    //  FIELD BOX (rounded input with icon, caption, placeholder, eye)
    // ======================================================================
    internal enum FieldIcon { User, Lock }

    internal class FieldBox : Panel
    {
        public string Caption = "";
        public FieldIcon Icon = FieldIcon.User;
        public bool HasEye;
        public bool EyeRevealed;
        public event EventHandler EyeClicked;

        private bool focused;
        private TextBox editor;
        private Rectangle eyeRect = Rectangle.Empty;
        private static readonly Color Fill = Color.FromArgb(246, 251, 255);
        private static readonly Font CapF = new Font("Segoe UI", 8.5f);
        private static readonly Font EditF = new Font("Segoe UI", 10f);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public FieldBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        /// <summary>Moves the existing TextBox into this box and styles it.</summary>
        public void AttachEditor(TextBox t, string placeholder)
        {
            editor = t;
            t.BorderStyle = BorderStyle.None;
            t.BackColor = Fill;
            t.ForeColor = Theme.Navy;
            t.Font = EditF;
            t.Multiline = false;
            t.Enter += delegate { focused = true; Invalidate(); };
            t.Leave += delegate { focused = false; Invalidate(); };

            Controls.Add(t);
            PositionEditor();

            EventHandler apply = delegate
            {
                SendMessage(t.Handle, EM_SETCUEBANNER, new IntPtr(1), placeholder);
            };
            t.HandleCreated += apply;
            if (t.IsHandleCreated) apply(t, EventArgs.Empty);
        }

        private void PositionEditor()
        {
            if (editor == null) return;
            int right = HasEye ? 52 : 18;
            editor.Location = new Point(50, 28);
            editor.Width = Math.Max(40, Width - 50 - right);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionEditor();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Cursor = (HasEye && eyeRect.Contains(e.Location)) ? Cursors.Hand : Cursors.Default;
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (HasEye && eyeRect.Contains(e.Location))
            {
                if (EyeClicked != null) EyeClicked(this, EventArgs.Empty);
            }
            else if (editor != null)
            {
                editor.Focus();
            }
            base.OnMouseClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            if (r.Width < 20 || r.Height < 20) return;

            using (GraphicsPath p = Gfx.RoundRect(r, 12))
            {
                using (SolidBrush b = new SolidBrush(Fill)) g.FillPath(b, p);
                Color border = focused ? Color.FromArgb(27, 108, 181) : Color.FromArgb(190, 214, 236);
                using (Pen pen = new Pen(border, focused ? 1.8f : 1f)) g.DrawPath(pen, p);
            }

            Color ic = Color.FromArgb(60, 82, 112);
            if (Icon == FieldIcon.User) DrawUserIcon(g, 16, Height / 2 - 11, ic);
            else DrawLockIcon(g, 16, Height / 2 - 11, ic);

            Gfx.Text(g, Caption, CapF, new Point(50, 8), Color.FromArgb(70, 110, 160));

            if (HasEye)
            {
                eyeRect = new Rectangle(Width - 48, 0, 44, Height);
                DrawEyeIcon(g, Width - 38, Height / 2 - 8, ic, EyeRevealed);
            }
        }

        private static void DrawUserIcon(Graphics g, int x, int y, Color c)
        {
            using (Pen p = new Pen(c, 1.7f))
            {
                g.DrawEllipse(p, x + 5, y, 10, 10);
                g.DrawArc(p, x, y + 12, 20, 18, 200, 140);
            }
        }

        private static void DrawLockIcon(Graphics g, int x, int y, Color c)
        {
            using (Pen p = new Pen(c, 1.7f))
            {
                g.DrawArc(p, x + 4, y, 12, 12, 180, 180);
                g.DrawLine(p, x + 4, y + 6, x + 4, y + 10);
                g.DrawLine(p, x + 16, y + 6, x + 16, y + 10);
                using (GraphicsPath body = Gfx.RoundRect(new Rectangle(x + 1, y + 10, 18, 13), 3))
                    g.DrawPath(p, body);
            }
            using (SolidBrush b = new SolidBrush(c))
                g.FillEllipse(b, x + 8, y + 14, 4, 4);
        }

        private static void DrawEyeIcon(Graphics g, int x, int y, Color c, bool revealed)
        {
            using (Pen p = new Pen(c, 1.7f))
            {
                g.DrawArc(p, x, y - 2, 24, 18, 200, 140);
                g.DrawArc(p, x, y - 2 + 2, 24, 14, 20, 140);
                g.DrawEllipse(p, x + 8, y + 3, 8, 8);
                if (!revealed) { /* plain eye = PIN hidden */ }
                else g.DrawLine(p, x + 2, y + 14, x + 22, y - 2);
            }
        }
    }

    // ======================================================================
    //  ROLE CARD (radio-style selectable card)
    // ======================================================================
    internal class RoleCard : Control
    {
        public string Title = "";
        public string Description = "";
        public bool Farmer;
        private bool selected;
        private bool hover;

        private static readonly Font TitleF = new Font("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font DescF = new Font("Segoe UI", 8f);

        public bool Selected
        {
            get { return selected; }
            set { selected = value; Invalidate(); }
        }

        public RoleCard()
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
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            if (r.Width < 30 || r.Height < 30) return;

            Color fill = selected ? Color.FromArgb(222, 240, 253) :
                         hover ? Color.FromArgb(244, 249, 254) : Color.White;
            Color border = selected ? Color.FromArgb(27, 108, 181) : Color.FromArgb(214, 224, 234);

            using (GraphicsPath p = Gfx.RoundRect(r, 12))
            {
                using (SolidBrush b = new SolidBrush(fill)) g.FillPath(b, p);
                using (Pen pen = new Pen(border, selected ? 1.6f : 1f)) g.DrawPath(pen, p);
            }

            // radio
            Rectangle rd = new Rectangle(Width - 24, 9, 14, 14);
            using (Pen rp = new Pen(selected ? Color.FromArgb(27, 108, 181) : Color.FromArgb(170, 182, 196), 1.5f))
                g.DrawEllipse(rp, rd);
            if (selected)
                using (SolidBrush rb = new SolidBrush(Color.FromArgb(27, 108, 181)))
                    g.FillEllipse(rb, rd.X + 3, rd.Y + 3, 8, 8);

            // person icon
            DrawPerson(g, Width / 2, 14, Farmer);

            Gfx.TextIn(g, Title, TitleF, new Rectangle(4, 52, Width - 8, 18), Theme.Navy,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine);
            Gfx.TextIn(g, Description, DescF, new Rectangle(10, 71, Width - 20, 32),
                Theme.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
        }

        private static void DrawPerson(Graphics g, int cx, int y, bool hat)
        {
            Color c = Color.FromArgb(27, 108, 181);
            int headY = hat ? y + 6 : y + 2;
            using (SolidBrush b = new SolidBrush(c))
            using (Pen p = new Pen(c, 1.7f))
            {
                if (hat)
                {
                    g.FillEllipse(b, cx - 13, headY - 1, 26, 6);       // brim
                    g.FillEllipse(b, cx - 7, headY - 8, 14, 10);       // crown
                    using (SolidBrush white = new SolidBrush(Color.White))
                        g.FillEllipse(white, cx - 6, headY + 4, 12, 12);
                    g.DrawEllipse(p, cx - 6, headY + 4, 12, 12);
                }
                else
                {
                    g.DrawEllipse(p, cx - 7, headY, 14, 14);
                }
                g.SetClip(new Rectangle(cx - 16, headY + 18, 32, 14));
                g.DrawEllipse(p, cx - 13, headY + 18, 26, 22);
                g.ResetClip();
            }
        }
    }

    // ======================================================================
    //  PRIMARY BUTTON ("Log In  →")
    // ======================================================================
    internal class PrimaryButton : Control
    {
        private bool hover, down;
        private static readonly Font BtnF = new Font("Segoe UI", 11f, FontStyle.Bold);

        public PrimaryButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor | ControlStyles.StandardClick, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { down = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { down = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            if (r.Width < 20 || r.Height < 20) return;

            Color c = Color.FromArgb(27, 108, 181);
            if (!Enabled) c = Color.FromArgb(160, 180, 200);
            else if (down) c = Color.FromArgb(20, 88, 150);
            else if (hover) c = Color.FromArgb(38, 124, 202);

            using (GraphicsPath p = Gfx.RoundRect(r, 10))
            using (LinearGradientBrush b = new LinearGradientBrush(r,
                ControlPaint.Light(c, 0.1f), c, 90f))
                g.FillPath(b, p);

            Gfx.TextCenter(g, Text + "   \u2192", BtnF, r, Color.White);
        }
    }
}