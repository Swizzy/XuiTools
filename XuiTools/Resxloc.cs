namespace XuiTools {
    using System.Collections.Generic;
    using System.IO;

    public class Resxloc {
        public string[] SplitResxMaster(Stream input, string outputdir) {
            var list = SplitResxMaster(input);
            var ret = new List<string>();
            foreach(var key in list.Keys) {
                var dir = !string.IsNullOrEmpty(key) ? Path.Combine(outputdir, key + ".resx") : Path.Combine(outputdir, "_.resx");
                if(key == null)
                    continue;
                ResxHelper.SaveResxFile(list[key], dir);
                ret.Add(dir);
            }
            return ret.ToArray();
        }

        public Dictionary<string, List<ResxObj>> SplitResxMaster(Stream input, bool closeStreamWhenDone = true) {
            var list = new Dictionary<string, List<ResxObj>>();
            foreach(var resxobj in ResxHelper.ReadResxObjectsFromXml(input, true, closeStreamWhenDone)) {
                if(resxobj.Key.Contains(".")) {
                    var resxfile = resxobj.Key.Substring(0, resxobj.Key.IndexOf('.'));
                    if(!list.ContainsKey(resxfile))
                        list.Add(resxfile, new List<ResxObj>());
                    list[resxfile].Add(new ResxObj(resxobj.Content, resxobj.Key.Substring(resxobj.Key.IndexOf('.') + 1)));
                }
                else {
                    if(!list.ContainsKey(""))
                        list.Add("", new List<ResxObj>());
                    list[""].Add(resxobj);
                }
            }
            return list;
        }
    }
}