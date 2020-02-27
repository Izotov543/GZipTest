namespace GZipTest
{
    /// <summary>
    /// Экземпляр класса <see cref="T:GZipTest.Block"/> предназначен 
    /// для хранения массива бит (<paramref name="Data"></paramref>)
    /// и порядкового номера (<paramref name="Number"></paramref>) 
    /// для реализации последовательной записи в файл экземпляром
    /// класса <see cref="T:GZipTest.IO.Writer"/>
    /// </summary>
    public class Block
    {
        public byte[] Data;
        public int Number { get; }
        public int Size { get => Data.Length; }

        public Block(byte[] data, int number)
        {
            this.Data = data;
            Number = number;
        }
    }
}