namespace NEF.WindowsForm.CreateSalesQRCode
{
    partial class Form1
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.salesNumberTextBox = new System.Windows.Forms.TextBox();
            this.genereQrCodeButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.genereQrCodeButton);
            this.groupBox1.Controls.Add(this.salesNumberTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(482, 103);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Satış Numarası";
            // 
            // salesNumberTextBox
            // 
            this.salesNumberTextBox.Location = new System.Drawing.Point(121, 15);
            this.salesNumberTextBox.Name = "salesNumberTextBox";
            this.salesNumberTextBox.Size = new System.Drawing.Size(349, 22);
            this.salesNumberTextBox.TabIndex = 1;
            // 
            // genereQrCodeButton
            // 
            this.genereQrCodeButton.Location = new System.Drawing.Point(282, 52);
            this.genereQrCodeButton.Name = "genereQrCodeButton";
            this.genereQrCodeButton.Size = new System.Drawing.Size(188, 36);
            this.genereQrCodeButton.TabIndex = 2;
            this.genereQrCodeButton.Text = "QR Kod Oluştur";
            this.genereQrCodeButton.UseVisualStyleBackColor = true;
            this.genereQrCodeButton.Click += new System.EventHandler(this.genereQrCodeButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 103);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 150);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 150);
            this.Name = "Form1";
            this.Text = "NEF Satış - QR Code ";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button genereQrCodeButton;
        private System.Windows.Forms.TextBox salesNumberTextBox;
        private System.Windows.Forms.Label label1;
    }
}

