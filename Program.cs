using GZipTest.Utils;
using System.IO.Compression;

namespace GZipTest
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
        {
            int codeToReturn;
            ArgumentsCheker.Check(ref args);
            string sourceFileName = args[1];
            string outputFileName = args[2];

            CompressionMode GZipStreamCommand;
            GZipStreamCommand = args[0].ToLower() == "compress" ? CompressionMode.Compress : CompressionMode.Decompress;

            GZipModifier fileModifier = new GZipModifier(GZipStreamCommand, sourceFileName, outputFileName);
            codeToReturn = fileModifier.Start();

            return codeToReturn;
        }
    }
}