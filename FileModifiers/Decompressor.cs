using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
namespace GZipTest.FileModifiers
{
    /// <summary>
    /// Реализует распаковку блока
    /// </summary>
    internal class Decompressor : IFileModifier
    {
        /// <summary>
        /// Реализует распаковку прочитанного блока <paramref name="blockToModify"></paramref>,
        /// и его последующее добавление в очередь на запись <paramref name="writeQueue"></paramref>.
        /// </summary>
        public void Modify(ConcurrentDictionary<int, 
            Block> blocksToWriteBuffer, Block blockToModify, 
            Action<Exception> onException)
        {
            try
            {
                Block decompressedBlock;
                using var decompressedDataStream = new MemoryStream();
                using var compressedDataStream = new MemoryStream(blockToModify.Data);
                using GZipStream GZipStream = new GZipStream(compressedDataStream, CompressionMode.Decompress);
                GZipStream.CopyTo(decompressedDataStream);
                decompressedBlock = new Block(decompressedDataStream.ToArray(), blockToModify.Number);
                blocksToWriteBuffer.TryAdd(decompressedBlock.Number, decompressedBlock);
            }

            catch (Exception e)
            {
                onException?.Invoke(e);
            }
        }
    }
}