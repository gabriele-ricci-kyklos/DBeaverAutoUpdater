namespace DBeaverAutoUpdater.GUI
{
    partial class ConfigForm
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
            this.components = new System.ComponentModel.Container();
            this.titleLabel = new System.Windows.Forms.Label();
            this.installPathButton = new System.Windows.Forms.Button();
            this.installPathTextBox = new System.Windows.Forms.TextBox();
            this.installPathLabel = new System.Windows.Forms.Label();
            this.architectureComboBox = new System.Windows.Forms.ComboBox();
            this.architectureLabel = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(232, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(176, 31);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Configuration";
            // 
            // installPathButton
            // 
            this.installPathButton.AutoSize = true;
            this.installPathButton.Location = new System.Drawing.Point(545, 95);
            this.installPathButton.Name = "installPathButton";
            this.installPathButton.Size = new System.Drawing.Size(75, 27);
            this.installPathButton.TabIndex = 1;
            this.installPathButton.Text = "Choose";
            this.installPathButton.UseVisualStyleBackColor = true;
            this.installPathButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // installPathTextBox
            // 
            this.installPathTextBox.Location = new System.Drawing.Point(12, 96);
            this.installPathTextBox.Name = "installPathTextBox";
            this.installPathTextBox.Size = new System.Drawing.Size(527, 22);
            this.installPathTextBox.TabIndex = 2;
            // 
            // installPathLabel
            // 
            this.installPathLabel.AutoSize = true;
            this.installPathLabel.Location = new System.Drawing.Point(9, 76);
            this.installPathLabel.Name = "installPathLabel";
            this.installPathLabel.Size = new System.Drawing.Size(215, 17);
            this.installPathLabel.TabIndex = 3;
            this.installPathLabel.Text = "Choose the DBeaver install path:";
            // 
            // architectureComboBox
            // 
            this.architectureComboBox.FormattingEnabled = true;
            this.architectureComboBox.Items.AddRange(new object[] {
            "x86",
            "x64"});
            this.architectureComboBox.Location = new System.Drawing.Point(12, 161);
            this.architectureComboBox.Name = "architectureComboBox";
            this.architectureComboBox.Size = new System.Drawing.Size(121, 24);
            this.architectureComboBox.TabIndex = 4;
            // 
            // architectureLabel
            // 
            this.architectureLabel.AutoSize = true;
            this.architectureLabel.Location = new System.Drawing.Point(9, 141);
            this.architectureLabel.Name = "architectureLabel";
            this.architectureLabel.Size = new System.Drawing.Size(163, 17);
            this.architectureLabel.TabIndex = 5;
            this.architectureLabel.Text = "Choose the architecture:";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(279, 207);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 242);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.architectureLabel);
            this.Controls.Add(this.architectureComboBox);
            this.Controls.Add(this.installPathLabel);
            this.Controls.Add(this.installPathTextBox);
            this.Controls.Add(this.installPathButton);
            this.Controls.Add(this.titleLabel);
            this.Name = "ConfigForm";
            this.Text = "ConfigForm";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button installPathButton;
        private System.Windows.Forms.TextBox installPathTextBox;
        private System.Windows.Forms.Label installPathLabel;
        private System.Windows.Forms.ComboBox architectureComboBox;
        private System.Windows.Forms.Label architectureLabel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}