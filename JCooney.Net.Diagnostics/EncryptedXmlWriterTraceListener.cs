using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace JCooney.Net.Diagnostics
{
    public class EncryptedXmlWriterTraceListener : XmlWriterTraceListener
    {
        public const char Delimiter = '|';

        public EncryptedXmlWriterTraceListener(Stream stream) : base(stream)
        {
            throw new NotImplementedException();
        }

        public EncryptedXmlWriterTraceListener(Stream stream, string name) : base(stream, name)
        {
            throw new NotImplementedException();
        }

        public EncryptedXmlWriterTraceListener(TextWriter writer) : base(writer)
        {
            throw new NotImplementedException();
        }

        public EncryptedXmlWriterTraceListener(TextWriter writer, string name) : base(writer, name)
        {
            throw new NotImplementedException();
        }

        public EncryptedXmlWriterTraceListener(string initialisationData)
            : base(CreateSymmetricCryptoStream(EncryptedXmlWriterTraceListenerInitialisationDataParser.Parse(initialisationData)))
        {
        }

        public EncryptedXmlWriterTraceListener(string initialisationData, string name) 
            : base(CreateSymmetricCryptoStream(EncryptedXmlWriterTraceListenerInitialisationDataParser.Parse(initialisationData)), name)
        {
        }
        

        private static Stream CreateSymmetricCryptoStream(EncryptedXmlWriterTraceListenerInitialisationData config)
        {
            if (!Directory.Exists(config.DirectoryName))
            {
                return null;
            }

            var fileName = Guid.NewGuid() + ".log";
            var path = Path.Combine(config.DirectoryName, fileName);
            var stream = new FileStream(path, FileMode.CreateNew);

            
            var rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(new RSAParameters { Modulus = config.PublickKey, Exponent = config.Exponent });

            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Padding = PaddingMode.Zeros;
            rijndaelManaged.GenerateIV();
            rijndaelManaged.GenerateKey();

            var symmetricKey = rsaProvider.Encrypt(rijndaelManaged.Key, false);
            var symmetricIV = rsaProvider.Encrypt(rijndaelManaged.IV, false);
            rsaProvider.Dispose();

            var keyBase64 = Convert.ToBase64String(symmetricKey);
            var ivBase64 = Convert.ToBase64String(symmetricIV);
            var textWriter = new StreamWriter(stream);
            var keyAndIv = keyBase64.Length.ToString() + Delimiter + keyBase64 + Delimiter + ivBase64.Length + Delimiter + ivBase64 + Delimiter;
            textWriter.Write(keyAndIv);
            textWriter.Flush();
            

            var cryptoStream = new CryptoStream(stream, rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV), CryptoStreamMode.Write);
            
            return cryptoStream;
        }
    }
}
