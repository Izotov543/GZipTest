using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;

namespace GZipTest.FileModifiers
{
    /// <summary>
    /// Реализует сжатие блока
    /// </summary>
    internal class Compressor : IFileModifier
    {
        /// <summary>
        /// Реализует сжатие прочитанного блока <paramref name="blockToModify"></paramref>,
        /// и его последующее добавление в очередь на запись <paramref name="writeQueue"></paramref>.
        /// </summary>
        public void Modify(ConcurrentDictionary<int, 
            Block> blocksToWriteBuffer, Block blockToModify, 
            Action<Exception> onException)
        {
            try
            {
                if (blockToModify == null)
                {
                    return;
                }

                using MemoryStream compressedDataStream = new MemoryStream();
                using (var GZipStream = new GZipStream(compressedDataStream, CompressionMode.Compress))
                {
                    GZipStream.Write(blockToModify.Data, 0, blockToModify.Size);
                }

                byte[] compressedData = compressedDataStream.ToArray();
                byte[] compressedDataSize = BitConverter.GetBytes(compressedData.Length);
                byte[] dataToWrite = new byte[compressedData.Length + BlockSizeGetter.nextBlockSizeSectorLength];
                compressedDataSize.CopyTo(dataToWrite, 0);
                compressedData.CopyTo(dataToWrite, BlockSizeGetter.nextBlockSizeSectorLength);
                Block compressedBlock = new Block(dataToWrite, blockToModify.Number);
                blocksToWriteBuffer.TryAdd(compressedBlock.Number, compressedBlock);
            }

            catch (Exception e)
            {
                onException?.Invoke(e);
            }
        }
    }
}