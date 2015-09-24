using System;
using System.Diagnostics;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.WriteLine("Testing");
            Trace.WriteLine("More Testing");
            Trace.WriteLine("Even More Testing");
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}
