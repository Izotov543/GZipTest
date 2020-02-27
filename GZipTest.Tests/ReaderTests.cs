using GZipTest.IO;
using NUnit.Framework;
using System;
using System.IO;

namespace GZipTest.Tests
{
    [TestFixture]
    public class ReaderTests
    {
        [Test]
        public void ReaderTest()
        {
            byte[] byteArray = new byte[] { 0x08, 0x08, 0x08, 0x08, 0x08 };
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(
                "\\bin\\Debug\\netcoreapp3.0\\".ToCharArray()), "Test Files\\readerTest.txt");
            File.WriteAllBytes(filePath, byteArray);
            using Reader reader = new Reader(filePath);
            byte[] expectedResult = byteArray;
            byte[] actualResult = reader.ReadNextBlock().Data;
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}