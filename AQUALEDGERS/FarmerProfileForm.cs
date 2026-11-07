
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using AQUALEDGERS.Services;

namespace AQUALEDGERS
{
    public partial class FarmerProfileForm : Form
    {
        private readonly int farmerId;

        private TextBox txtFullName;
        private TextBox txtPhone;
        private TextBox txtAddress;

        private Label lblFarmerIdValue;
        private Label lblDateRegisteredValue;
        private Label lblStatusValue;

        private Button btnEdit;
        private Button btnSave;
        private Button btnCancel;

        private PictureBox picFarmerPhoto;
        private Button btnChangePhoto;
        private string currentPhotoPath;

        private bool isEditing = false;

        public FarmerProfileForm(int farmerId)
        {
            InitializeComponent();

            this.farmerId = farmerId;

            Text = "AquaLedger - Farmer Profile";
            Size = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            BuildProfileLayout();
            LoadFarmerProfile();
            SetEditing(false);
        }

        private void BuildProfileLayout()
        {
            Label title = new Label
            {
                Text = "Farmer Profile",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 100, 75),
                Location = new Point(30, 20),
                AutoSize = true
            };
            Controls.Add(title);

            lblFarmerIdValue = AddReadOnlyDetail(
                "Farmer ID:", 75);

            txtFullName = AddTextBox(
                "Full Name:", 120, 430);

            txtPhone = AddTextBox(
                "Phone Number:", 170, 430);

            txtAddress = AddTextBox(
                "Address:", 220, 430);

            lblDateRegisteredValue = AddReadOnlyDetail(
                "Date Registered:", 270);

            lblStatusValue = AddReadOnlyDetail(
                "Status:", 320);

