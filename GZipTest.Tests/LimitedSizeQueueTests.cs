using NUnit.Framework;
using System;

namespace GZipTest.Tests
{
    [TestFixture]
    public class LimitedSizeQueueTests
    {
        [TestCase(1, 1)]
        [TestCase(3, 3)]
        public void LimitedSizeQueueEnqueue(int enqueuesCount, int expectedQueueBlocksCount)
        {
            LimitedSizeBlocksQueue testQueue = new LimitedSizeBlocksQueue(3);
            for (int i = 0; i < enqueuesCount; i++)
            {
                testQueue.Enqueue(new Block(BitConverter.GetBytes(i), i));                
            }

            int actualQueueBlocksCount = testQueue.Count;
            Assert.AreEqual(expectedQueueBlocksCount, actualQueueBlocksCount);
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        public void LimitedSizeQueueDequeue(int dequeuesCount, int expectedBlocksNumber)
        {
            LimitedSizeBlocksQueue testQueue = new LimitedSizeBlocksQueue(3);
            for (int i = 0; i < 3; i++)
            {
                testQueue.Enqueue(new Block(BitConverter.GetBytes(i), i));
            }

            Block tempBlock = null;
            for (int i = 0; i < dequeuesCount; i++)
            {
                tempBlock = testQueue.Dequeue();
            }

            int actualBlocksNumber = tempBlock.Number;
            Assert.AreEqual(expectedBlocksNumber, actualBlocksNumber);
        }
    }
}