namespace AQUALEDGERS
{
    partial class EditFarmForm
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
            this.lblFarmName = new System.Windows.Forms.Label();
            this.txtFarmName = new System.Windows.Forms.TextBox();
            this.lblFarmAddress = new System.Windows.Forms.Label();
            this.txtFarmAddress = new System.Windows.Forms.TextBox();
            this.btnSaveFarm = new System.Windows.Forms.Button();
            this.btnCancelFarm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFarmName
            // 
            this.lblFarmName.AutoSize = true;
            this.lblFarmName.Location = new System.Drawing.Point(30, 40);
            this.lblFarmName.Name = "lblFarmName";
            this.lblFarmName.Size = new System.Drawing.Size(146, 29);
            this.lblFarmName.TabIndex = 0;
            this.lblFarmName.Text = "Farm Name:";
            // 
            // txtFarmName
            // 
            this.txtFarmName.Location = new System.Drawing.Point(150, 35);
            this.txtFarmName.Name = "txtFarmName";
            this.txtFarmName.Size = new System.Drawing.Size(240, 35);
            this.txtFarmName.TabIndex = 1;
            // 
            // lblFarmAddress
            // 
            this.lblFarmAddress.AutoSize = true;
            this.lblFarmAddress.Location = new System.Drawing.Point(30, 100);
            this.lblFarmAddress.Name = "lblFarmAddress";
            this.lblFarmAddress.Size = new System.Drawing.Size(108, 29);
            this.lblFarmAddress.TabIndex = 2;
            this.lblFarmAddress.Text = "Address:";
            // 
            // txtFarmAddress
            // 
            this.txtFarmAddress.Location = new System.Drawing.Point(150, 95);
            this.txtFarmAddress.Name = "txtFarmAddress";
            this.txtFarmAddress.Size = new System.Drawing.Size(240, 35);
            this.txtFarmAddress.TabIndex = 3;
            // 
            // btnSaveFarm
            // 
            this.btnSaveFarm.Location = new System.Drawing.Point(150, 180);
            this.btnSaveFarm.Name = "btnSaveFarm";
            this.btnSaveFarm.Size = new System.Drawing.Size(125, 35);
            this.btnSaveFarm.TabIndex = 4;
            this.btnSaveFarm.Text = "Save Changes";
            this.btnSaveFarm.UseVisualStyleBackColor = true;
            this.btnSaveFarm.Click += new System.EventHandler(this.btnSaveFarm_Click);
            // 
            // btnCancelFarm
            // 
            this.btnCancelFarm.Location = new System.Drawing.Point(280, 180);
            this.btnCancelFarm.Name = "btnCancelFarm";
            this.btnCancelFarm.Size = new System.Drawing.Size(110, 35);
            this.btnCancelFarm.TabIndex = 5;
            this.btnCancelFarm.Text = "Cancel";
            this.btnCancelFarm.UseVisualStyleBackColor = true;
            this.btnCancelFarm.Click += new System.EventHandler(this.btnCancelFarm_Click);
            // 
            // EditFarmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 271);
            this.Controls.Add(this.btnCancelFarm);
            this.Controls.Add(this.btnSaveFarm);
            this.Controls.Add(this.txtFarmAddress);
            this.Controls.Add(this.lblFarmAddress);
            this.Controls.Add(this.txtFarmName);
            this.Controls.Add(this.lblFarmName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditFarmForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Farm Information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFarmName;
        private System.Windows.Forms.TextBox txtFarmName;
        private System.Windows.Forms.Label lblFarmAddress;
        private System.Windows.Forms.TextBox txtFarmAddress;
        private System.Windows.Forms.Button btnSaveFarm;
        private System.Windows.Forms.Button btnCancelFarm;
    }
}