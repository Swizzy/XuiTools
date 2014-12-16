namespace XuiTools {
    using System.Collections.Generic;
    using System.IO;

    internal static class ParseHelper {
        internal static int ReadSize16(IList<byte> data, int offset = 0) { return data[offset] << 8 | data[offset + 1]; }

        internal static int ReadSize16(Stream input) {
            var buf = new byte[2];
            input.Read(buf, 0, 2);
            return ReadSize16(buf);
        }

        internal static int ReadSize32(IList<byte> data, int offset = 0) { return data[offset] << 24 | data[offset + 1] << 16 | data[offset + 2] << 8 | data[offset + 3]; }

        internal static int ReadSize32(Stream input) {
            var buf = new byte[4];
            input.Read(buf, 0, 4);
            return ReadSize32(buf);
        }
    }
}