using NUnit.Framework;
using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Tests
{
    [TestFixture]
    public class GZipModifierTests
    {
        [Test]
        public void GZipModifierCompressTest()
        {
            CompressionMode command = CompressionMode.Compress;
            string sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(
                "\\bin\\Debug\\netcoreapp3.0\\".ToCharArray()), "Test Files\\sample1MB.txt");
            string targetFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(
                "\\bin\\Debug\\netcoreapp3.0\\".ToCharArray()), "Test Files\\target1MB.gz");
            string sampleArchivePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(
                "\\bin\\Debug\\netcoreapp3.0\\".ToCharArray()), "Test Files\\sample1MB.gz");
            GZipModifier gZipModifier = new GZipModifier(command, sourceFilePath, targetFilePath);
            gZipModifier.Start();
            byte[] expectedResult = File.ReadAllBytes(sampleArchivePath);
            byte[] actualResult = File.ReadAllBytes(targetFilePath);
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GZipModifierDecompressTest()
        {
            CompressionMode command = CompressionMode.Decompress;
            string sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(
                "\\bin\\Debug\\netcoreapp3.0\\".ToCharArray()), "Test Files\\sample1MB.gz");
            string targetFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(
                "\\bin\\Debug\\netcoreapp3.0\\".ToCharArray()), "Test Files\\target1MB.txt");
            string sampleArchivePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Trim(
                "\\bin\\Debug\\netcoreapp3.0\\".ToCharArray()), "Test Files\\sample1MB.txt");
            GZipModifier gZipModifier = new GZipModifier(command, sourceFilePath, targetFilePath);
            gZipModifier.Start();
            byte[] expectedResult = File.ReadAllBytes(sampleArchivePath);
            byte[] actualResult = File.ReadAllBytes(targetFilePath);
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }
    }
}