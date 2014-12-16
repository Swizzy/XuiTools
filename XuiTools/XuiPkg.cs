namespace XuiTools {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class XuiPkg {
        private readonly List<PackageFileEntry> _files = new List<PackageFileEntry>();

        public PackageFileEntry[] Files { get { return _files.ToArray(); } }

        public void AddFile(string filename, byte[] data) { _files.Add(new PackageFileEntry(filename, data)); }

        public void AddFile(string filename, string data) { _files.Add(new PackageFileEntry(filename, data)); }

        public void SaveToFile(Stream output, bool closeWhenDone = true) {
            var dataBlob = new List<byte>();
            var fsBlob = new List<byte>();
            _files.Sort(PackageFileEntry.Comparison);
            foreach(var fileEntry in _files) {
                fsBlob.AddRange(fileEntry.MakeFsEntry(dataBlob.Count));
                dataBlob.AddRange(fileEntry.Data);
            }
            var header = new List<byte>();
            header.AddRange(Encoding.ASCII.GetBytes("XUIZ")); // Add Magic
            header.AddRange(new byte[3]); // Add Static 1
            header.Add(0x01); // Add Static 1 last digit
            var xzpSize = dataBlob.Count + fsBlob.Count + 0x16; // Calculate total size
            header.AddRange(new[] {
                                      (byte)(xzpSize >> 24),
                                      (byte)(xzpSize >> 16),
                                      (byte)(xzpSize >> 8),
                                      (byte)(xzpSize)
                                  }); // Add Total Size
            header.AddRange(new byte[4]); // Add Static 2
            header.AddRange(new[] {
                                      (byte)(fsBlob.Count >> 24),
                                      (byte)(fsBlob.Count >> 16),
                                      (byte)(fsBlob.Count >> 8),
                                      (byte)(fsBlob.Count)
                                  }); // Add FSTable Size
            header.AddRange(new[] {
                                      (byte)(_files.Count >> 8),
                                      (byte)(_files.Count)
                                  });
            header.AddRange(fsBlob);
            header.AddRange(dataBlob);
            output.Write(header.ToArray(), 0, header.Count);
            if (closeWhenDone)
                output.Close();
        }

        public void ClearFileTable() { _files.Clear(); }

        public void ReadFromFile(Stream input) {
            if (input.Position != 0 && input.CanSeek)
                input.Seek(0, SeekOrigin.Begin);
            else if (input.Position != 0)
                throw new InvalidOperationException("input must be at the start or be seekable!");
            var buf = new byte[4];
            input.Read(buf, 0, 4);
            var magic = Encoding.ASCII.GetString(buf);
            if (magic != "XUIZ")
                throw new InvalidDataException(string.Format("Invalid Header, expected XUIZ got: {0}", magic));
            input.Read(buf, 0, 4); // Discard File Version
            var xzpSize = ParseHelper.ReadSize32(input); // Read XZP file size
            var fsSize = ParseHelper.ReadSize32(input); // Read File Table Size
            var fsCount = ParseHelper.ReadSize16(input); // Read File Entry Count
            var fsBlob = new byte[fsSize];
            input.Read(fsBlob, 0, fsBlob.Length);
            var dataBlobSize = xzpSize - (0x16 + fsSize);
            var dataBlob = new byte[dataBlobSize];
            input.Read(dataBlob, 0, dataBlob.Length);
            var fsBlobOffset = 0;
            for(var i = 0; i < fsCount && fsBlobOffset < fsBlob.Length; i++) {
                var data = new byte[ParseHelper.ReadSize32(fsBlob, fsBlobOffset)];
                fsBlobOffset += 4;
                Buffer.BlockCopy(dataBlob, ParseHelper.ReadSize32(fsBlob, fsBlobOffset), data, 0, data.Length);
                fsBlobOffset += 4;
                _files.Add(new PackageFileEntry(Encoding.BigEndianUnicode.GetString(fsBlob, fsBlobOffset + 1, fsBlob[fsBlobOffset] * 2), data));
                fsBlobOffset += (fsBlob[fsBlobOffset] * 2) + 1;
            }
        }
    }
}