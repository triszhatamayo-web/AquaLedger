namespace AQUALEDGERS
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTagline = new System.Windows.Forms.Label();
            this.pnlBrand = new System.Windows.Forms.Panel();
            this.pnlLogin = new System.Windows.Forms.Panel();
            this.btnSignUp = new System.Windows.Forms.Button();
            this.txtLogin = new System.Windows.Forms.Button();
            this.lblPIN = new System.Windows.Forms.Label();
            this.txtPIN = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.cmbAccountType = new System.Windows.Forms.ComboBox();
            this.lblAccountType = new System.Windows.Forms.Label();
            this.lblLoginSubtitle = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.pnlBrand.SuspendLayout();
            this.pnlLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTitle.Image = global::AQUALEDGERS.Properties.Resources.logo_full;
            this.lblTitle.Location = new System.Drawing.Point(134, 256);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(496, 112);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "AquaLedger";
            // 
            // lblTagline
            // 
            this.lblTagline.AutoSize = true;
            this.lblTagline.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTagline.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblTagline.Location = new System.Drawing.Point(3, 379);
            this.lblTagline.Name = "lblTagline";
            this.lblTagline.Size = new System.Drawing.Size(600, 48);
            this.lblTagline.TabIndex = 1;
            this.lblTagline.Text = "Smarter  Records. Healthier Records.";
            // 
            // pnlBrand
            // 
            this.pnlBrand.BackColor = System.Drawing.Color.LightSteelBlue;
            this.pnlBrand.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlBrand.BackgroundImage")));
            this.pnlBrand.Controls.Add(this.pnlLogin);
            this.pnlBrand.Controls.Add(this.lblTagline);
            this.pnlBrand.Controls.Add(this.lblTitle);
            this.pnlBrand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBrand.Location = new System.Drawing.Point(0, 0);
            this.pnlBrand.Name = "pnlBrand";
            this.pnlBrand.Size = new System.Drawing.Size(2107, 1042);
            this.pnlBrand.TabIndex = 8;
            // 
            // pnlLogin
            // 
            this.pnlLogin.BackColor = System.Drawing.Color.White;
            this.pnlLogin.Controls.Add(this.btnSignUp);
            this.pnlLogin.Controls.Add(this.txtLogin);
            this.pnlLogin.Controls.Add(this.lblPIN);
            this.pnlLogin.Controls.Add(this.txtPIN);
            this.pnlLogin.Controls.Add(this.txtUsername);
            this.pnlLogin.Controls.Add(this.lblUsername);
            this.pnlLogin.Controls.Add(this.cmbAccountType);
            this.pnlLogin.Controls.Add(this.lblAccountType);
            this.pnlLogin.Controls.Add(this.lblLoginSubtitle);
            this.pnlLogin.Controls.Add(this.lblWelcome);
            this.pnlLogin.Location = new System.Drawing.Point(859, -1);
            this.pnlLogin.Name = "pnlLogin";
            this.pnlLogin.Size = new System.Drawing.Size(1236, 938);
            this.pnlLogin.TabIndex = 9;
            // 
            // btnSignUp
            // 
            this.btnSignUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSignUp.ForeColor = System.Drawing.Color.DarkGreen;
            this.btnSignUp.Location = new System.Drawing.Point(75, 813);
            this.btnSignUp.Name = "btnSignUp";
            this.btnSignUp.Size = new System.Drawing.Size(1056, 46);
            this.btnSignUp.TabIndex = 9;
            this.btnSignUp.Text = "Sign Up";
            this.btnSignUp.UseVisualStyleBackColor = true;
            this.btnSignUp.Click += new System.EventHandler(this.btnSignUp_Click);
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(75, 733);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(1056, 49);
            this.txtLogin.TabIndex = 8;
            this.txtLogin.Text = "LOG IN";
            this.txtLogin.UseVisualStyleBackColor = true;
            this.txtLogin.Click += new System.EventHandler(this.txtLogin_Click);
            // 
            // lblPIN
            // 
            this.lblPIN.AutoSize = true;
            this.lblPIN.Location = new System.Drawing.Point(70, 526);
            this.lblPIN.Name = "lblPIN";
            this.lblPIN.Size = new System.Drawing.Size(53, 29);
            this.lblPIN.TabIndex = 7;
            this.lblPIN.Text = "PIN";
            // 
            // txtPIN
            // 
            this.txtPIN.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPIN.Location = new System.Drawing.Point(75, 574);
            this.txtPIN.MaxLength = 6;
            this.txtPIN.Name = "txtPIN";
            this.txtPIN.Size = new System.Drawing.Size(1056, 55);
            this.txtPIN.TabIndex = 6;
            this.txtPIN.UseSystemPasswordChar = true;
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(75, 440);
            this.txtUsername.MaxLength = 50;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(1056, 55);
            this.txtUsername.TabIndex = 5;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(70, 398);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(124, 29);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "Username";
            // 
            // cmbAccountType
            // 
            this.cmbAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccountType.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAccountType.FormattingEnabled = true;
            this.cmbAccountType.Items.AddRange(new object[] {
            "Cooperatives",
            "Buyer"});
            this.cmbAccountType.Location = new System.Drawing.Point(75, 308);
            this.cmbAccountType.Name = "cmbAccountType";
            this.cmbAccountType.Size = new System.Drawing.Size(1056, 53);
            this.cmbAccountType.TabIndex = 3;
            // 
            // lblAccountType
            // 
            this.lblAccountType.AutoSize = true;
            this.lblAccountType.Location = new System.Drawing.Point(70, 265);
            this.lblAccountType.Name = "lblAccountType";
            this.lblAccountType.Size = new System.Drawing.Size(159, 29);
            this.lblAccountType.TabIndex = 2;
            this.lblAccountType.Text = "Account Type";
            // 
            // lblLoginSubtitle
            // 
            this.lblLoginSubtitle.AutoSize = true;
            this.lblLoginSubtitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoginSubtitle.ForeColor = System.Drawing.Color.DimGray;
            this.lblLoginSubtitle.Location = new System.Drawing.Point(420, 150);
            this.lblLoginSubtitle.Name = "lblLoginSubtitle";
            this.lblLoginSubtitle.Size = new System.Drawing.Size(479, 48);
            this.lblLoginSubtitle.TabIndex = 1;
            this.lblLoginSubtitle.Text = "Please log in to your account";
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblWelcome.Location = new System.Drawing.Point(203, 37);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(861, 96);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Welcome to Aqualedger";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2107, 1042);
            this.Controls.Add(this.pnlBrand);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AquaLedger - Login";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlBrand.ResumeLayout(false);
            this.pnlBrand.PerformLayout();
            this.pnlLogin.ResumeLayout(false);
            this.pnlLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTagline;
        private System.Windows.Forms.Panel pnlBrand;
        private System.Windows.Forms.Panel pnlLogin;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblLoginSubtitle;
        private System.Windows.Forms.ComboBox cmbAccountType;
        private System.Windows.Forms.Label lblAccountType;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPIN;
        private System.Windows.Forms.TextBox txtPIN;
        private System.Windows.Forms.Button txtLogin;
        private System.Windows.Forms.Button btnSignUp;
    }
}

