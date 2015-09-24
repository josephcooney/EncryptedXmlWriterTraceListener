using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

namespace JCooney.Net.Diagnostics.Test
{
    public class EncryptedXmlTextWriterTraceListenerTest
    {
        [Fact]
        public void CanWriteEncryptedTrace()
        {
            var rsa = new RSACryptoServiceProvider();
            var parameters = rsa.ExportParameters(true);
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temp);
            var formatted = EncryptedXmlWriterTraceListenerInitialisationDataParser.Format(parameters.Modulus, parameters.Exponent, temp);
            var testText = Guid.NewGuid().ToString() + " this is a test";
            var staticTestText = "This is another test";
            using (var writer = new EncryptedXmlWriterTraceListener(formatted))
            {
                writer.Write(testText);
                writer.WriteLine(staticTestText);
                writer.Flush();
                //writer.Close();
            }

            var files = Directory.GetFiles(temp).Select(a => new FileInfo(a)).OrderByDescending(b => b.CreationTime);
            var mostRecent = files.First();

            System.Diagnostics.Debug.WriteLine("Checking output file " + mostRecent.FullName);

            var decrypter = new EncryptedXmlWriterDecrypter();
            var decryptedStream = decrypter.Decrypt(parameters, mostRecent.FullName);
            using (var reader = new StreamReader(decryptedStream))
            {
                var decryptedText = reader.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(decryptedText);
                Assert.True(decryptedText.Contains(testText), "Asserting that decrypted text contains test text");
                Assert.True(decryptedText.Contains(staticTestText), "Asserting that decrypted text contains additional static text");
                Assert.True(decryptedText.StartsWith("<E2ETraceEvent"), "Asserting traced begins as expected");
            }

            File.Delete(mostRecent.FullName);
            Directory.Delete(temp);
        }
    }
}
