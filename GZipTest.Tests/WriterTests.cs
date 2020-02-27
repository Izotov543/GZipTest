using GZipTest.IO;
using NUnit.Framework;
using System;
using System.IO;

namespace GZipTest.Tests
{
    [TestFixture]
    public class WriterTests
    {
        [Test]
        public void WriterTest()
        {
            byte[] byteArray = new byte[] { 0x08, 0x08, 0x08, 0x08, 0x08 };
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(
                "\\bin\\Debug\\netcoreapp3.0\\".ToCharArray()), "Test Files\\writerTest.txt");
            Writer writer = new Writer(filePath); 
            writer.Write(new Block(byteArray, 0));
            writer.Dispose();
            byte[] expectedResult = byteArray;
            byte[] actualResult = File.ReadAllBytes(filePath);
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}