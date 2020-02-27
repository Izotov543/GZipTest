using System;
using System.Collections.Concurrent;

namespace GZipTest.FileModifiers
{
    interface IFileModifier
    {
        void Modify(ConcurrentDictionary<int, Block> blocksToWriteBuffer, 
            Block blockToModify, 
            Action<Exception> onException);
    }
}