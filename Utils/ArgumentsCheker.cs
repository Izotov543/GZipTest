using System;
using System.IO;

namespace GZipTest.Utils
{
    /// <summary>
    /// Содержит методы проверки аргументов командной строки переданные в метод
    /// <seealso cref="T:GZipTest.Program"/>.Main()
    /// </summary>
    internal static class ArgumentsCheker
    {
        /// <summary>
        /// Вызывает методы проверки аргументов командной строки переданные в метод
        /// <seealso cref="T:GZipTest.Program"/>.Main()
        /// </summary>
        internal static void Check(ref string[] args)
        {
            if (args.Length == 3)
            {
                args[0] = ChekGZipCommand(args[0]);
                args[1] = CheckSourceFile(args[1]);
                args[2] = CheckOutputFileAndDirectory(args[2]);
            }

            else
            {
                Console.WriteLine("Неверно указаны аргументы");
                args = new string[3];
                args[0] = ChekGZipCommand("");
                args[1] = CheckSourceFile("");
                args[2] = CheckOutputFileAndDirectory("");
            }
        }

        /// <summary>
        /// Проверяет аргумент <paramref name="inputCommand"></paramref>, отвечающий за режим приложения
        /// </summary>
        private static string ChekGZipCommand(string inputCommand)
        {
            while (inputCommand.ToLower() != "compress" && inputCommand.ToLower() != "decompress")
            {
                Console.WriteLine("Доступные режимы: сжатие {compress} и распаковка {decompress}");
                Console.WriteLine("Укажите требуемый режим:");
                inputCommand = Console.ReadLine();
            }

            return inputCommand;
        }

        /// <summary>
        /// Проверяет аргумент <paramref name="sourceFilePath"></paramref>, 
        /// содержащий полное имя исходного файла
        /// </summary>
        internal static string CheckSourceFile(string sourceFilePath)
        {
            while (!File.Exists(sourceFilePath))
            {
                Console.WriteLine("Укажите полное имя исходного файла:");
                sourceFilePath = Console.ReadLine();
            }

            return sourceFilePath;
        }

        /// <summary>
        /// Проверяет аргумент <paramref name="outputFilePath"></paramref>, 
        /// содержащий полное имя целевого файла
        /// </summary>
        internal static string CheckOutputFileAndDirectory(string outputFilePath)
        {
            if (outputFilePath.Length == 0)
            {
                outputFilePath = "stab\\";
            }

            string outputDirectoryName = outputFilePath.Substring(0, outputFilePath.LastIndexOf("\\"));
            string outputFileName = outputFilePath.Substring(outputFilePath.LastIndexOf("\\") + 1);
            while (!Directory.Exists(outputDirectoryName))
            {
                Console.WriteLine($"Укажите полное имя директории в которой следует создать файл:");
                outputDirectoryName = Console.ReadLine();
            }

            while (outputFileName.Length == 0)
            {
                Console.WriteLine($"Укажите имя файла которой следует создать (например example.gz):");
                outputFileName = Console.ReadLine();
            }
            
            return outputDirectoryName + "\\" + outputFileName;
        }
    }
}