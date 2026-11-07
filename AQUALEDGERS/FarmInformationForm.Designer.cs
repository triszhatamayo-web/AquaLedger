namespace AQUALEDGERS
{
    partial class FarmInformationForm
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
            this.dgvFarms = new System.Windows.Forms.DataGridView();
            this.lblFarmInformationTitle = new System.Windows.Forms.Label();
            this.btnSelectFarm = new System.Windows.Forms.Button();
            this.btnEditFarm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFarms)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvFarms
            // 
            this.dgvFarms.AllowUserToAddRows = false;
            this.dgvFarms.AllowUserToDeleteRows = false;
            this.dgvFarms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFarms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFarms.Location = new System.Drawing.Point(20, 80);
            this.dgvFarms.MultiSelect = false;
            this.dgvFarms.Name = "dgvFarms";
            this.dgvFarms.ReadOnly = true;
            this.dgvFarms.RowHeadersWidth = 92;
            this.dgvFarms.RowTemplate.Height = 37;
            this.dgvFarms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFarms.Size = new System.Drawing.Size(740, 300);
            this.dgvFarms.TabIndex = 0;
            // 
            // lblFarmInformationTitle
            // 
            this.lblFarmInformationTitle.AutoSize = true;
            this.lblFarmInformationTitle.Location = new System.Drawing.Point(20, 20);
            this.lblFarmInformationTitle.Name = "lblFarmInformationTitle";
            this.lblFarmInformationTitle.Size = new System.Drawing.Size(281, 29);
            this.lblFarmInformationTitle.TabIndex = 1;
            this.lblFarmInformationTitle.Text = "Select a Farm to Manage";
            // 
            // btnSelectFarm
            // 
            this.btnSelectFarm.Location = new System.Drawing.Point(610, 400);
            this.btnSelectFarm.Name = "btnSelectFarm";
            this.btnSelectFarm.Size = new System.Drawing.Size(150, 35);
            this.btnSelectFarm.TabIndex = 2;
            this.btnSelectFarm.Text = "Select Farm";
            this.btnSelectFarm.UseVisualStyleBackColor = true;
            this.btnSelectFarm.Click += new System.EventHandler(this.btnSelectFarm_Click);
            // 
            // btnEditFarm
            // 
            this.btnEditFarm.Location = new System.Drawing.Point(440, 400);
            this.btnEditFarm.Name = "btnEditFarm";
            this.btnEditFarm.Size = new System.Drawing.Size(150, 35);
            this.btnEditFarm.TabIndex = 3;
            this.btnEditFarm.Text = "Edit Selected Farm";
            this.btnEditFarm.UseVisualStyleBackColor = true;
            this.btnEditFarm.Click += new System.EventHandler(this.btnEditFarm_Click);
            // 
            // FarmInformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 450);
            this.Controls.Add(this.btnEditFarm);
            this.Controls.Add(this.btnSelectFarm);
            this.Controls.Add(this.lblFarmInformationTitle);
            this.Controls.Add(this.dgvFarms);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FarmInformationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Farm Information";
            this.Load += new System.EventHandler(this.FarmInformationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFarms)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFarms;
        private System.Windows.Forms.Label lblFarmInformationTitle;
        private System.Windows.Forms.Button btnSelectFarm;
        private System.Windows.Forms.Button btnEditFarm;
    }
}