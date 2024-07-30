namespace XuiTools {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class Bin2Resx {
        public IEnumerable<ResxObj> ConvertToResxObjects(Stream input) {
            if(input.Position != 0 && input.CanSeek)
                input.Seek(0, SeekOrigin.Begin);
            else if(input.Position != 0)
                throw new InvalidOperationException("input must be at the start or be seekable!");
            var buf = new byte[4];
            input.Read(buf, 0, 4);
            var magic = Encoding.ASCII.GetString(buf);
            if(magic != "XUIS")
                throw new InvalidDataException(string.Format("Invalid Header, expected XUIS got: {0}", magic));
            input.Read(buf, 0, 2);
            if(buf[0] != 0x01 || (buf[1] != 0x00 && buf[1] != 0x02))
                throw new NotSupportedException();
            var indexed = buf[1] == 0x02;
            input.Read(buf, 0, 4); // We don't need this shit!
            var ret = new List<ResxObj>();
            var count = ParseHelper.ReadSize16(input);
            for(var index = 0; index < count; index++) {
                var size = ParseHelper.ReadSize16(input);
                buf = new byte[size * 2];
                input.Read(buf, 0, buf.Length);
                var content = Encoding.BigEndianUnicode.GetString(buf);
                if(!indexed) {
                    size = ParseHelper.ReadSize16(input);
                    buf = new byte[size * 2];
                    input.Read(buf, 0, buf.Length);
                    ret.Add(new ResxObj(content, Encoding.BigEndianUnicode.GetString(buf)));
                }
                else
                    ret.Add(new ResxObj(content, string.Format("Index{0}", index)));
            }
            return ret.ToArray();
        }

        public void ConvertToResx(Stream input, string output) { ResxHelper.SaveResxFile(ConvertToResxObjects(input), output); }
    }
}
