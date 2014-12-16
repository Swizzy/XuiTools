namespace XuiTools {
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Resx2Bin {
        public byte[] ConvertToKeyBasedTable(ResxObj[] input) {
            var list = new List<ResxObj>(input);
            list.Sort(ResxHelper.Comparison);
            input = list.ToArray();
            var ret = new List<byte>();
            ret.AddRange(new Byte[] {
                                        0x58,
                                        0x55,
                                        0x49,
                                        0x53,
                                        0x01,
                                        0x00,
                                        0x00,
                                        0x00,
                                        0x00,
                                        0x00,
                                        (byte)(input.Length >> 8),
                                        (byte)(input.Length)
                                    }); // Make header with placeholders for total size
            foreach(var resxObj in input) {
                ret.AddRange(resxObj.GetContentBytes());
                ret.AddRange(resxObj.GetKeyBytes());
            }
            ret[6] = (byte)(ret.Count >> 24);
            ret[7] = (byte)(ret.Count >> 16);
            ret[8] = (byte)(ret.Count >> 8);
            ret[9] = (byte)(ret.Count);
            return ret.ToArray();
        }

        public byte[] ConvertToKeyBasedTable(Stream input, bool closeStreamWhenDone = true) {
            return ConvertToKeyBasedTable(ResxHelper.ReadResxObjectsFromXml(input, true, closeStreamWhenDone));
        }

        public void ConvertToKeyBasedTable(ResxObj[] input, string output) { File.WriteAllBytes(output, ConvertToKeyBasedTable(input)); }

        public void ConvertToKeyBasedTable(Stream input, string output, bool closeStreamWhenDone = true) { File.WriteAllBytes(output, ConvertToKeyBasedTable(input, closeStreamWhenDone)); }

        public byte[] ConvertToIndexBasedTables(ResxObj[] input) {
            var ret = new List<byte>();
            ret.AddRange(new Byte[] {
                                        0x58,
                                        0x55,
                                        0x49,
                                        0x53,
                                        0x01,
                                        0x02,
                                        0x00,
                                        0x00,
                                        0x00,
                                        0x00,
                                        (byte)(input.Length >> 8),
                                        (byte)(input.Length)
                                    }); // Make header with placeholders for total size
            foreach(var resxObj in input)
                ret.AddRange(resxObj.GetContentBytes());
            ret[6] = (byte)(ret.Count >> 24);
            ret[7] = (byte)(ret.Count >> 16);
            ret[8] = (byte)(ret.Count >> 8);
            ret[9] = (byte)(ret.Count);
            return ret.ToArray();
        }

        public byte[] ConvertToIndexBasedTables(Stream input, bool closeStreamWhenDone = true) {
            return ConvertToIndexBasedTables(ResxHelper.ReadResxObjectsFromXml(input, false, closeStreamWhenDone));
        }

        public void ConvertToIndexBasedTables(ResxObj[] input, string output) { File.WriteAllBytes(output, ConvertToIndexBasedTables(input)); }

        public void ConvertToIndexBasedTables(Stream input, string output, bool closeStreamWhenDone = true) { File.WriteAllBytes(output, ConvertToIndexBasedTables(input, closeStreamWhenDone)); }
    }
}