            btnEdit = new Button
            {
                Text = "Edit Farmer",
                Location = new Point(40, 390),
                Size = new Size(140, 38),
                BackColor = Color.FromArgb(30, 100, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;
            Controls.Add(btnEdit);

            btnSave = new Button
            {
                Text = "Save Changes",
                Location = new Point(190, 390),
                Size = new Size(140, 38),
                BackColor = Color.FromArgb(30, 100, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };


            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(340, 390),
                Size = new Size(110, 38),
                BackColor = Color.FromArgb(235, 238, 235),
                ForeColor = Color.FromArgb(40, 60, 45),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;
            Controls.Add(btnCancel);

            picFarmerPhoto = new PictureBox
            {
                Location = new Point(680, 85),
                Size = new Size(160, 160),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 247, 245),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            Controls.Add(picFarmerPhoto);

            btnChangePhoto = new Button
            {
                Text = "Change Photo",
                Location = new Point(680, 260),
                Size = new Size(160, 38),
                BackColor = Color.FromArgb(30, 100, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnChangePhoto.FlatAppearance.BorderSize = 0;
            btnChangePhoto.Click += BtnChangePhoto_Click;
            Controls.Add(btnChangePhoto);
        }

        private TextBox AddTextBox(
            string labelText, int y, int width)
        {
            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(40, y + 4),
                Size = new Size(150, 30)
            };
            Controls.Add(label);

            TextBox textBox = new TextBox
            {
                Location = new Point(200, y),
                Size = new Size(width, 30),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true
            };
            Controls.Add(textBox);

            return textBox;
        }

        private Label AddReadOnlyDetail(
            string labelText, int y)
        {
            Label label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(40, y + 4),
                Size = new Size(150, 30)
            };
            Controls.Add(label);

            Label valueLabel = new Label
            {
                Text = "",
                Location = new Point(200, y),
                Size = new Size(430, 30),
                AutoEllipsis = true
            };
            Controls.Add(valueLabel);

            return valueLabel;
        }

        private void LoadFarmerProfile()
        {
            try
            {
                using (var connection =
                    AQUALEDGERS.Data.Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, FullName, Phone, Address,
                        DateRegistered, IsActive, PhotoPath
                        FROM Farmers
                        WHERE Id = @Id;";

                    command.Parameters.AddWithValue ("@Id", farmerId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show(
                                "Farmer record was not found.",
                                "AquaLedger",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            Close();
                            return;
                        }

                        lblFarmerIdValue.Text =
                        Convert.ToInt32(reader["Id"]).ToString("D3");                

                        txtFullName.Text =
                            reader["FullName"].ToString();

                        txtPhone.Text =
                            reader["Phone"].ToString();

                        txtAddress.Text =
                            reader["Address"].ToString();

                        lblDateRegisteredValue.Text =
                            reader["DateRegistered"] == DBNull.Value
                                ? "Not available"
                                : reader["DateRegistered"].ToString();

                        currentPhotoPath =
                        reader["PhotoPath"] == DBNull.Value
                        ? null
                        : reader["PhotoPath"].ToString();

                        DisplayFarmerPhoto(currentPhotoPath);

                        lblStatusValue.Text =
                            Convert.ToInt32(reader["IsActive"]) == 1
                                ? "Active"
                                : "Inactive";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load farmer profile:\n" + ex.Message,
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SetEditing(bool editing)
        {
            isEditing = editing;

            txtFullName.ReadOnly = !editing;
            txtPhone.ReadOnly = !editing;
            txtAddress.ReadOnly = !editing;

            btnEdit.Visible = !editing;
            btnSave.Visible = editing;
            btnCancel.Visible = editing;

            txtFullName.BackColor = editing
                ? Color.White
                : Color.FromArgb(245, 247, 245);

            txtPhone.BackColor = editing
                ? Color.White
                : Color.FromArgb(245, 247, 245);

            txtAddress.BackColor = editing
                ? Color.White
                : Color.FromArgb(245, 247, 245);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            SetEditing(true);
            txtFullName.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show(
                    "Please enter the farmer's full name.",
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtFullName.Focus();
                return;
            }

            DialogResult answer = MessageBox.Show(
                "Save the changes to this farmer's profile?",
                "Confirm Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            try
            {
                using (var connection =
                    AQUALEDGERS.Data.Db.GetConnection())
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Farmers
                        SET FullName = @FullName,
                            Phone = @Phone,
                            Address = @Address
                        WHERE Id = @Id;";

                    command.Parameters.AddWithValue(
                        "@FullName", txtFullName.Text.Trim());

                    command.Parameters.AddWithValue(
                        "@Phone",
                        string.IsNullOrWhiteSpace(txtPhone.Text)
                            ? (object)DBNull.Value
                            : txtPhone.Text.Trim());

                    command.Parameters.AddWithValue(
                        "@Address",
                        string.IsNullOrWhiteSpace(txtAddress.Text)
                            ? (object)DBNull.Value
                            : txtAddress.Text.Trim());

                    command.Parameters.AddWithValue("@Id", farmerId);

                    int rowsUpdated = command.ExecuteNonQuery();
                
                    if (rowsUpdated == 0)
                    {
                        MessageBox.Show(
                            "The farmer record was not updated. " +
                            "It may no longer exist.",
                            "AquaLedger",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }

                MessageBox.Show(
                    "Farmer profile updated successfully!",
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadFarmerProfile();
                SetEditing(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to save farmer changes:\n" + ex.Message,
                    "AquaLedger",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            LoadFarmerProfile();
            SetEditing(false);
        }

        private void FarmerProfileForm_Load(
            object sender, EventArgs e)
        {
        }

        private void BtnChangePhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Farmer Photo";
                dialog.Filter =
                    "Image Files|*.jpg;*.jpeg;*.png";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    // Check that the selected file is a valid image.
                    using (Image testImage = Image.FromFile(dialog.FileName))
                    {
                        if (testImage.Width <= 0 || testImage.Height <= 0)
                            throw new InvalidDataException("Invalid image.");
                    }

                    string photoFolder = Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData),
                        "AquaLedger",
                        "FarmerPhotos");

                    Directory.CreateDirectory(photoFolder);

                    string extension =
                        Path.GetExtension(dialog.FileName).ToLowerInvariant();

                    string destinationPath = Path.Combine(
                        photoFolder,
                        "Farmer_" + farmerId + extension);

                    // Copy the selected image into AquaLedger's photo folder.
                    File.Copy(dialog.FileName, destinationPath, true);

                    // Save the photo path in the farmer's database record.
                    using (var connection = AQUALEDGERS.Data.Db.GetConnection())
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                    UPDATE Farmers
                    SET PhotoPath = @PhotoPath
                    WHERE Id = @Id;";

                        command.Parameters.AddWithValue(
                            "@PhotoPath", destinationPath);

                        command.Parameters.AddWithValue("@Id", farmerId);

                        if (command.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show(
                                "The farmer record could not be updated.",
                                "AquaLedger",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    currentPhotoPath = destinationPath;
                    DisplayFarmerPhoto(currentPhotoPath);

                    MessageBox.Show(
                        "Farmer photo saved successfully!",
                        "AquaLedger",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to save the farmer photo:\n" + ex.Message,
                        "AquaLedger",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void DisplayFarmerPhoto(string photoPath)
        {
            Image previousImage = picFarmerPhoto.Image;
            picFarmerPhoto.Image = null;
            previousImage?.Dispose();

            if (!string.IsNullOrWhiteSpace(photoPath) &&
                File.Exists(photoPath))
            {
                try
                {
                    // Load a separate copy so the file can be replaced later.
                    using (Image source = Image.FromFile(photoPath))
                    {
                        picFarmerPhoto.Image = new Bitmap(source);
                    }
                }
                catch
                {
                    picFarmerPhoto.Image = null;
                }
            }
        }

    }
}
