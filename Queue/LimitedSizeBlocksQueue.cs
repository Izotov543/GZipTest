using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Является элементом очереди ограничиваемой длины <see cref="T:GZipTest.LimitedSizeBlocksQueue"/>
    /// </summary>
    public class QueueItem
    {
        public Block Value { get; set; }
        public QueueItem Next { get; set; }
    }

    /// <summary>
    /// Предназначен для хранения элементов <see cref="T:GZipTest.QueueItem"/>,
    /// в количестве не более указанного при создании.
    /// <para>Организует добавление/извлечение элементов по принципу очереди (FIFO)</para>
    /// </summary>
    public class LimitedSizeBlocksQueue
    {
        private readonly AutoResetEvent enqueueEvent;
        private readonly ManualResetEvent dequeueEvent;
        private readonly int limit;
        private readonly object locker;
        private readonly QueueItem dummyHead = new QueueItem { Value = new Block(new byte[0], -1), Next = null };
        private QueueItem tail;
        private int nextBlockNumber;

        public int Count { get; set; }
        public bool IsWaitingForEnqueue { get; set; }
        public bool IsEmpty
        {
            get
            {
                return tail == dummyHead;
            }
        }

        public LimitedSizeBlocksQueue(int queueSize)
        {
            limit = queueSize;
            IsWaitingForEnqueue = true;
            enqueueEvent = new AutoResetEvent(true);
            dequeueEvent = new ManualResetEvent(true);
            locker = new object();
            nextBlockNumber = 0;
            tail = dummyHead;
        }

        private void AddAfter(QueueItem previousItem, QueueItem newItem)
        {
            previousItem.Next = newItem;
            tail = newItem;
        }

        public void Enqueue(Block value)
        {
            if (value == null)
            {
                return;
            }

            while (value.Number != nextBlockNumber || Count == limit)
            {
                enqueueEvent.WaitOne();
            }

            lock (locker)
            {
                if (Count != limit)
                {
                    AddAfter(tail, new QueueItem { Value = value, Next = null });
                    Count++;
                    nextBlockNumber++;
                    dequeueEvent.Set();
                }

                else
                {
                    dequeueEvent.Reset();
                }
            }
        }

        public Block Dequeue()
        {
            while (IsEmpty && IsWaitingForEnqueue)
            {
                dequeueEvent.WaitOne();
            }

            lock (locker)
            {
                Block result = null;
                if (dummyHead.Next != null)
                {
                    result = dummyHead.Next.Value;
                    dummyHead.Next = dummyHead.Next.Next;
                    Count--;
                    if (dummyHead.Next == null)
                    {
                        tail = dummyHead;
                    }

                    enqueueEvent.Set();
                }

                return result;
            }
        }
    }
}