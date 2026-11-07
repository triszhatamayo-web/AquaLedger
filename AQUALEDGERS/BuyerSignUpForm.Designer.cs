namespace AQUALEDGERS
{
    partial class BuyerSignUpForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPIN = new System.Windows.Forms.Label();
            this.txtPIN = new System.Windows.Forms.TextBox();
            this.lblConfirmPIN = new System.Windows.Forms.Label();
            this.txtConfirmPIN = new System.Windows.Forms.TextBox();
            this.btnCreateAccount = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlBrand = new System.Windows.Forms.Panel();
            this.pnlRegistration = new System.Windows.Forms.Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.lblTagline = new System.Windows.Forms.Label();
            this.lblsubtagline = new System.Windows.Forms.Label();
            this.pnlBrand.SuspendLayout();
            this.pnlRegistration.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTitle.Location = new System.Drawing.Point(145, 25);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(776, 96);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Create Buyer Account";
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(156, 184);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(124, 29);
            this.lblFullName.TabIndex = 1;
            this.lblFullName.Text = "Full Name";
            // 
            // txtFullName
            // 
            this.txtFullName.Location = new System.Drawing.Point(161, 233);
            this.txtFullName.MaxLength = 100;
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(668, 35);
            this.txtFullName.TabIndex = 2;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(156, 309);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(124, 29);
            this.lblUsername.TabIndex = 3;
            this.lblUsername.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(161, 359);
            this.txtUsername.MaxLength = 50;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(668, 35);
            this.txtUsername.TabIndex = 4;
            // 
            // lblPIN
            // 
            this.lblPIN.AutoSize = true;
            this.lblPIN.Location = new System.Drawing.Point(156, 425);
            this.lblPIN.Name = "lblPIN";
            this.lblPIN.Size = new System.Drawing.Size(53, 29);
            this.lblPIN.TabIndex = 5;
            this.lblPIN.Text = "PIN";
            // 
            // txtPIN
            // 
            this.txtPIN.Location = new System.Drawing.Point(161, 470);
            this.txtPIN.MaxLength = 6;
            this.txtPIN.Name = "txtPIN";
            this.txtPIN.Size = new System.Drawing.Size(668, 35);
            this.txtPIN.TabIndex = 6;
            this.txtPIN.UseSystemPasswordChar = true;
            // 
            // lblConfirmPIN
            // 
            this.lblConfirmPIN.AutoSize = true;
            this.lblConfirmPIN.Location = new System.Drawing.Point(156, 535);
            this.lblConfirmPIN.Name = "lblConfirmPIN";
            this.lblConfirmPIN.Size = new System.Drawing.Size(143, 29);
            this.lblConfirmPIN.TabIndex = 7;
            this.lblConfirmPIN.Text = "Confirm PIN";
            // 
            // txtConfirmPIN
            // 
            this.txtConfirmPIN.Location = new System.Drawing.Point(161, 583);
            this.txtConfirmPIN.MaxLength = 6;
            this.txtConfirmPIN.Name = "txtConfirmPIN";
            this.txtConfirmPIN.Size = new System.Drawing.Size(668, 35);
            this.txtConfirmPIN.TabIndex = 8;
            this.txtConfirmPIN.UseSystemPasswordChar = true;
            // 
            // btnCreateAccount
            // 
            this.btnCreateAccount.BackColor = System.Drawing.Color.DarkGreen;
            this.btnCreateAccount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateAccount.ForeColor = System.Drawing.Color.White;
            this.btnCreateAccount.Location = new System.Drawing.Point(161, 658);
            this.btnCreateAccount.Name = "btnCreateAccount";
            this.btnCreateAccount.Size = new System.Drawing.Size(668, 45);
            this.btnCreateAccount.TabIndex = 9;
            this.btnCreateAccount.Text = "Create Account";
            this.btnCreateAccount.UseVisualStyleBackColor = false;
            this.btnCreateAccount.Click += new System.EventHandler(this.btnCreateAccount_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.DarkGreen;
            this.btnCancel.Location = new System.Drawing.Point(161, 731);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(668, 45);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlBrand
            // 
            this.pnlBrand.BackColor = System.Drawing.Color.LightBlue;
            this.pnlBrand.Controls.Add(this.lblTagline);
            this.pnlBrand.Controls.Add(this.lblBrand);
            this.pnlBrand.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlBrand.Location = new System.Drawing.Point(0, 0);
            this.pnlBrand.Name = "pnlBrand";
            this.pnlBrand.Size = new System.Drawing.Size(420, 981);
            this.pnlBrand.TabIndex = 11;
            // 
            // pnlRegistration
            // 
            this.pnlRegistration.Controls.Add(this.lblsubtagline);
            this.pnlRegistration.Controls.Add(this.btnCancel);
            this.pnlRegistration.Controls.Add(this.lblTitle);
            this.pnlRegistration.Controls.Add(this.btnCreateAccount);
            this.pnlRegistration.Controls.Add(this.lblFullName);
            this.pnlRegistration.Controls.Add(this.txtConfirmPIN);
            this.pnlRegistration.Controls.Add(this.txtFullName);
            this.pnlRegistration.Controls.Add(this.lblConfirmPIN);
            this.pnlRegistration.Controls.Add(this.lblUsername);
            this.pnlRegistration.Controls.Add(this.txtPIN);
            this.pnlRegistration.Controls.Add(this.txtUsername);
            this.pnlRegistration.Controls.Add(this.lblPIN);
            this.pnlRegistration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRegistration.Location = new System.Drawing.Point(420, 0);
            this.pnlRegistration.Name = "pnlRegistration";
            this.pnlRegistration.Size = new System.Drawing.Size(1006, 981);
            this.pnlRegistration.TabIndex = 12;
            // 
            // lblBrand
            // 
            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrand.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblBrand.Location = new System.Drawing.Point(-19, 193);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(524, 112);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "AquaLedger";
            // 
            // lblTagline
            // 
            this.lblTagline.AutoSize = true;
            this.lblTagline.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTagline.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblTagline.Location = new System.Drawing.Point(12, 335);
            this.lblTagline.Name = "lblTagline";
            this.lblTagline.Size = new System.Drawing.Size(541, 40);
            this.lblTagline.TabIndex = 1;
            this.lblTagline.Text = "Smarter Records. Healthier Fish.";
            // 
            // lblsubtagline
            // 
            this.lblsubtagline.AutoSize = true;
            this.lblsubtagline.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblsubtagline.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblsubtagline.Location = new System.Drawing.Point(138, 121);
            this.lblsubtagline.Name = "lblsubtagline";
            this.lblsubtagline.Size = new System.Drawing.Size(835, 40);
            this.lblsubtagline.TabIndex = 11;
            this.lblsubtagline.Text = "Join AquaLedger and start buying fresh quality fish.";
            // 
            // BuyerSignUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1426, 981);
            this.Controls.Add(this.pnlRegistration);
            this.Controls.Add(this.pnlBrand);
            this.Name = "BuyerSignUpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AquaLedger - Buyer Sign Up";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlBrand.ResumeLayout(false);
            this.pnlBrand.PerformLayout();
            this.pnlRegistration.ResumeLayout(false);
            this.pnlRegistration.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPIN;
        private System.Windows.Forms.TextBox txtPIN;
        private System.Windows.Forms.Label lblConfirmPIN;
        private System.Windows.Forms.TextBox txtConfirmPIN;
        private System.Windows.Forms.Button btnCreateAccount;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlBrand;
        private System.Windows.Forms.Label lblTagline;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Panel pnlRegistration;
        private System.Windows.Forms.Label lblsubtagline;
    }
}