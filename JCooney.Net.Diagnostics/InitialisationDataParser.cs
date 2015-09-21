using System;

namespace JCooney.Net.Diagnostics
{
    public class EncryptedXmlWriterTraceListenerInitialisationDataParser
    {
        public static EncryptedXmlWriterTraceListenerInitialisationData Parse(string initialisationDataString)
        {
            // expects Key=<public key base64>;Directory=<directory goes here> 
            if (string.IsNullOrEmpty(initialisationDataString))
            {
                throw new ArgumentException("initialisationDataString");
            }

            const string keyPrefix = "Key=";
            const string exponentPrefix = "Exp=";
            const string directoryPrefix = "Directory=";

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
