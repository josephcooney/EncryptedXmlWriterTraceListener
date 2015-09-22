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
            using (var writer = new EncryptedXmlWriterTraceListener(formatted))
            {
                writer.Write(testText);
                writer.Flush();
                writer.Close();
            }

            var files = Directory.GetFiles(temp).Select(a => new FileInfo(a)).OrderByDescending(b => b.CreationTime);
            var mostRecent = files.First();

            Console.WriteLine("Checking output file " + mostRecent.FullName);

            var decrypter = new EncryptedXmlTextWriterDecrypter();
            var decryptedStream = decrypter.Decrypt(parameters, mostRecent.FullName);
            using (var reader = new StreamReader(decryptedStream))
            {
                var decryptedText = reader.ReadToEnd();
                Console.WriteLine(decryptedText);
                Assert.True(decryptedText.Contains(testText), "Asserting that decrypted text contains test text");
            }

            File.Delete(mostRecent.FullName);
            Directory.Delete(temp);
        }
    }
}
