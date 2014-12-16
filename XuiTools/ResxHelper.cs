namespace XuiTools {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    internal static class ResxHelper {
        internal static int Comparison(ResxObj o1, ResxObj o2) { return String.Compare(o1.Key, o2.Key, StringComparison.Ordinal); }

        internal static ResxObj[] ReadResxObjectsFromXml(Stream input, bool sort = false, bool closeStreamWhenDone = true) {
            var ret = new List<ResxObj>();
            using(var xml = XmlReader.Create(input)) {
                while(xml.Read()) {
                    if(!xml.IsStartElement())
                        continue; // We don't care about the ending ones
                    if(!xml.Name.Equals("data", StringComparison.CurrentCultureIgnoreCase))
                        continue; // Not what we're looking for
                    var key = xml[0];
                    xml.Read();
                    if(!xml.IsStartElement() || !xml.Name.Equals("value", StringComparison.CurrentCultureIgnoreCase))
                        continue; // Invalid!
                    xml.Read();
                    ret.Add(new ResxObj(xml.Value, key));
                }
            }
            if(sort)
                ret.Sort(Comparison);
            if (closeStreamWhenDone)
                input.Close();
            return ret.ToArray();
        }

        internal static void SaveResxFile(IEnumerable<ResxObj> input, string filename) {
            using(var xml = XmlWriter.Create(filename, new XmlWriterSettings {
                                                                               OmitXmlDeclaration = true,
                                                                               ConformanceLevel = ConformanceLevel.Fragment,
                                                                               Encoding = Encoding.UTF8,
                                                                               CloseOutput = true,
                                                                           })) {
                xml.WriteStartElement("root");
                foreach(var resx in input) {
                    xml.WriteStartElement("data"); // <data 
                    xml.WriteAttributeString("name", resx.Key); // name="[resx.Key]">
                    xml.WriteElementString("value", resx.Content ?? ""); // <value>[resx.Content]</value>
                    xml.WriteEndElement(); // </data>
                    xml.WriteWhitespace("\n");
                }
                xml.WriteEndElement();
                xml.Close();
            }
        }
    }
}