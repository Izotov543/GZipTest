using System.IO.Compression;

namespace GZipTest.FileModifiers
{
    /// <summary>
    /// Предназначен для создания конкретной сущности <seealso cref="T:GZipTest.FileModifiers.IFileModifier"/>.
    /// </summary>
    internal static class ModifierFactory
    {
        internal static IFileModifier Create(CompressionMode command)
        {
            switch (command)
            {
                case CompressionMode.Compress:
                    return new Compressor();

                default:
                    return new Decompressor();
            }
        }
    }
}