using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using JCooney.Net.Diagnostics;

namespace TestDecrypter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void decryptButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(privateKeyTextbox.Text))
            {
                MessageBox.Show("Please choose a private key");
                return;
            }

            if (string.IsNullOrEmpty(fileNameTextbox.Text))
            {
                MessageBox.Show("Please choose a file to decrypt");
                return;
            }

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKeyTextbox.Text);
            var info = RSA.ExportParameters(true);

            var decrypter = new EncryptedXmlWriterDecrypter();
            var decryptedStream = decrypter.Decrypt(info, fileNameTextbox.Text);
            decryptedStream.Seek(0, SeekOrigin.Begin);

            var randomOutFileName = fileNameTextbox.Text + Guid.NewGuid() + ".xml";


            using (var outFileStream = new FileStream(randomOutFileName, FileMode.CreateNew))
            {
                decryptedStream.CopyTo(outFileStream);
                outFileStream.Flush();
            }
            MessageBox.Show("Wrote output to:\n\n" + randomOutFileName, "Decrypted File", MessageBoxButtons.OK);
        }

        private void fileBrowseButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                var result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fileNameTextbox.Text = dlg.FileName;
                }
            }
        }
    }
}
