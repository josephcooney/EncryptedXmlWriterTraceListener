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
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKeyTextbox.Text);
            var info = RSA.ExportParameters(true);

            var decrypter = new EncryptedXmlTextWriterDecrypter();
            var decryptedStream = decrypter.Decrypt(info, fileNameTextbox.Text);
            decryptedStream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(decryptedStream))
            {
                var content = reader.ReadToEnd();
                System.Diagnostics.Trace.WriteLine(content);
            }
        }
    }
}
