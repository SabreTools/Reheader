using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reheader
{
    public class Reheader
    {
        private const string mappingFile = "MesenDB.txt";
        private readonly Dictionary<string, HeaderInfo> headerMapping = new Dictionary<string, HeaderInfo>();

        public Reheader()
        {
            if (!File.Exists(mappingFile))
                return;

            // Populate the header mapping dictionary
            using (TextReader reader = File.OpenText(mappingFile))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    // Comment lines start with #
                    if (line.StartsWith("#"))
                        continue;

                    var headerInfo = new HeaderInfo(line);
                    headerMapping.Add(headerInfo.CRC, headerInfo);
                }
            }
        }

        /// <summary>
        /// Process a list of paths that may contain unheadered NES files
        /// </summary>
        /// <param name="paths"></param>
        public void Process(string[] paths)
        {
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    if (!this.AddHeader(Path.GetFullPath(path), Path.GetFullPath(path) + "-headered"))
                        Console.WriteLine($"Could not find a header for {path}");
                }
                else if (Directory.Exists(path))
                {
                    foreach (string file in Directory.EnumerateFiles(Path.GetFullPath(path), "*", SearchOption.AllDirectories))
                        if (!this.AddHeader(Path.GetFullPath(file), Path.GetFullPath(file) + "-headered"))
                            Console.WriteLine($"Could not find a header for {file}");
                }
                else
                {
                    Console.WriteLine($"{path} is not a file or directory");
                }
            }
        }

        /// <summary>
        /// Add an iNES header to a given input file, if applicable
        /// </summary>
        /// <param name="infile">Input file to check and header</param>
        /// <param name="outfile">Output file path</param>
        /// <returns>True if a header was able to be generated and added, false otherwise</returns>
        private bool AddHeader(string infile, string outfile)
        {
            Console.WriteLine($"Processing {infile}");

            if (!File.Exists(infile))
                return false;

            // Get the CRC hash of the file
            string crc = GetCRCFromFile(infile).ToUpperInvariant();

            // See if that's a known CRC
            if (!headerMapping.ContainsKey(crc))
                return false;

            // Get the mapping
            var header = headerMapping[crc];

            // Get the new bytes
            var headerBytes = header.GenerateHeader(false);

            using (BinaryReader reader = new BinaryReader(File.OpenRead(infile)))
            using (BinaryWriter writer = new BinaryWriter(File.Create(outfile)))
            {
                // Write the header
                writer.Write(headerBytes);

                // Now the rest
                var tempBytes = reader.ReadBytes((int)reader.BaseStream.Length);
                writer.Write(tempBytes);
                writer.Flush();
            }

            Console.WriteLine($"Found header for {infile}. Wrote new file to {outfile}.");

            return true;
        }

        /// <summary>
        /// Get the CRC32 hash for a file, if possible
        /// </summary>
        /// <param name="file">Path to the file</param>
        /// <returns>String representation of the CRC32 hash of the file</returns>
        private string GetCRCFromFile(string file)
        {
            using (OptimizedCRC crc = new OptimizedCRC())
            using (BinaryReader reader = new BinaryReader(File.OpenRead(file)))
            {
                byte[] buffer = new byte[8 * 1024];
                int read = Int32.MaxValue;
                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    crc.Update(buffer, 0, read);
                }

                crc.Update(buffer, 0, 0);
                byte[] crcHash = BitConverter.GetBytes(crc.Value).Reverse().ToArray();

                return ByteArrayToString(crcHash);
            }
        }

        /// <summary>
        /// Convert a byte array to a hex string
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <returns>Hex string representing the byte array</returns>
        /// <link>http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa</link>
        private string ByteArrayToString(byte[] bytes)
        {
            // If we get null in, we send null out
            if (bytes == null)
                return null;

            try
            {
                string hex = BitConverter.ToString(bytes);
                return hex.Replace("-", string.Empty).ToLowerInvariant();
            }
            catch
            {
                return null;
            }
        }
    }
}
