namespace XuiTools {
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ResxObj {
        public readonly string Content;
        public readonly string Key;

        public ResxObj(string content, string key) {
            Content = Regex.Replace(content, "\r\n|\r|\n", "\r\n");
            Key = Regex.Replace(key, "\r\n|\r|\n", "\r\n");
        }

        public byte[] GetContentBytes() {
            var ret = new List<byte>();
            var len = Content.Length;
            ret.Add((byte)(len >> 8));
            ret.Add((byte)(len));
            ret.AddRange(!Regex.IsMatch(Content, "[\u0600-\u06FF]+") ? Encoding.BigEndianUnicode.GetBytes(Content) : Encoding.BigEndianUnicode.GetBytes(RTeeL.RtlConverter.FixArabicAndFarsi(Content)));
            return ret.ToArray();
        }

        public byte[] GetKeyBytes() {
            var ret = new List<byte>();
            var len = Key.Length;
            ret.Add((byte)(len >> 8 & 0xff));
            ret.Add((byte)(len & 0xFF));
            ret.AddRange(Encoding.BigEndianUnicode.GetBytes(Key));
            return ret.ToArray();
        }
    }
}