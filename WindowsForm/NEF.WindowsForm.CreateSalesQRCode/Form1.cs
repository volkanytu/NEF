using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NEF.WindowsForm.CreateSalesQRCode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void genereQrCodeButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(salesNumberTextBox.Text))
            {
                this.Enabled = false;
                string retVal = Process.CreateQrCode(salesNumberTextBox.Text.ToUpper().Trim());
                salesNumberTextBox.Clear();
                MessageBox.Show(retVal, "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Enabled = true;
            }
            else
            {
                MessageBox.Show("Satış Numarası girmelisiniz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Enabled = true;
            }
        }
    }
}
