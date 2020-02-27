using System;
using System.IO;

namespace GZipTest.IO
{
    /// <summary>
    /// Реализует чтение из файла
    /// </summary>
    public sealed class Reader : IDisposable
    {
        private readonly FileInfo fileToRead;
        private readonly FileStream fileToReadStream;

        public int LastBlockPosition { get; private set; }
        public bool CanRead { get; private set; }

        /// <summary>
        /// <para>Конструктор. Возвращает экземпляр <see cref="T:GZipTest.IO.Reader"/>.</para>
        /// Принимает полное имя (<paramref name="fileToReadPath"></paramref>) читаемого файла
        /// </summary>
        public Reader(string fileToReadPath)
        {
            fileToRead = new FileInfo(fileToReadPath);
            fileToReadStream = fileToRead.OpenRead();
            LastBlockPosition = 0;
            CanRead = true;
        }

        /// <summary>
        /// Читает блоки из указанного в конструкторе файла (<paramref name="fileToReadPath"></paramref>)
        /// </summary>
        public Block ReadNextBlock()
        {
            while (fileToReadStream.Length > fileToReadStream.Position)
            {
                int dataLength = BitConverter.ToInt32(BlockSizeGetter.GetNextBlockSize(fileToReadStream), 0);
                byte[] data = new byte[dataLength];
                fileToReadStream.Read(data, 0, data.Length);
                return new Block(data, LastBlockPosition++);
            }

            CanRead = false;
            return null;
        }

        public void Dispose()
        {
            fileToReadStream.Close();
        }
    }
}