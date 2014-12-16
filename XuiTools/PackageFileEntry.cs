namespace XuiTools {
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class PackageFileEntry
    {
        public readonly string Filename;
        private readonly byte[] _data; // Can be null
        private readonly string _dataFile; // Can be null

        public PackageFileEntry(string filename, string dataFile)
        {
            Filename = filename;
            _dataFile = dataFile;
        }

        public PackageFileEntry(string filename, byte[] data)
        {
            Filename = filename;
            _data = data;
        }

        private int FileSize
        {
            get
            {
                if (_data != null)
                    return _data.Length;
                return (int)new FileInfo(_dataFile).Length;
            }
        }

        public IEnumerable<byte> Data { get { return _data ?? File.ReadAllBytes(_dataFile); } }

        public IEnumerable<byte> MakeFsEntry(int dataOffset)
        {
            var ret = new List<byte>();
            ret.AddRange(new[] {
                                   (byte)(FileSize >> 24),
                                   (byte)(FileSize >> 16),
                                   (byte)(FileSize >> 8),
                                   (byte)(FileSize)
                               }); // Add File Size
            ret.AddRange(new[] {
                                   (byte)(dataOffset >> 24),
                                   (byte)(dataOffset >> 16),
                                   (byte)(dataOffset >> 8),
                                   (byte)(dataOffset)
                               }); // Add Data Offset
            if (Filename.Length > 0xD0)
            {
                ret.Add(0xD0);
                ret.AddRange(Encoding.BigEndianUnicode.GetBytes(Filename.ToCharArray(), 0, 0xD0)); // Only add the first 208 letters as this is what's supported!
            }
            else
            {
                ret.Add((byte)Filename.Length);
                ret.AddRange(Encoding.BigEndianUnicode.GetBytes(Filename));
            }
            return ret.ToArray();
        }

        public static int Comparison(PackageFileEntry fe1, PackageFileEntry fe2) { return string.CompareOrdinal(fe1.Filename, fe2.Filename); }
    }
}