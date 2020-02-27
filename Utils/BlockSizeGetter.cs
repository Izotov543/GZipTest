using GZipTest.Utils;
using System;
using System.IO;

namespace GZipTest
{
    /// <summary>
    /// Вспомогательный статический класс
    /// </summary>
    internal static class BlockSizeGetter
    {
        internal const int nextBlockSizeSectorLength = 4;
        internal const int defaultBlockSize = 1048576;

        /// <summary>
        /// Возвращает длину следующего записываемого блока в байтах 
        /// на основании передаваемого экземпляра <see cref="T:System.IO.Stream"/>
        /// </summary>
        internal static byte[] GetNextBlockSize(Stream stream)
        {
            if (stream.IsGZipArchivePart())
            {
                var nextBlockLength = new byte[nextBlockSizeSectorLength];
                stream.Read(nextBlockLength, 0, nextBlockLength.Length);
                return nextBlockLength;
            }

            else
            {
                var nextBlockLength = (int)Math.Min(defaultBlockSize, stream.Length - stream.Position);
                return BitConverter.GetBytes(nextBlockLength);
            }
        }
    }
}