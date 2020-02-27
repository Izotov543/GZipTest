using System.IO;

namespace GZipTest.Utils
{
    /// <summary>
    /// Класс содержит методы расширения класса <see cref="T:System.IO.Stream"/>
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// Определяет, является ли передаваемый экземпляр <see cref="T:System.IO.Stream"/>
        /// частью архива формата GZip
        /// </summary>
        internal static bool IsGZipArchivePart(this Stream stream)
        {
            byte[] GZipDefaultHeader = { 0x1f, 0x8b, 0x08 };
            int BlockSizeSectorLength = 4;
            var temp = stream.Position;
            stream.Position += BlockSizeSectorLength;

            var header = new byte[GZipDefaultHeader.Length];
            stream.Read(header, 0, header.Length);

            stream.Position = temp;

            for (int i = 0; i < header.Length; i++)
            {
                if (header[i] != GZipDefaultHeader[i])
                    return false;
            }

            return true;
        }
    }
}