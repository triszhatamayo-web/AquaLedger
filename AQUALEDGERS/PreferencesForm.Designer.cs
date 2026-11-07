namespace AQUALEDGERS
{
    partial class PreferencesForm
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
            this.lblCurrency = new System.Windows.Forms.Label();
            this.cmbCurrency = new System.Windows.Forms.ComboBox();
            this.lblDataFormat = new System.Windows.Forms.Label();
            this.cmbDateFormat = new System.Windows.Forms.ComboBox();
            this.btnSavePreferences = new System.Windows.Forms.Button();
            this.btnCancelPreferences = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCurrency
            // 
            this.lblCurrency.AutoSize = true;
            this.lblCurrency.Location = new System.Drawing.Point(30, 40);
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Size = new System.Drawing.Size(109, 29);
            this.lblCurrency.TabIndex = 0;
            this.lblCurrency.Text = "Currency";
            // 
            // cmbCurrency
            // 
            this.cmbCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurrency.FormattingEnabled = true;
            this.cmbCurrency.Location = new System.Drawing.Point(150, 35);
            this.cmbCurrency.Name = "cmbCurrency";
            this.cmbCurrency.Size = new System.Drawing.Size(240, 37);
            this.cmbCurrency.TabIndex = 1;
            // 
            // lblDataFormat
            // 
            this.lblDataFormat.AutoSize = true;
            this.lblDataFormat.Location = new System.Drawing.Point(30, 100);
            this.lblDataFormat.Name = "lblDataFormat";
            this.lblDataFormat.Size = new System.Drawing.Size(151, 29);
            this.lblDataFormat.TabIndex = 2;
            this.lblDataFormat.Text = "Date Format:";
            // 
            // cmbDateFormat
            // 
            this.cmbDateFormat.FormattingEnabled = true;
            this.cmbDateFormat.Location = new System.Drawing.Point(150, 95);
            this.cmbDateFormat.Name = "cmbDateFormat";
            this.cmbDateFormat.Size = new System.Drawing.Size(240, 37);
            this.cmbDateFormat.TabIndex = 3;
            // 
            // btnSavePreferences
            // 
            this.btnSavePreferences.Location = new System.Drawing.Point(150, 170);
            this.btnSavePreferences.Name = "btnSavePreferences";
            this.btnSavePreferences.Size = new System.Drawing.Size(140, 35);
            this.btnSavePreferences.TabIndex = 4;
            this.btnSavePreferences.Text = "Save Preferences";
            this.btnSavePreferences.UseVisualStyleBackColor = true;
            this.btnSavePreferences.Click += new System.EventHandler(this.btnSavePreferences_Click);
            // 
            // btnCancelPreferences
            // 
            this.btnCancelPreferences.Location = new System.Drawing.Point(300, 170);
            this.btnCancelPreferences.Name = "btnCancelPreferences";
            this.btnCancelPreferences.Size = new System.Drawing.Size(90, 35);
            this.btnCancelPreferences.TabIndex = 5;
            this.btnCancelPreferences.Text = "Cancel";
            this.btnCancelPreferences.UseVisualStyleBackColor = true;
            this.btnCancelPreferences.Click += new System.EventHandler(this.btnCancelPreferences_Click);
            // 
            // PreferencesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 271);
            this.Controls.Add(this.btnCancelPreferences);
            this.Controls.Add(this.btnSavePreferences);
            this.Controls.Add(this.cmbDateFormat);
            this.Controls.Add(this.lblDataFormat);
            this.Controls.Add(this.cmbCurrency);
            this.Controls.Add(this.lblCurrency);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PreferencesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCurrency;
        private System.Windows.Forms.ComboBox cmbCurrency;
        private System.Windows.Forms.Label lblDataFormat;
        private System.Windows.Forms.ComboBox cmbDateFormat;
        private System.Windows.Forms.Button btnSavePreferences;
        private System.Windows.Forms.Button btnCancelPreferences;
    }
}