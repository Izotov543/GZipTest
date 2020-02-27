using GZipTest.FileModifiers;
using GZipTest.IO;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading;
using static GZipTest.Utils.ExceptionHandlers;

namespace GZipTest
{
    /// <summary>
    /// Содержит методы чтения из файла, сжатия/распаковки файла и записи в файл
    /// </summary>
    public class GZipModifier
    {
        #region private fields
        private readonly int threadsNumber;
        private readonly Thread[] threadPool;
        private readonly CountdownEvent countdown;
        private readonly LimitedSizeBlocksQueue readQueue;
        private readonly ConcurrentDictionary<int, Block> writeBuffer;
        private readonly CompressionMode gZipStreamCommand;
        private readonly IFileModifier fileModifier;
        private readonly string fileToReadPath;
        private readonly string fileToWritePath;
        private readonly string[] args;
        internal event Action<Exception> OnException;
        private bool canAddBlockToBuffer;
        private bool allBlocksWriten;
        private int blocksReaded;
        #endregion

        /// <summary>
        /// <para>Конструктор. Возвращает экземпляр <see cref="T:GZipTest.GZipModifier"/></para>
        /// <para>Принимает:</para> <para><paramref name="gZipStreamCommand"></paramref> - 
        /// режим работы <see cref="T:System.IO.Compression.GZipStream"/>
        /// (<paramref name="gZipStreamCommand"></paramref>)</para>
        /// <para>полное имя (<paramref name="fileToReadPath"></paramref>) читаемого файла</para>
        /// <para>полное имя (<paramref name="fileToWritePath"></paramref>) записываемого файла</para>
        /// </summary>
        #region constructor
        public GZipModifier(CompressionMode gZipStreamCommand, string fileToReadPath, string fileToWritePath)
        {
            threadsNumber = Environment.ProcessorCount;
            threadPool = new Thread[threadsNumber];
            this.gZipStreamCommand = gZipStreamCommand;
            this.fileToReadPath = fileToReadPath;
            this.fileToWritePath = fileToWritePath;
            args = new string[] { gZipStreamCommand.ToString(), fileToReadPath, fileToWritePath };
            fileModifier = ModifierFabric.Create(gZipStreamCommand);
            readQueue = new LimitedSizeBlocksQueue(threadsNumber);
            writeBuffer = new ConcurrentDictionary<int, Block>();
            countdown = new CountdownEvent(threadsNumber + 1);
            canAddBlockToBuffer = true;
            allBlocksWriten = false;
            blocksReaded = 0;
        }
        #endregion

        /// <summary>
        /// Вызывает методы чтения из файла, сжатия/распаковки файла и записи в файл
        /// </summary>
        public int Start()
        {
            string operation;
            operation = gZipStreamCommand == CompressionMode.Compress ? "сжатия" : "распаковки";
            Console.WriteLine($"Начато выполнение {operation}." +
                $"\nПосле завершения операции, приложение будет закрыто");
            OnException += (e) => ThreadExceptionHandler.Run(e);
            ReadFile();
            ModifyFile();
            WriteFile();
            countdown.Wait();
            return 0;
        }

        /// <summary>
        /// Реализует поблочное чтение из файла
        /// </summary>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        private void ReadFile()
        {
            Thread readerThread = new Thread(() =>
            {
                Reader fileReader = null;
                try
                {
                    fileReader = new Reader(fileToReadPath);
                    while (fileReader.CanRead)
                    {
                        Block block = fileReader.ReadNextBlock();
                        readQueue.Enqueue(block);
                        blocksReaded++;
                    }
                }

                catch (IOException e)
                {
                    IOExceptionHandler.Run(e.Message, args, "Reader");
                }

                catch (UnauthorizedAccessException e)
                {
                    UnauthorizedAccessExceptionHandler.Run(e.Message, args, "Reader");
                }

                catch (Exception e)
                {
                    OnException?.Invoke(e);
                }

                finally
                {
                    if (fileReader != null)
                    {
                        fileReader.Dispose();
                    }
                }

                readQueue.IsWaitingForEnqueue = false;
                blocksReaded--;
                Console.WriteLine(Thread.CurrentThread.Name + " ended");
                countdown.Signal();
            });

            readerThread.Name = "ReaderThread";
            readerThread.Start();
        }

        /// <summary>
        /// Запускает процесс сжатия/распаковки файла в многопоточном режиме
        /// </summary>
        private void ModifyFile()
        {
            for (int i = 0; i < threadsNumber; i++)
            {
                threadPool[i] = new Thread(() =>
                {
                    while (readQueue.IsWaitingForEnqueue || !readQueue.IsEmpty)
                    {
                        if (canAddBlockToBuffer)
                        {
                            Block blockToModify = readQueue.Dequeue();
                            if (blockToModify != null)
                            {
                                fileModifier.Modify(writeBuffer, blockToModify, OnException);
                                if (writeBuffer.Count >= 100)
                                {
                                    canAddBlockToBuffer = false;
                                }
                            }
                        }
                    }

                    Console.WriteLine(Thread.CurrentThread.Name + " ended");
                    countdown.Signal();
                });

                threadPool[i].Name = "ModifyThread " + i;
                threadPool[i].Start();
            }
        }

        /// <summary>
        /// Реализует поблочную запись в файл
        /// </summary>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        private void WriteFile()
        {
            bool fileHadHiddenAttribute = (File.Exists(fileToWritePath) && TryToRemoveHiddenAtribute(fileToWritePath));
            int blockToWriteNumber = 0;
            try
            {
                using Writer fileWriter = new Writer(fileToWritePath);
                Block blockToWrite = null;
                while (!allBlocksWriten)
                {
                    if (writeBuffer.ContainsKey(blockToWriteNumber))
                    {
                        if (writeBuffer[blockToWriteNumber] != null)
                        {
                            writeBuffer.TryRemove(blockToWriteNumber, out blockToWrite);
                            blockToWriteNumber++;
                            fileWriter.Write(blockToWrite);
                        }
                    }

                    if (writeBuffer.Count < 100)
                    {
                        canAddBlockToBuffer = true;
                    }

                    if (blocksReaded == fileWriter.TotalBlockWrite && !readQueue.IsWaitingForEnqueue)
                    {
                        allBlocksWriten = true;
                    }
                }
            }

            catch (IOException e)
            {
                if (fileHadHiddenAttribute)
                {
                    SetHiddenAttribute();
                }

                IOExceptionHandler.Run(e.Message, args, "Writer");
            }

            catch (UnauthorizedAccessException e)
            {
                if (fileHadHiddenAttribute)
                {
                    SetHiddenAttribute();
                }

                UnauthorizedAccessExceptionHandler.Run(e.Message, args, "Writer");
            }

            if (fileHadHiddenAttribute)
            {
                SetHiddenAttribute();
            }
        }

        /// <summary>
        /// При наличии атрибута "Hidden" у целевого файла снимает данный атрибут 
        /// и возвращает значение <see cref="true"/>
        /// </summary>
        private bool TryToRemoveHiddenAtribute(string filePath)
        {
            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Hidden))
            {
                File.SetAttributes(filePath, File.GetAttributes(filePath) & ~FileAttributes.Hidden);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Добавляет атрибут "Hidden" к уже установленным
        /// </summary>
        private void SetHiddenAttribute()
        {
            File.SetAttributes(fileToWritePath, File.GetAttributes(fileToWritePath) | FileAttributes.Hidden);
        }
    }
}