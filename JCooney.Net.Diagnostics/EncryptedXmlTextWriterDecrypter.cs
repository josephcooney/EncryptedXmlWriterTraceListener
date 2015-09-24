using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JCooney.Net.Diagnostics
{
    public class EncryptedXmlWriterDecrypter
    {
        public Stream Decrypt(RSAParameters privateKey, string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new ArgumentException(string.Format("File {0} does not exist", fileName));
            }

            var rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(privateKey);

            var ms = new MemoryStream();

            using (var fs = new FileStream(fileName, FileMode.Open))
            using (var reader = new StreamReader(fs))
            {
                var keyLength = ReadUntil(reader, EncryptedXmlWriterTraceListener.Delimiter);
                var keyString = ReadUntil(reader, EncryptedXmlWriterTraceListener.Delimiter);
                var ivLength = ReadUntil(reader, EncryptedXmlWriterTraceListener.Delimiter);
                var ivString = ReadUntil(reader, EncryptedXmlWriterTraceListener.Delimiter);

                var key = rsaProvider.Decrypt(Convert.FromBase64String(keyString), false);
                var iv = rsaProvider.Decrypt(Convert.FromBase64String(ivString), false);

                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Padding = PaddingMode.Zeros;
                rijndaelManaged.IV = iv;
                rijndaelManaged.Key = key;

                fs.Seek(keyLength.Length + int.Parse(keyLength) + ivLength.Length + int.Parse(ivLength) + 4, SeekOrigin.Begin);
                var crypto = new CryptoStream(fs, rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV), CryptoStreamMode.Read);
                crypto.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                rijndaelManaged.Dispose();
            }

            rsaProvider.Dispose();
            return ms;
        }

        private string ReadUntil(StreamReader reader, char c)
        {
            var sb = new StringBuilder();
            char r = (char)reader.Read();
            while (r != c && !reader.EndOfStream)
            {
                sb.Append(r);
                r = (char)reader.Read();
            }

            return sb.ToString();
        }
    }
}
