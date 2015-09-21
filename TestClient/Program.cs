using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            //Save the public key information to an RSAParameters structure.
            RSAParameters RSAKeyInfo = RSA.ExportParameters(false);
            Console.WriteLine(RSA.ToXmlString(true));

            Trace.WriteLine("Testing");
            Trace.WriteLine("More Testing");
            Trace.Flush();
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}
