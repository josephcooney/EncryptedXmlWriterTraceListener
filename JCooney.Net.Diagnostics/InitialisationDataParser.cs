using System;

namespace JCooney.Net.Diagnostics
{
    public class EncryptedXmlWriterTraceListenerInitialisationDataParser
    {
        private const string keyPrefix = "Key=";
        private const string exponentPrefix = "Exp=";
        private const string directoryPrefix = "Directory=";

        public static string Format(byte[] key, byte[] exponent, string directory)
        {
            return keyPrefix + Convert.ToBase64String(key) + ";" + exponentPrefix + Convert.ToBase64String(exponent) + ";" + directoryPrefix + directory;
        }

        public static EncryptedXmlWriterTraceListenerInitialisationData Parse(string initialisationDataString)
        {
            // expects Key=<public key base64>;Exp=<exponent>;Directory=<directory goes here> 
            if (string.IsNullOrEmpty(initialisationDataString))
            {
                throw new ArgumentException("initialisationDataString");
            }

            var result = new EncryptedXmlWriterTraceListenerInitialisationData();

            var parts = initialisationDataString.Split(';');
            foreach (var part in parts)
            {
                if (part.StartsWith(keyPrefix))
                {
                    result.PublickKey = GetByteValue(part, keyPrefix);
                }

                if (part.StartsWith(exponentPrefix))
                {
                    result.Exponent = GetByteValue(part, exponentPrefix);
                }

                if (part.StartsWith(directoryPrefix))
                {
                    result.DirectoryName = part.Replace(directoryPrefix, "");
                }
            }

            return result;
        }

        private static byte[] GetByteValue(string part, string prefix)
        {
            var keyBase64 = part.Replace(prefix, "");
            if (keyBase64.Length > 0)
            {
                return Convert.FromBase64String(keyBase64);
            }
            return null;
        }
    }

    public class EncryptedXmlWriterTraceListenerInitialisationData
    {
        public string DirectoryName { get; set; }
        public byte[] PublickKey { get; set; }
        public byte[] Exponent { get; set; }
    }
}
