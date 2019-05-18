using System;

namespace Reheader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Reheader.exe <file|dir> ...");
                return;
            }

            var reheader = new Reheader();
            reheader.Process(args);
        }
    }
}
