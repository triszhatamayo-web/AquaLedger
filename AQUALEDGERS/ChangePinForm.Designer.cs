namespace AQUALEDGERS
{
    partial class ChangePinForm
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
            this.lblCurrentPin = new System.Windows.Forms.Label();
            this.txtCurrentPin = new System.Windows.Forms.TextBox();
            this.lblNewPin = new System.Windows.Forms.Label();
            this.lblConfirmPin = new System.Windows.Forms.Label();
            this.txtConfirmPin = new System.Windows.Forms.TextBox();
            this.txtNewPin = new System.Windows.Forms.TextBox();
            this.btnSavePin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCurrentPin
            // 
            this.lblCurrentPin.AutoSize = true;
            this.lblCurrentPin.Location = new System.Drawing.Point(30, 35);
            this.lblCurrentPin.Name = "lblCurrentPin";
            this.lblCurrentPin.Size = new System.Drawing.Size(138, 29);
            this.lblCurrentPin.TabIndex = 0;
            this.lblCurrentPin.Text = "Current PIN";
            // 
            // txtCurrentPin
            // 
            this.txtCurrentPin.Location = new System.Drawing.Point(150, 30);
            this.txtCurrentPin.Name = "txtCurrentPin";
            this.txtCurrentPin.Size = new System.Drawing.Size(200, 35);
            this.txtCurrentPin.TabIndex = 1;
            this.txtCurrentPin.UseSystemPasswordChar = true;
            // 
            // lblNewPin
            // 
            this.lblNewPin.AutoSize = true;
            this.lblNewPin.Location = new System.Drawing.Point(30, 85);
            this.lblNewPin.Name = "lblNewPin";
            this.lblNewPin.Size = new System.Drawing.Size(109, 29);
            this.lblNewPin.TabIndex = 2;
            this.lblNewPin.Text = "New PIN";
            // 
            // lblConfirmPin
            // 
            this.lblConfirmPin.AutoSize = true;
            this.lblConfirmPin.Location = new System.Drawing.Point(30, 135);
            this.lblConfirmPin.Name = "lblConfirmPin";
            this.lblConfirmPin.Size = new System.Drawing.Size(199, 29);
            this.lblConfirmPin.TabIndex = 3;
            this.lblConfirmPin.Text = "Confirm New PIN";
            // 
            // txtConfirmPin
            // 
            this.txtConfirmPin.Location = new System.Drawing.Point(150, 130);
            this.txtConfirmPin.Name = "txtConfirmPin";
            this.txtConfirmPin.Size = new System.Drawing.Size(200, 35);
            this.txtConfirmPin.TabIndex = 4;
            this.txtConfirmPin.UseSystemPasswordChar = true;
            // 
            // txtNewPin
            // 
            this.txtNewPin.Location = new System.Drawing.Point(150, 80);
            this.txtNewPin.Name = "txtNewPin";
            this.txtNewPin.Size = new System.Drawing.Size(200, 35);
            this.txtNewPin.TabIndex = 5;
            this.txtNewPin.UseSystemPasswordChar = true;
            // 
            // btnSavePin
            // 
            this.btnSavePin.Location = new System.Drawing.Point(150, 190);
            this.btnSavePin.Name = "btnSavePin";
            this.btnSavePin.Size = new System.Drawing.Size(125, 35);
            this.btnSavePin.TabIndex = 6;
            this.btnSavePin.Text = "Save PIN";
            this.btnSavePin.UseVisualStyleBackColor = true;
            this.btnSavePin.Click += new System.EventHandler(this.btnSavePin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(280, 190);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
            // 
            // ChangePinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 271);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSavePin);
            this.Controls.Add(this.txtNewPin);
            this.Controls.Add(this.txtConfirmPin);
            this.Controls.Add(this.lblConfirmPin);
            this.Controls.Add(this.lblNewPin);
            this.Controls.Add(this.txtCurrentPin);
            this.Controls.Add(this.lblCurrentPin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChangePin - AquaLedger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCurrentPin;
        private System.Windows.Forms.TextBox txtCurrentPin;
        private System.Windows.Forms.Label lblNewPin;
        private System.Windows.Forms.Label lblConfirmPin;
        private System.Windows.Forms.TextBox txtConfirmPin;
        private System.Windows.Forms.TextBox txtNewPin;
        private System.Windows.Forms.Button btnSavePin;
        private System.Windows.Forms.Button btnCancel;
    }
}