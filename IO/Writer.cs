using System;
using System.IO;

namespace GZipTest.IO
{
    /// <summary>
    /// Реализует запись в файл
    /// </summary>
    public sealed class Writer : IDisposable
    {
        private readonly FileStream outputFileStream;
        public int TotalBlockWrite { get; private set; }

        /// <summary>
        /// <para>Конструктор. Возвращает экземпляр <see cref="T:GZipTest.IO.Writer"/>.</para>
        /// Принимает полное имя (<paramref name="fileToWritePath"></paramref>) выходного файла
        /// </summary>
        public Writer(string fileToWritePath)
        {
            {
                outputFileStream = new FileStream(fileToWritePath, FileMode.Create);
                TotalBlockWrite = 0;
            }
        }

        /// <summary>
        /// Записывает блоки байт (<paramref name="blockToWrite"></paramref>) 
        /// в указанный в конструкторе файл (<paramref name="fileToWritePath"></paramref>)
        /// </summary>
        public void Write(Block blockToWrite)
        {
            outputFileStream.Write(blockToWrite.Data, 0, blockToWrite.Size);
            TotalBlockWrite++;
        }

        public void Dispose()
        {
            outputFileStream.Close();
        }
    }
}